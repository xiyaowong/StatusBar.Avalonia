using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using StatusBar.Avalonia.Themes;
using StatusBarDemo.ViewModels;

namespace StatusBarDemo.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    /// <inheritdoc />
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (DataContext is MainViewModel vm)
        {
            vm.StatusBarManager.BindContainer(_StatusBarContainer);
        }
    }

    private void ColorThemeChanged(object? sender, SelectionChangedEventArgs e)
    {
        var box = (ComboBox)sender!;

        if (box.SelectedItem is ComboBoxItem { Content: ColorTheme theme })
        {
            ((App)Application.Current!).StatusBarTheme.ColorTheme = theme;
        }
    }

    private void ThemeChanged(object? sender, SelectionChangedEventArgs e)
    {
        var box = (ComboBox)sender!;

        if (box.SelectedItem is ComboBoxItem { Content: ThemeVariant theme })
        {
            ((App)Application.Current!).RequestedThemeVariant = theme;
        }
    }
}
