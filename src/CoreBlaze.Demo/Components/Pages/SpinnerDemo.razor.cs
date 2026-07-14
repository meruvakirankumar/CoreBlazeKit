using CoreBlaze.Components.Shared;
using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Demo.Components.Pages;

public partial class SpinnerDemo : ComponentBase
{
    // Playground state
    private SpinnerSize _size = SpinnerSize.Large;
    private SpinnerVariant _variant = SpinnerVariant.Ring;
    private int _ringCount = 0;          // 0 = auto
    private string _centerIcon = string.Empty;
    private string _centerImage = string.Empty;
    private string _color1 = "#4f46e5";  // indigo
    private string _color2 = "#22d3ee";  // cyan
    private string _color3 = string.Empty;
    private bool _useColors = false;
    private string _label = string.Empty;

    // Loading overlay state
    private bool _isLoading;

    private async Task SimulateLoadAsync()
    {
        _isLoading = true;
        StateHasChanged();
        await Task.Delay(2000);
        _isLoading = false;
    }

    private IReadOnlyList<string>? ResolvedColors
    {
        get
        {
            if (!_useColors) return null;
            var list = new List<string> { _color1 };
            if (!string.IsNullOrWhiteSpace(_color2)) list.Add(_color2);
            if (!string.IsNullOrWhiteSpace(_color3)) list.Add(_color3);
            return list;
        }
    }

    private int ResolvedRingCount => _ringCount > 0 ? _ringCount : _variant switch
    {
        SpinnerVariant.Pulse => 3,
        SpinnerVariant.Wave  => 3,
        _                    => 1,
    };

    private const string _code = """
        @using CoreBlaze.Components.Shared

        <!-- Classic ring -->
        <Spinner />

        <!-- Custom size + label -->
        <Spinner Size="SpinnerSize.Large" Label="Fetching data…" />

        <!-- Pulse (radar) with custom colours -->
        <Spinner Variant="SpinnerVariant.Pulse"
                 Size="SpinnerSize.Large"
                 Colors="@(new[] { "#4f46e5", "#a855f7", "#ec4899" })"
                 RingCount="3" />

        <!-- Wave with an icon centre -->
        <Spinner Variant="SpinnerVariant.Wave"
                 Size="SpinnerSize.Large"
                 Colors="@(new[] { "#06b6d4", "#3b82f6" })"
                 CenterIcon="🔵"
                 Label="Loading…" />

        <!-- Gradient with a custom image in the centre -->
        <Spinner Variant="SpinnerVariant.Gradient"
                 Size="SpinnerSize.Large"
                 Colors="@(new[] { "#f59e0b", "#ef4444" })"
                 CenterImage="/img/logo.png" />

        <!-- Inside a button -->
        <button class="btn btn-primary" disabled="@_saving">
            @if (_saving) { <Spinner Size="SpinnerSize.Small" /> }
            @(_saving ? "Saving…" : "Save")
        </button>

        <!-- LoadingOverlay wraps any content -->
        <LoadingOverlay IsLoading="_busy" LoadingText="Please wait…">
            <div class="card">…your content…</div>
        </LoadingOverlay>
        """;
}
