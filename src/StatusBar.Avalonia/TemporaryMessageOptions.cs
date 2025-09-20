using Avalonia.Media;

namespace StatusBar.Avalonia;

/// <summary>
/// Options for configuring a temporary status bar message.
/// Allows customization of message appearance and behavior.
/// </summary>
public class TemporaryMessageOptions
{
    /// <summary>
    /// Alignment of the message in the status bar.
    /// </summary>
    public StatusBarAlignment? Alignment { get; set; }

    /// <summary>
    /// Priority of the message. Higher values indicate higher priority.
    /// </summary>
    public int? Priority { get; set; }

    /// <summary>
    /// Foreground color of the message text.
    /// </summary>
    public IBrush? Color { get; set; }

    /// <summary>
    /// Background color of the message.
    /// </summary>
    public IBrush? BackgroundColor { get; set; }

    /// <summary>
    /// Font weight of the message text.
    /// </summary>
    public FontWeight? FontWeight { get; set; }
}
