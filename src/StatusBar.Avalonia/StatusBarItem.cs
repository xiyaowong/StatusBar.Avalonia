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
    private StatusBarItemView? _itemView;

    private bool _isDisposed;

    internal StatusBarItem(StatusBarItemView itemView)
    {
        Id = itemView.Id;
        Alignment = itemView.Alignment;
        Priority = itemView.Priority;
        Text = itemView.Text;
        ToolTip = itemView.ToolTip;
        Click = itemView.Click;
        Color = itemView.Color;
        BackgroundColor = itemView.BackgroundColor;
        FontWeight = itemView.FontWeight;

        _itemView = itemView;
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

            if (_itemView == null)
                return;

            field = value;

            if (Dispatcher.UIThread.CheckAccess())
                _itemView.Text = value;
            else
                Dispatcher.UIThread.Invoke(() => _itemView.Text = value);
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

            if (_itemView == null)
                return;

            field = value;

            if (Dispatcher.UIThread.CheckAccess())
                _itemView.ToolTip = value;
            else
                Dispatcher.UIThread.Invoke(() => _itemView.ToolTip = value);
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

            if (_itemView == null)
                return;

            field = value;

            if (Dispatcher.UIThread.CheckAccess())
                _itemView.Click = value;
            else
                Dispatcher.UIThread.Invoke(() => _itemView.Click = value);
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

            if (_itemView == null)
                return;

            field = value;

            if (Dispatcher.UIThread.CheckAccess())
                _itemView.Color = value;
            else
                Dispatcher.UIThread.Invoke(() => _itemView.Color = value);
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

            if (_itemView == null)
                return;

            field = value;

            if (Dispatcher.UIThread.CheckAccess())
                _itemView.BackgroundColor = value;
            else
                Dispatcher.UIThread.Invoke(() => _itemView.BackgroundColor = value);
        }
    }

    public FontWeight FontWeight
    {
        get;
        set
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);
            if (_itemView == null)
                return;

            field = value;

            if (Dispatcher.UIThread.CheckAccess())
                _itemView.FontWeight = value;
            else
                Dispatcher.UIThread.Invoke(() => _itemView.FontWeight = value);
        }
    }

    public object? Content
    {
        get
        {
            if (_itemView == null)
                return null;

            return Dispatcher.UIThread.CheckAccess()
                ? _itemView.Content
                : Dispatcher.UIThread.Invoke(() => _itemView.Content);
        }
        set
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);
            if (_itemView == null)
                return;

            if (Dispatcher.UIThread.CheckAccess())
                _itemView.Content = value;
            else
                Dispatcher.UIThread.Invoke(() => _itemView.Content = value);
        }
    }

    /// <summary>
    ///  Shows the entry in the status bar.
    /// </summary>
    public void Show()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        if (_itemView == null)
            return;

        if (Dispatcher.UIThread.CheckAccess())
            _itemView.IsShow = true;
        else
            Dispatcher.UIThread.Invoke(() => _itemView.IsShow = true);
    }

    /// <summary>
    ///  Hide the entry in the status bar.
    /// </summary>
    public void Hide()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        if (_itemView == null)
            return;

        if (Dispatcher.UIThread.CheckAccess())
            _itemView.IsShow = false;
        else
            Dispatcher.UIThread.Invoke(() => _itemView.IsShow = false);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_itemView == null)
            return;

        if (_isDisposed)
            return;

        _isDisposed = true;

        if (Dispatcher.UIThread.CheckAccess())
            _itemView.Dispose();
        else
            Dispatcher.UIThread.Invoke(() => _itemView.Dispose());

        _itemView = null;
    }
}
