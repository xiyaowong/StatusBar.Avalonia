using System;
using Avalonia.Media;
using Avalonia.Threading;
using StatusBar.Avalonia.Controls;

namespace StatusBar.Avalonia;

/// <summary>
/// Controls the status bar item.
/// </summary>
public sealed class StatusBarItem : IDisposable
{
    private StatusBarEntry? _entry;

    private bool _isDisposed;

    internal StatusBarItem(StatusBarEntry entry)
    {
        Id = entry.Id;
        Alignment = entry.Alignment;
        Priority = entry.Priority;
        Text = entry.Text;
        ToolTip = entry.ToolTip;
        Click = entry.Click;
        Color = entry.Color;
        BackgroundColor = entry.BackgroundColor;
        FontWeight = entry.FontWeight;

        _entry = entry;
    }

    /// <summary>
    /// The identifier of this item.
    /// </summary>
    public string Id { get; private set; }

    /// <summary>
    /// The alignment of this item.
    /// </summary>
    public StatusBarAlignment Alignment { get; private set; }

    /// <summary>
    /// The priority of this item.
    /// Higher value means the item should be shown more to the left.
    /// </summary>
    public int Priority { get; private set; }

    /// <summary>
    /// The text to show for the entry. You can embed icons in the text by leveraging the syntax:
    ///
    /// <code>My text $(icon-name) contains icons like $(icon-name) this one.</code>
    ///
    /// Where the icon-name is taken from the ThemeIcon [icon set](https://code.visualstudio.com/api/references/icons-in-labels#icon-listing),
    /// e.g. light-bulb`, `thumbsup`, `zap` etc.
    /// </summary>
    public string Text
    {
        get;
        set
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);

            if (_entry == null)
                return;

            field = value;

            if (Dispatcher.UIThread.CheckAccess())
                _entry.Text = value;
            else
                Dispatcher.UIThread.Invoke(() => _entry.Text = value);
        }
    }

    /// <summary>
    /// The tooltip text when you hover over this entry.
    /// </summary>
    public string ToolTip
    {
        get;
        set
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);

            if (_entry == null)
                return;

            field = value;

            if (Dispatcher.UIThread.CheckAccess())
                _entry.ToolTip = value;
            else
                Dispatcher.UIThread.Invoke(() => _entry.ToolTip = value);
        }
    }

    /// <summary>
    /// Action to be executed when the item is clicked.
    /// </summary>
    public Action? Click
    {
        get;
        set
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);

            if (_entry == null)
                return;

            field = value;

            if (Dispatcher.UIThread.CheckAccess())
                _entry.Click = value;
            else
                Dispatcher.UIThread.Invoke(() => _entry.Click = value);
        }
    }

    /// <summary>
    /// The foreground color of the item.
    /// </summary>
    public IBrush? Color
    {
        get;
        set
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);

            if (_entry == null)
                return;

            field = value;

            if (Dispatcher.UIThread.CheckAccess())
                _entry.Color = value;
            else
                Dispatcher.UIThread.Invoke(() => _entry.Color = value);
        }
    }

    /// <summary>
    /// The background color of the item.
    /// </summary>
    public IBrush? BackgroundColor
    {
        get;
        set
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);

            if (_entry == null)
                return;

            field = value;

            if (Dispatcher.UIThread.CheckAccess())
                _entry.BackgroundColor = value;
            else
                Dispatcher.UIThread.Invoke(() => _entry.BackgroundColor = value);
        }
    }

    public FontWeight FontWeight
    {
        get;
        set
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);
            if (_entry == null)
                return;

            field = value;

            if (Dispatcher.UIThread.CheckAccess())
                _entry.FontWeight = value;
            else
                Dispatcher.UIThread.Invoke(() => _entry.FontWeight = value);
        }
    }

    public object? Content
    {
        get
        {
            if (_entry == null)
                return null;

            return Dispatcher.UIThread.CheckAccess()
                ? _entry.Content
                : Dispatcher.UIThread.Invoke(() => _entry.Content);
        }
        set
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);
            if (_entry == null)
                return;

            if (Dispatcher.UIThread.CheckAccess())
                _entry.Content = value;
            else
                Dispatcher.UIThread.Invoke(() => _entry.Content = value);
        }
    }

    /// <summary>
    ///  Shows the entry in the status bar.
    /// </summary>
    public void Show()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        if (_entry == null)
            return;

        if (Dispatcher.UIThread.CheckAccess())
            _entry.IsShow = true;
        else
            Dispatcher.UIThread.Invoke(() => _entry.IsShow = true);
    }

    /// <summary>
    ///  Hide the entry in the status bar.
    /// </summary>
    public void Hide()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        if (_entry == null)
            return;

        if (Dispatcher.UIThread.CheckAccess())
            _entry.IsShow = false;
        else
            Dispatcher.UIThread.Invoke(() => _entry.IsShow = false);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_entry == null)
            return;

        if (_isDisposed)
            return;

        _isDisposed = true;

        if (Dispatcher.UIThread.CheckAccess())
            _entry.Dispose();
        else
            Dispatcher.UIThread.Invoke(() => _entry.Dispose());

        _entry = null;
    }
}
