using System;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Media;

namespace StatusBar.Avalonia.Controls;

/// <summary>
/// Represents a status bar item.
/// </summary>
[PseudoClasses(PC_Pressed, PC_HasClick, PC_HasBackground, PC_HasForeground)]
internal partial class StatusBarEntry : ContentControl
{
    private const string PC_Pressed = ":pressed";
    private const string PC_HasClick = ":has-click";
    private const string PC_HasForeground = ":has-foreground";
    private const string PC_HasBackground = ":has-background";

    private const string SPIN_SUFFIX = "~spin";

    #region Avalonia Properties

    /// <summary>
    /// Defines the <see cref="Id"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string> IdProperty = AvaloniaProperty.Register<StatusBarEntry, string>(
        nameof(Id)
    );

    public static readonly StyledProperty<string?> DisplayNameProperty = AvaloniaProperty.Register<
        StatusBarEntry,
        string?
    >(nameof(DisplayName));

    /// <summary>
    /// Defines the <see cref="Alignment"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<StatusBarAlignment> AlignmentProperty = AvaloniaProperty.Register<
        StatusBarEntry,
        StatusBarAlignment
    >(nameof(Alignment));

    /// <summary>
    /// Defines the <see cref="Priority"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<int> PriorityProperty = AvaloniaProperty.Register<StatusBarEntry, int>(
        nameof(Priority)
    );

    /// <summary>
    /// Defines the <see cref="Text"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<StatusBarEntry, string>(
        nameof(Text),
        string.Empty
    );

    /// <summary>
    /// Defines the <see cref="ToolTip"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string> ToolTipProperty = AvaloniaProperty.Register<StatusBarEntry, string>(
        nameof(ToolTip),
        string.Empty
    );

    /// <summary>
    /// Defines the <see cref="Click"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Action?> ClickProperty = AvaloniaProperty.Register<StatusBarEntry, Action?>(
        nameof(Click)
    );

    /// <summary>
    /// Defines the <see cref="IsShow"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> IsShowProperty = AvaloniaProperty.Register<StatusBarEntry, bool>(
        nameof(IsShow)
    );

    /// <summary>
    /// Defines the <see cref="Color"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> ColorProperty = AvaloniaProperty.Register<StatusBarEntry, IBrush?>(
        nameof(Color)
    );

    /// <summary>
    /// Defines the <see cref="BackgroundColor"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> BackgroundColorProperty = AvaloniaProperty.Register<
        StatusBarEntry,
        IBrush?
    >(nameof(BackgroundColor));

    /// <summary>
    /// Defines the <see cref="IsTemporary"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> IsTemporaryProperty = AvaloniaProperty.Register<StatusBarEntry, bool>(
        nameof(IsTemporary)
    );

    #endregion

    #region Properties

    /// <summary>
    ///
    /// </summary>
    public string Id
    {
        get => GetValue(IdProperty);
        set => SetValue(IdProperty, value);
    }

    public string? DisplayName
    {
        get => GetValue(DisplayNameProperty);
        set => SetValue(DisplayNameProperty, value);
    }

    /// <summary>
    ///
    /// </summary>
    public StatusBarAlignment Alignment
    {
        get => GetValue(AlignmentProperty);
        set => SetValue(AlignmentProperty, value);
    }

    /// <summary>
    ///
    /// </summary>
    public int Priority
    {
        get => GetValue(PriorityProperty);
        set => SetValue(PriorityProperty, value);
    }

    /// <summary>
    ///
    /// </summary>
    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    ///
    /// </summary>
    public string ToolTip
    {
        get => GetValue(ToolTipProperty);
        set => SetValue(ToolTipProperty, value);
    }

    /// <summary>
    ///
    /// </summary>
    public Action? Click
    {
        get => GetValue(ClickProperty);
        set => SetValue(ClickProperty, value);
    }

    /// <summary>
    ///
    /// </summary>
    public bool IsShow
    {
        get => GetValue(IsShowProperty);
        set => SetValue(IsShowProperty, value);
    }

    public IBrush? Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public IBrush? BackgroundColor
    {
        get => GetValue(BackgroundColorProperty);
        set => SetValue(BackgroundColorProperty, value);
    }

    public bool IsTemporary
    {
        get => GetValue(IsTemporaryProperty);
        set => SetValue(IsTemporaryProperty, value);
    }

    #endregion


    internal EventHandler? Disposed;

    private Border _rootBorder = null!;

    private bool isLeftButtonPressed
    {
        get;
        set
        {
            field = value;
            PseudoClasses.Set(PC_Pressed, value);
        }
    }

    private TextBlock? _richText;

    /// <inheritdoc />
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _rootBorder = e.NameScope.Get<Border>("PART_Root");
        _richText = e.NameScope.Find<TextBlock>("PART_RichText");

        _rootBorder.PointerPressed += RootBorder_PointerPressed;
        _rootBorder.PointerReleased += RootBorder_PointerReleased;

        PseudoClasses.Set(PC_HasClick, Click != null);
        PseudoClasses.Set(PC_HasForeground, Color != null);
        PseudoClasses.Set(PC_HasBackground, BackgroundColor != null);

        OnTextChanged();
    }

    private void RootBorder_PointerPressed(object? sender, PointerPressedEventArgs args)
    {
        if (Click == null)
            return;

        if (args.GetCurrentPoint((Control)sender!).Properties.IsLeftButtonPressed)
        {
            isLeftButtonPressed = true;
        }
    }

    private void RootBorder_PointerReleased(object? sender, PointerReleasedEventArgs args)
    {
        if (Click == null)
            return;

        if (!isLeftButtonPressed)
            return;

        isLeftButtonPressed = false;

        if (!IsPointerOver)
            return;

        Click?.Invoke();
        args.Handled = true;
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ClickProperty)
        {
            PseudoClasses.Set(PC_HasClick, change.NewValue != null);
        }
        else if (change.Property == BackgroundColorProperty)
        {
            PseudoClasses.Set(PC_HasBackground, change.NewValue != null);
        }
        else if (change.Property == ColorProperty)
        {
            PseudoClasses.Set(PC_HasForeground, change.NewValue != null);
        }
        else if (change.Property == TextProperty)
        {
            OnTextChanged();
        }
    }

    private void OnTextChanged()
    {
        if (_richText == null)
            return;

        var statusText = Text;

        if (string.IsNullOrEmpty(statusText) || !IconPattern.IsMatch(statusText))
        {
            _richText.Inlines?.Clear();
            return;
        }

        var inlines = new InlineCollection();
        var matches = IconPattern.Matches(statusText);

        var lastIndex = 0;
        foreach (Match match in matches)
        {
            if (match.Index > lastIndex)
            {
                var text = statusText[lastIndex..match.Index];
                inlines.Add(new Run(text));
            }
            else
            {
                var _iconName = match.Groups[1].Value;
                var spin = _iconName.EndsWith(SPIN_SUFFIX);
                var iconName = spin ? _iconName[..^SPIN_SUFFIX.Length] : _iconName;
                if (IconProvider.GetIcon(iconName) != null)
                {
                    inlines.Add(new Codicon { Icon = iconName, Spin = spin });
                }
                else
                {
                    inlines.Add(new Run(match.Value));
                }
            }

            lastIndex = match.Index + match.Length;
        }

        if (lastIndex < statusText.Length)
        {
            inlines.Add(new Run(statusText[lastIndex..]));
        }

        _richText.Inlines = inlines;
    }

    internal void Dispose()
    {
        IsShow = false;
        Disposed?.Invoke(this, EventArgs.Empty);
        Disposed = null;
    }

    #region Converters

    [GeneratedRegex(@"\$\(([^)]+)\)", RegexOptions.Compiled)]
    private static partial Regex IconRegex();

    private static readonly Regex IconPattern = IconRegex();

    public static FuncValueConverter<string, string> ResolveIconConverter { get; } =
        new(text =>
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return IconPattern.Replace(
                text,
                match =>
                {
                    var iconName = match.Groups[1].Value;
                    var icon = IconProvider.GetIcon(iconName);
                    return icon ?? text;
                }
            );
        });

    public static IValueConverter TextHasIconConverter { get; } =
        new FuncValueConverter<string?, bool>(text => !string.IsNullOrEmpty(text) && IconPattern.IsMatch(text));

    public static IValueConverter TextHasIconReverseConverter { get; } =
        new FuncValueConverter<string?, bool>(text => string.IsNullOrEmpty(text) || !IconPattern.IsMatch(text));

    #endregion
}
