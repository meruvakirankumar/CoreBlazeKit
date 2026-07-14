using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Components.Shared;

/// <summary>A linear progress bar with indeterminate, striped, and labelled modes.</summary>
public partial class ProgressBar : ComponentBase
{
    /// <summary>Current value (0 – <see cref="Max"/>).</summary>
    [Parameter] public double Value { get; set; }

    /// <summary>Maximum value (default 100).</summary>
    [Parameter] public double Max { get; set; } = 100;

    /// <summary>Visual variant.</summary>
    [Parameter] public ProgressVariant Variant { get; set; } = ProgressVariant.Primary;

    /// <summary>Size preset.</summary>
    [Parameter] public ProgressSize Size { get; set; } = ProgressSize.Medium;

    /// <summary>Show a percentage / custom label inside the bar.</summary>
    [Parameter] public bool ShowLabel { get; set; }

    /// <summary>Custom label text. Falls back to <c>"{percent}%"</c>.</summary>
    [Parameter] public string? LabelText { get; set; }

    /// <summary>Accessibility label for the bar.</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>Animated diagonal stripes.</summary>
    [Parameter] public bool Striped { get; set; }

    /// <summary>Indeterminate mode — bouncing bar for unknown-duration operations.</summary>
    [Parameter] public bool Indeterminate { get; set; }

    /// <summary>Extra CSS classes.</summary>
    [Parameter] public string? CssClass { get; set; }

    internal double Percent => Max <= 0 ? 0 : Math.Clamp(Value / Max * 100, 0, 100);
    private string VariantClass => Variant.ToString().ToLowerInvariant();
    private string SizeClass => Size.ToString().ToLowerInvariant();
}

/// <summary>Colour variant for <see cref="ProgressBar"/>.</summary>
public enum ProgressVariant { Primary, Success, Warning, Error }

/// <summary>Height preset for <see cref="ProgressBar"/>.</summary>
public enum ProgressSize { Small, Medium, Large }
