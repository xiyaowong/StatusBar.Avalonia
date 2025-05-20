using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;

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

    private StackPanel _leftContainer = null!;
    private StackPanel _centerContainer = null!;
    private StackPanel _rightContainer = null!;

    private readonly ConcurrentQueue<StatusBarItemView> _pendingItems = new();

    static StatusBarContainer()
    {
        HeightProperty.OverrideDefaultValue(typeof(StatusBarContainer), 28);
    }

    /// <inheritdoc />
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _leftContainer = e.NameScope.Get<StackPanel>(PART_LeftContainer);
        _centerContainer = e.NameScope.Get<StackPanel>(PART_CenterContainer);
        _rightContainer = e.NameScope.Get<StackPanel>(PART_RightContainer);
    }

    /// <inheritdoc />
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        while (_pendingItems.TryDequeue(out var item))
        {
            AddStatusBarItemView(item);
        }
    }

    internal void AddStatusBarItemView(StatusBarItemView newItem)
    {
        ArgumentNullException.ThrowIfNull(newItem);

        if (!IsLoaded)
        {
            _pendingItems.Enqueue(newItem);
            return;
        }

        var targetContainer = newItem.Alignment switch
        {
            StatusBarAlignment.Center => _centerContainer,
            StatusBarAlignment.Right => _rightContainer,
            _ => _leftContainer,
        };

        var insertIndex = GetInsertIndex(targetContainer.Children, newItem.Priority);
        targetContainer.Children.Insert(insertIndex, newItem);

        newItem.Disposed += OnStatusBarItemDisposed;

        return;

        static int GetInsertIndex(IList<Control> items, int newItemPriority)
        {
            for (var i = 0; i < items.Count; i++)
            {
                if (items[i] is StatusBarItemView existingItem && existingItem.Priority < newItemPriority)
                {
                    return i;
                }
            }

            return items.Count;
        }
    }

    private void OnStatusBarItemDisposed(object? sender, EventArgs e)
    {
        if (sender is not StatusBarItemView item)
            return;

        item.Disposed -= OnStatusBarItemDisposed;

        _leftContainer.Children.Remove(item);
        _centerContainer.Children.Remove(item);
        _rightContainer.Children.Remove(item);
    }

    public static readonly StyledProperty<BoxShadows> BoxShadowProperty =
        Border.BoxShadowProperty.AddOwner<StatusBarContainer>();

    public BoxShadows BoxShadow
    {
        get => GetValue(BoxShadowProperty);
        set => SetValue(BoxShadowProperty, value);
    }
}
