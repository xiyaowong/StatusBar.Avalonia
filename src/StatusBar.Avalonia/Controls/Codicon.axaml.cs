using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace StatusBar.Avalonia.Controls;

internal class Codicon : TemplatedControl
{
    private static readonly Dictionary<string, StreamGeometry> SpinIconPathMap = new()
    {
        {
            "gear",
            StreamGeometry.Parse(
                "M9.1 4.4L8.6 2H7.4l-.5 2.4-.7.3-2-1.3-.9.8 1.3 2-.2.7-2.4.5v1.2l2.4.5.3.8-1.3 2 .8.8 2-1.3.8.3.4 2.3h1.2l.5-2.4.8-.3 2 1.3.8-.8-1.3-2 .3-.8 2.3-.4V7.4l-2.4-.5-.3-.8 1.3-2-.8-.8-2 1.3-.7-.2zM9.4 1l.5 2.4L12 2.1l2 2-1.4 2.1 2.4.4v2.8l-2.4.5L14 12l-2 2-2.1-1.4-.5 2.4H6.6l-.5-2.4L4 13.9l-2-2 1.4-2.1L1 9.4V6.6l2.4-.5L2.1 4l2-2 2.1 1.4.4-2.4h2.8zm.6 7c0 1.1-.9 2-2 2s-2-.9-2-2 .9-2 2-2 2 .9 2 2zM8 9c.6 0 1-.4 1-1s-.4-1-1-1-1 .4-1 1 .4 1 1 1z"
            )
        },
        {
            "loading",
            StreamGeometry.Parse(
                // "M13.917 7A6.002 6.002 0 0 0 2.083 7H1.071a7.002 7.002 0 0 1 13.858 0h-1.012z"
                "M10,20c-5.51,0-10-4.49-10-10S4.49,0,10,0c1.05,0,2.09.16,3.09.49.53.17.81.73.64,1.26-.17.53-.73.81-1.26.64-.8-.26-1.63-.39-2.47-.39-4.41,0-8,3.59-8,8s3.59,8,8,8c4.41,0,8-3.59,8-8,0-.55.45-1,1-1s1,.45,1,1c0,5.51-4.49,10-10,10Z"
            )
        },
        {
            "sync",
            StreamGeometry.Parse(
                "M2.006 8.267L.78 9.5 0 8.73l2.09-2.07.76.01 2.09 2.12-.76.76-1.167-1.18a5 5 0 0 0 9.4 1.983l.813.597a6 6 0 0 1-11.22-2.683zm10.99-.466L11.76 6.55l-.76.76 2.09 2.11.76.01 2.09-2.07-.75-.76-1.194 1.18a6 6 0 0 0-11.11-2.92l.81.594a5 5 0 0 1 9.3 2.346z"
            )
        },
    };

    public static readonly StyledProperty<string?> IconProperty = AvaloniaProperty.Register<Codicon, string?>(
        nameof(Icon)
    );

    public string? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly StyledProperty<bool> SpinProperty = AvaloniaProperty.Register<Codicon, bool>(nameof(Spin));

    public bool Spin
    {
        get => GetValue(SpinProperty);
        set => SetValue(SpinProperty, value);
    }

    private Path? _spinIconPath;
    private TextBlock? _iconText;

    /// <inheritdoc />
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _spinIconPath = e.NameScope.Find<Path>("PART_SpinIconPath");
        _iconText = e.NameScope.Find<TextBlock>("PART_IconText");

        UpdateIcon();
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IconProperty || change.Property == SpinProperty)
        {
            UpdateIcon();
        }
    }

    private void UpdateIcon()
    {
        if (_spinIconPath == null || _iconText == null)
            return;

        if (!Spin)
        {
            _spinIconPath.IsVisible = false;
            _iconText.IsVisible = true;
            _iconText.Text = Icon != null ? IconProvider.GetIcon(Icon) ?? Icon : null;
            return;
        }

        var path = SpinIconPathMap!.GetValueOrDefault(Icon);

        if (path is null)
        {
            _spinIconPath.IsVisible = false;
            _iconText.IsVisible = true;
            _iconText.Text = Icon != null ? IconProvider.GetIcon(Icon) ?? Icon : null;
            return;
        }

        _spinIconPath.IsVisible = true;
        _spinIconPath.Data = path;
        _iconText.IsVisible = false;
    }
}
