using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Components.Shared;

/// <summary>Renders zero or more validation messages using a small, unopinionated markup.</summary>
public partial class ValidationMessageDisplay : ComponentBase
{
    /// <summary>The messages to render.</summary>
    [Parameter] public IEnumerable<string>? Messages { get; set; }
}
