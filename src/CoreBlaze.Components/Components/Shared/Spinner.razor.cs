using System.Text;
using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Components.Shared;

/// <summary>
/// A customisable animated loading indicator. Supports four animation
/// variants, optional centre content (image / icon / arbitrary markup),
/// per-ring colour overrides, and three size presets.
/// </summary>
public partial class Spinner : ComponentBase
{
    // ----- Core ----------------------------------------------------------------

    /// <summary>Size preset.</summary>
    [Parameter] public SpinnerSize Size { get; set; } = SpinnerSize.Medium;

    /// <summary>Animation style.</summary>
    [Parameter] public SpinnerVariant Variant { get; set; } = SpinnerVariant.Ring;

    /// <summary>Text rendered below the animation.</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>Extra CSS classes on the root element.</summary>
    [Parameter] public string? CssClass { get; set; }

    // ----- Colour --------------------------------------------------------------

    /// <summary>
    /// Ring colours. When empty the component uses the CSS variable
    /// <c>var(--cb-primary)</c> for everything.
    /// <list type="bullet">
    ///   <item><b>Ring</b> — uses <c>Colors[0]</c> for the ring colour.</item>
    ///   <item><b>Pulse / Wave</b> — one colour per ring (cycles if fewer colours than rings).</item>
    ///   <item><b>Gradient</b> — up to the first two colours form the conic gradient.</item>
    /// </list>
    /// </summary>
    [Parameter] public IReadOnlyList<string>? Colors { get; set; }

    // ----- Centre content ------------------------------------------------------

    /// <summary>URL of an image rendered in the centre of the animation.</summary>
    [Parameter] public string? CenterImage { get; set; }

    /// <summary>Text, emoji, or symbol rendered in the centre (e.g. <c>"🔒"</c>, <c>"✓"</c>).</summary>
    [Parameter] public string? CenterIcon { get; set; }

    /// <summary>Arbitrary markup rendered in the centre (takes precedence over image/icon).</summary>
    [Parameter] public RenderFragment? CenterContent { get; set; }

    // ----- Advanced ------------------------------------------------------------

    /// <summary>
    /// Number of concentric rings. 0 = auto (1 for Ring/Gradient, 3 for Pulse/Wave).
    /// </summary>
    [Parameter] public int RingCount { get; set; }

    // ----- Computed properties -------------------------------------------------

    private string SizeClass => Size.ToString().ToLowerInvariant();
    private string VariantClass => Variant.ToString().ToLowerInvariant();

    private int ActualRingCount => RingCount > 0 ? RingCount : Variant switch
    {
        SpinnerVariant.Pulse => 3,
        SpinnerVariant.Wave  => 3,
        _                    => 1,
    };

    private bool HasCenter => CenterContent is not null
        || !string.IsNullOrEmpty(CenterImage)
        || !string.IsNullOrEmpty(CenterIcon);

    /// <summary>CSS custom properties that convey the size to the isolated stylesheet.</summary>
    private string? _sizeVars => Size switch
    {
        SpinnerSize.Small  => "--cb-spin-size:1rem;   --cb-spin-border:2px",
        SpinnerSize.Large  => "--cb-spin-size:3rem;   --cb-spin-border:4px",
        _                  => "--cb-spin-size:2rem;   --cb-spin-border:3px",
    };

    private string? GetRingStyle(int index)
    {
        if (Colors is null || Colors.Count == 0) return null;
        return $"color:{Colors[index % Colors.Count]}";
    }

    private string? GetGradientStyle()
    {
        if (Colors is null || Colors.Count == 0) return null;
        var c1 = Colors[0];
        var c2 = Colors.Count > 1 ? Colors[1] : Colors[0];
        return $"--cb-spin-g1:{c1};--cb-spin-g2:{c2}";
    }
}
