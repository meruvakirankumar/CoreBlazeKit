using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace CoreBlaze.Components.Shared;

/// <summary>An iOS-style toggle/switch for boolean settings.</summary>
public partial class Toggle : ComponentBase
{
    /// <summary>The bound boolean value.</summary>
    [Parameter] public bool Value { get; set; }

    /// <summary>Two-way binding companion.</summary>
    [Parameter] public EventCallback<bool> ValueChanged { get; set; }

    /// <summary>Label rendered next to the switch.</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>Text inside the track when on (e.g. "On").</summary>
    [Parameter] public string? OnText { get; set; }

    /// <summary>Text inside the track when off (e.g. "Off").</summary>
    [Parameter] public string? OffText { get; set; }

    /// <summary>Size preset.</summary>
    [Parameter] public ToggleSize Size { get; set; } = ToggleSize.Medium;

    /// <summary>Prevent interaction.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>Extra CSS classes.</summary>
    [Parameter] public string? CssClass { get; set; }

    /// <summary>Fires when the value changes.</summary>
    [Parameter] public EventCallback<bool> OnChange { get; set; }

    private string SizeClass => Size.ToString().ToLowerInvariant();

    private async Task OnChangedAsync(ChangeEventArgs args)
    {
        if (Disabled) return;
        var next = (bool)(args.Value ?? false);
        Value = next;
        await ValueChanged.InvokeAsync(next);
        await OnChange.InvokeAsync(next);
    }
}

/// <summary>Size preset for <see cref="Toggle"/>.</summary>
public enum ToggleSize { Small, Medium, Large }
