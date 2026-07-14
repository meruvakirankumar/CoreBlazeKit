using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Components.Shared;

/// <summary>
/// An inline alert/banner for contextual feedback — info, success, warning, or error.
/// Unlike <c>Toast</c> it stays in the document flow. Supports a dismiss button.
/// </summary>
public partial class Alert : ComponentBase
{
    private bool _dismissed;

    /// <summary>Visual variant.</summary>
    [Parameter] public AlertVariant Variant { get; set; } = AlertVariant.Info;

    /// <summary>Optional bold title above the body text.</summary>
    [Parameter] public string? Title { get; set; }

    /// <summary>Body content.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Show a dismiss button that hides the alert.</summary>
    [Parameter] public bool Dismissible { get; set; }

    /// <summary>Extra CSS classes.</summary>
    [Parameter] public string? CssClass { get; set; }

    /// <summary>Fires when the alert is dismissed.</summary>
    [Parameter] public EventCallback OnDismissed { get; set; }

    private string VariantClass => Variant.ToString().ToLowerInvariant();

    private string IconChar => Variant switch
    {
        AlertVariant.Success => "✓",
        AlertVariant.Warning => "⚠",
        AlertVariant.Error   => "✕",
        _                    => "ⓘ",
    };

    private async Task Dismiss()
    {
        _dismissed = true;
        await OnDismissed.InvokeAsync();
    }
}

/// <summary>Colour/intent variant for <see cref="Alert"/>.</summary>
public enum AlertVariant { Info, Success, Warning, Error }
