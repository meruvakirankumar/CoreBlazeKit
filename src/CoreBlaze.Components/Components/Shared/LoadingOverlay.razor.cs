using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Components.Shared;

/// <summary>
/// Wraps any content in a relative-positioned container and overlays a centred
/// <see cref="Spinner"/> when <see cref="IsLoading"/> is true.
/// </summary>
public partial class LoadingOverlay : ComponentBase
{
    /// <summary>The content to render inside the overlay container.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Whether the loading curtain is visible.</summary>
    [Parameter] public bool IsLoading { get; set; }

    /// <summary>Optional label shown below the spinner ring.</summary>
    [Parameter] public string? LoadingText { get; set; }

    /// <summary>Spinner size used in the overlay.</summary>
    [Parameter] public SpinnerSize SpinnerSize { get; set; } = SpinnerSize.Large;
}
