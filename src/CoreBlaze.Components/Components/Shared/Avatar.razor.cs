using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Components.Shared;

/// <summary>
/// A user-facing avatar — renders a photo when <see cref="Src"/> is set,
/// or auto-generates coloured initials from <see cref="Name"/>.
/// Accepts a badge slot (<see cref="ChildContent"/>) for online indicators.
/// </summary>
public partial class Avatar : ComponentBase
{
    // Palette of colours cycled by name — keeps the same colour for the same person
    private static readonly string[] Palette =
    {
        "#4f46e5", "#7c3aed", "#db2777", "#dc2626", "#d97706",
        "#16a34a", "#0891b2", "#0284c7", "#9333ea", "#be185d",
    };

    /// <summary>URL of the profile photo.</summary>
    [Parameter] public string? Src { get; set; }

    /// <summary>Full name — used for initials and accessible labels.</summary>
    [Parameter] public string? Name { get; set; }

    /// <summary>Optional emoji or symbol to show instead of initials (e.g. <c>"🤖"</c>).</summary>
    [Parameter] public string? Icon { get; set; }

    /// <summary>Override background colour. When omitted it is derived from <see cref="Name"/>.</summary>
    [Parameter] public string? Color { get; set; }

    /// <summary>Size preset.</summary>
    [Parameter] public AvatarSize Size { get; set; } = AvatarSize.Medium;

    /// <summary>Shape preset.</summary>
    [Parameter] public AvatarShape Shape { get; set; } = AvatarShape.Circle;

    /// <summary>Badge overlay content (e.g. an online indicator dot).</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Extra CSS classes.</summary>
    [Parameter] public string? CssClass { get; set; }

    private string SizeClass  => Size.ToString().ToLowerInvariant();
    private string ShapeClass => Shape.ToString().ToLowerInvariant();
    private bool HasIcon => !string.IsNullOrEmpty(Icon);

    internal string Initials
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Name)) return "?";
            var parts = Name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length >= 2
                ? $"{parts[0][0]}{parts[^1][0]}".ToUpperInvariant()
                : Name[..Math.Min(2, Name.Length)].ToUpperInvariant();
        }
    }

    private string? _colorStyle
    {
        get
        {
            if (!string.IsNullOrEmpty(Src) || HasIcon) return null;
            var bg = !string.IsNullOrEmpty(Color) ? Color
                : string.IsNullOrEmpty(Name) ? Palette[0]
                : Palette[Math.Abs(Name.GetHashCode()) % Palette.Length];
            return $"background-color:{bg}";
        }
    }
}

/// <summary>Size preset for <see cref="Avatar"/>.</summary>
public enum AvatarSize { XSmall, Small, Medium, Large, XLarge }

/// <summary>Shape preset for <see cref="Avatar"/>.</summary>
public enum AvatarShape { Circle, Rounded }
