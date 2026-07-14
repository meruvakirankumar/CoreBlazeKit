using CoreBlaze.Components.Models;
using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Components.Shared;

/// <summary>
/// Anchors the toast queue somewhere in the viewport. Drop <c>&lt;Toaster /&gt;</c>
/// once in your app's main layout (typically at the bottom, outside the routed area).
/// </summary>
public partial class Toaster : ComponentBase, IDisposable
{
    private IReadOnlyList<Toast> _toasts = Array.Empty<Toast>();

    /// <summary>Where the toast stack anchors within the viewport.</summary>
    [Parameter] public ToastPosition Position { get; set; } = ToastPosition.TopRight;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        ToastService.Changed += OnChanged;
        _toasts = ToastService.Toasts;
    }

    private void OnChanged()
    {
        _toasts = ToastService.Toasts;
        InvokeAsync(StateHasChanged);
    }

    private static string IconFor(ToastVariant v) => v switch
    {
        ToastVariant.Success => "✓",
        ToastVariant.Warning => "⚠",
        ToastVariant.Error   => "✕",
        _ => "ⓘ",
    };

    /// <inheritdoc />
    public void Dispose()
    {
        ToastService.Changed -= OnChanged;
    }
}
