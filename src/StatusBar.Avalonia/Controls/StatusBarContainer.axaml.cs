using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace StatusBar.Avalonia.Controls;

/// <summary>
///  Represents a container for status bar items.
/// </summary>
[PseudoClasses(PC_HasBackground)]
public sealed class StatusBarContainer : TemplatedControl
{
    private const string PART_LeftContainer = "PART_LeftContainer";
    private const string PART_CenterContainer = "PART_CenterContainer";
    private const string PART_RightContainer = "PART_RightContainer";

    private const string PC_HasBackground = ":has-background";

    public static readonly StyledProperty<BoxShadows> BoxShadowProperty =
        Border.BoxShadowProperty.AddOwner<StatusBarContainer>();

    public static readonly DirectProperty<StatusBarContainer, AvaloniaList<string>> DisabledItemsProperty =
        AvaloniaProperty.RegisterDirect<StatusBarContainer, AvaloniaList<string>>(
            nameof(DisabledItems),
            o => o.DisabledItems,
            (o, v) => o.DisabledItems = v
        );

    public AvaloniaList<string> DisabledItems
    {
        get;
        set => SetAndRaise(DisabledItemsProperty, ref field, value);
    } = [];

    public BoxShadows BoxShadow
    {
        get => GetValue(BoxShadowProperty);
        set => SetValue(BoxShadowProperty, value);
    }

    private StackPanel? _leftContainer;
    private StackPanel? _centerContainer;
    private StackPanel? _rightContainer;

    private readonly ConcurrentQueue<StatusBarEntry> _pendingItems = new();

    static StatusBarContainer()
    {
        HeightProperty.OverrideDefaultValue(typeof(StatusBarContainer), 28);
    }

    public StatusBarContainer()
    {
        DisabledItems.CollectionChanged -= DisabledItems_CollectionChanged;
        DisabledItems.CollectionChanged += DisabledItems_CollectionChanged;
    }

    /// <inheritdoc />
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _leftContainer = e.NameScope.Get<StackPanel>(PART_LeftContainer);
        _centerContainer = e.NameScope.Get<StackPanel>(PART_CenterContainer);
        _rightContainer = e.NameScope.Get<StackPanel>(PART_RightContainer);

        ContextRequested += OnContextRequested;
    }

    /// <inheritdoc />
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        while (_pendingItems.TryDequeue(out var item))
        {
            AddStatusBarEntry(item);
        }
    }

    internal void AddStatusBarEntry(StatusBarEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        if (!IsLoaded)
        {
            _pendingItems.Enqueue(entry);
            return;
        }

        var targetContainer = entry.Alignment switch
        {
            StatusBarAlignment.Center => _centerContainer,
            StatusBarAlignment.Right => _rightContainer,
            _ => _leftContainer,
        };

        var insertIndex = GetInsertIndex(targetContainer!.Children, entry.Priority);
        targetContainer.Children.Insert(insertIndex, entry);

        entry.Disposed += OnStatusBarItemDisposed;

        if (DisabledItems.Contains(entry.Id))
        {
            entry.IsVisible = false;
        }

        return;

        static int GetInsertIndex(IList<Control> items, int priority)
        {
            for (var i = 0; i < items.Count; i++)
            {
                if (items[i] is StatusBarEntry existingItem && existingItem.Priority < priority)
                {
                    return i;
                }
            }

            return items.Count;
        }
    }

    private void OnStatusBarItemDisposed(object? sender, EventArgs e)
    {
        if (sender is not StatusBarEntry item)
        {
            return;
        }

        item.Disposed -= OnStatusBarItemDisposed;

        _leftContainer!.Children.Remove(item);
        _centerContainer!.Children.Remove(item);
        _rightContainer!.Children.Remove(item);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == DisabledItemsProperty)
        {
            var oldItems = change.OldValue as AvaloniaList<string>;
            var newItems = change.NewValue as AvaloniaList<string>;

            if (oldItems is not null)
            {
                oldItems.CollectionChanged -= DisabledItems_CollectionChanged;
            }

            if (newItems is not null)
            {
                newItems.CollectionChanged -= DisabledItems_CollectionChanged;
                newItems.CollectionChanged += DisabledItems_CollectionChanged;
            }

            OnDisabledItemsChanged(oldItems, newItems);
        }
    }

    private void DisabledItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnDisabledItemsChanged(e.OldItems, e.NewItems);
    }

    private void OnDisabledItemsChanged(IList? changeOldValue, IList? changeNewValue)
    {
        var oldItems = changeOldValue?.Cast<string>().ToList() ?? [];
        var newItems = changeNewValue?.Cast<string>().ToList() ?? [];

        var itemsToShow = oldItems.Where(x => !newItems.Contains(x)).ToArray();
        var itemsToHide = newItems.Where(x => !oldItems.Contains(x)).ToArray();

        if (itemsToShow.Length == 0 && itemsToHide.Length == 0)
            return;

        var allItems = _leftContainer
            ?.Children.OfType<StatusBarEntry>()
            .Concat(_centerContainer?.Children.OfType<StatusBarEntry>() ?? [])
            .Concat(_rightContainer?.Children.OfType<StatusBarEntry>() ?? []);

        if (allItems == null)
            return;

        foreach (var item in allItems)
        {
            if (itemsToShow.Contains(item.Id))
            {
                item.IsVisible = true;
            }
            else if (itemsToHide.Contains(item.Id))
            {
                item.IsVisible = false;
            }
        }
    }

    private void OnContextRequested(object? sender, ContextRequestedEventArgs args)
    {
        if (args.Handled)
        {
            return;
        }

        var sourceControl = (Control)args.Source!;

        // build the context menu

        var activeEntry = sourceControl.FindAncestorOfType<StatusBarEntry>(true);
        if (activeEntry?.IsTemporary == true)
            activeEntry = null;

        var menu = new ContextMenu
        {
            Placement = PlacementMode.Custom,
            CustomPopupPlacementCallback = parameters =>
            {
                var offsetX = args.TryGetPosition((Control)parameters.Target, out var position) ? position.X : 0;
                var offsetY = -parameters.PopupSize.Height / 2;

                parameters.Anchor = PopupAnchor.TopLeft;
                parameters.Offset = parameters.Offset.WithY(offsetY).WithX(offsetX);
            },
        };

        foreach (
            var entry in _leftContainer
                .Children.OfType<StatusBarEntry>()
                .Concat(_centerContainer.Children.OfType<StatusBarEntry>())
                .Concat(_rightContainer.Children.OfType<StatusBarEntry>())
                .Where(e => !e.IsTemporary)
        )
        {
            if (entry == activeEntry)
            {
                continue;
            }

            menu.Items.Add(
                new MenuItem()
                {
                    // TODO: display name
                    Header = entry.Id,
                    IsChecked = !DisabledItems.Contains(entry.Id),
                    Tag = entry.Id,
                    ToggleType = MenuItemToggleType.CheckBox,
                }
            );
        }

        if (activeEntry != null)
        {
            menu.Items.Add("-");

            var hideItem = new MenuItem { Header = $"Hide '{activeEntry.Id}'" };
            hideItem.Click += (_, _) => DisabledItems.Add(activeEntry.Id);
            menu.Items.Add(hideItem);
        }

        menu.Closed += delegate
        {
            var enabledItems = new List<string>();
            var disabledItems = new List<string>();

            foreach (var menuItem in menu.Items.OfType<MenuItem>())
            {
                if (menuItem.Tag is not string id)
                {
                    continue;
                }

                switch (menuItem)
                {
                    case { IsChecked: true }:
                        enabledItems.Add(id);
                        break;
                    case { IsChecked: false }:
                        disabledItems.Add(id);
                        break;
                }
            }

            var disabledItemsToAdd = disabledItems.Where(x => !DisabledItems.Contains(x)).ToArray();
            if (disabledItemsToAdd.Length > 0)
            {
                DisabledItems.AddRange(disabledItemsToAdd);
            }

            if (enabledItems.Count > 0)
            {
                DisabledItems.RemoveAll(enabledItems);
            }
        };

        menu.Open(sourceControl);
    }
}
