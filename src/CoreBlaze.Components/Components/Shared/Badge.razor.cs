using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Components.Shared;

/// <summary>
/// A small status indicator. Renders standalone as a pill or chip,
/// or anchored in the corner of any wrapped element (set <see cref="ChildContent"/>).
/// </summary>
public partial class Badge : ComponentBase
{
    /// <summary>Numeric count. Values above <see cref="Max"/> render as <c>Max+</c>.</summary>
    [Parameter] public int? Count { get; set; }

    /// <summary>Maximum count before truncation (default 99).</summary>
    [Parameter] public int Max { get; set; } = 99;

    /// <summary>Custom text — takes precedence over <see cref="Count"/>.</summary>
    [Parameter] public string? Text { get; set; }

    /// <summary>Render as a small dot (no text). Useful for unread / online indicators.</summary>
    [Parameter] public bool Dot { get; set; }

    /// <summary>Visual variant.</summary>
    [Parameter] public BadgeVariant Variant { get; set; } = BadgeVariant.Primary;

    /// <summary>Extra CSS classes on the badge span.</summary>
    [Parameter] public string? CssClass { get; set; }

    /// <summary>Extra CSS classes on the anchor wrapper (when <see cref="ChildContent"/> is set).</summary>
    [Parameter] public string? AnchorCssClass { get; set; }

    /// <summary>When set the badge anchors to the top-right of this content.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private string VariantClass => Variant.ToString().ToLowerInvariant();

    private string? BadgeText
    {
        get
        {
            if (Dot) return null;
            if (Text is not null) return Text;
            if (Count is null) return null;
            return Count > Max ? $"{Max}+" : Count.ToString();
        }
    }
}

/// <summary>Visual colour variant for <see cref="Badge"/>.</summary>
public enum BadgeVariant
{
    Primary, Success, Warning, Error, Neutral
}
