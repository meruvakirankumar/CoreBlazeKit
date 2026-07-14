using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Components.Shared;

/// <summary>A horizontal (or vertical) line separator with an optional centre label.</summary>
public partial class Divider : ComponentBase
{
    /// <summary>Optional text rendered across the divider line.</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>Render as a vertical divider instead of horizontal.</summary>
    [Parameter] public bool Vertical { get; set; }

    /// <summary>Extra CSS classes on the root element.</summary>
    [Parameter] public string? CssClass { get; set; }
}
