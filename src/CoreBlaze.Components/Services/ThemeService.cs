namespace CoreBlaze.Components.Services;

/// <summary>
/// A very small theme service. Consumers can subscribe to <see cref="ThemeChanged"/>
/// to re-render when the theme toggles. Bonus scaffold — expand as needed.
/// </summary>
public sealed class ThemeService
{
    private string _theme = "light";

    /// <summary>Fires whenever the theme changes.</summary>
    public event Action<string>? ThemeChanged;

    /// <summary>Gets the current theme name (default: <c>light</c>).</summary>
    public string CurrentTheme => _theme;

    /// <summary>Sets the current theme and notifies subscribers.</summary>
    public void SetTheme(string theme)
    {
        if (string.IsNullOrWhiteSpace(theme) || _theme == theme) return;
        _theme = theme;
        ThemeChanged?.Invoke(_theme);
    }
}
