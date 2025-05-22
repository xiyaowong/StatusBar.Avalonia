using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace StatusBar.Avalonia.Themes;

public enum ColorTheme
{
    /// <summary>
    /// Dark: DarkPlus - Light: LightPlus
    /// </summary>
    DarkPlus,

    /// <summary>
    /// Dark: OneDark - Light: OneLight
    /// </summary>
    OneDark,

    /// <summary>
    /// Dark: Github Dark Default - Light: Github Light Default
    /// </summary>
    GithubDefault,
}

public class StatusBarTheme : Styles, IResourceNode
{
    /// <summary>
    /// Defines the <see cref="ColorTheme"/> avalonia property.
    /// </summary>
    public static readonly DirectProperty<StatusBarTheme, ColorTheme> ColorThemeProperty =
        AvaloniaProperty.RegisterDirect<StatusBarTheme, ColorTheme>(
            nameof(ColorTheme),
            o => o.ColorTheme,
            (o, v) => o.ColorTheme = v
        );

    /// <summary>
    /// Color theme of the status bar.
    /// </summary>
    public ColorTheme ColorTheme
    {
        get;
        set => SetAndRaise(ColorThemeProperty, ref field, value);
    }

    private readonly ResourceDictionary _darkPlusTheme;
    private readonly ResourceDictionary _oneDarkTheme;
    private readonly ResourceDictionary _githubDefaultTheme;

    public StatusBarTheme()
    {
        AvaloniaXamlLoader.Load(this);

        _darkPlusTheme = (ResourceDictionary)GetAndRemove(ColorTheme.DarkPlus);
        _oneDarkTheme = (ResourceDictionary)GetAndRemove(ColorTheme.OneDark);
        _githubDefaultTheme = (ResourceDictionary)GetAndRemove(ColorTheme.GithubDefault);

        return;

        object GetAndRemove(object key)
        {
            var val = Resources[key] ?? throw new KeyNotFoundException($"Key {key} was not found in the resources");
            Resources.Remove(key);
            return val;
        }
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ColorThemeProperty)
        {
            Owner?.NotifyHostedResourcesChanged(ResourcesChangedEventArgs.Empty);
        }
    }

    /// <inheritdoc />
    bool IResourceNode.TryGetResource(object key, ThemeVariant? theme, out object? value)
    {
        var themeDict = ColorTheme switch
        {
            ColorTheme.DarkPlus => _darkPlusTheme,
            ColorTheme.OneDark => _oneDarkTheme,
            ColorTheme.GithubDefault => _githubDefaultTheme,
            _ => null,
        };

        if (themeDict is not null && themeDict.TryGetResource(key, theme, out value))
        {
            return true;
        }

        return base.TryGetResource(key, theme, out value);
    }
}
