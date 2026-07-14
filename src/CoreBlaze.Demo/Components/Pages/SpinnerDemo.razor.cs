using CoreBlaze.Components.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CoreBlaze.Demo.Components.Pages;

public partial class SpinnerDemo : ComponentBase
{
    [Inject] private IJSRuntime JS { get; set; } = default!;

    // Playground state
    private SpinnerSize _size = SpinnerSize.Large;
    private SpinnerVariant _variant = SpinnerVariant.Ring;
    private int _ringCount = 0;          // 0 = auto
    private string _centerIcon = string.Empty;
    private string _centerImage = string.Empty;
    private List<string> _colors = new() { "#4f46e5", "#22d3ee" };
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

    private IReadOnlyList<string>? ResolvedColors =>
        _useColors ? _colors.Where(c => !string.IsNullOrWhiteSpace(c)).ToList() : null;

    private void AddColor()
    {
        _colors.Add("#a855f7");
    }

    private void RemoveColor(int index)
    {
        if (_colors.Count > 1)
            _colors.RemoveAt(index);
    }

    private void UpdateColor(int index, string value)
    {
        if (index >= 0 && index < _colors.Count)
            _colors[index] = value;
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

    // ── Gallery ──────────────────────────────────────────────────────────
    private string? _copiedPreset;

    private sealed record GalleryPreset(
        string Name,
        string Variant,
        string Size,
        string[] Colors,
        int RingCount,
        string CenterIcon,
        string Code,
        RenderFragment Render);

    private List<GalleryPreset> _galleryPresets = default!;

    protected override void OnInitialized()
    {
        _galleryPresets = new List<GalleryPreset>
        {
            Make("Ring — default",    "Ring",     "Large",   Array.Empty<string>(), 0, "",   "<Spinner />",
                b => b.OpenComponent<Spinner>(0), SpinnerVariant.Ring,  SpinnerSize.Large, Array.Empty<string>(), 0, null),

            Make("Ring amber",        "Ring",     "Large",   new[]{"#f59e0b"}, 0, "",
                "<Spinner Colors=\"@(new[]{\\\"#f59e0b\\\"})\" />",
                b => b.OpenComponent<Spinner>(0), SpinnerVariant.Ring, SpinnerSize.Large, new[]{"#f59e0b"}, 0, null),

            Make("Ring XL",           "Ring",     "XLarge",  Array.Empty<string>(), 0, "",
                "<Spinner Size=\"SpinnerSize.XLarge\" />",
                b => b.OpenComponent<Spinner>(0), SpinnerVariant.Ring, SpinnerSize.XLarge, Array.Empty<string>(), 0, null),

            Make("Pulse tri-colour",  "Pulse",    "Large",   new[]{"#4f46e5","#a855f7","#ec4899"}, 3, "",
                "<Spinner Variant=\"SpinnerVariant.Pulse\"\n         Colors=\"@(new[]{\\\"#4f46e5\\\",\\\"#a855f7\\\",\\\"#ec4899\\\"})\" RingCount=\"3\" />",
                b => b.OpenComponent<Spinner>(0), SpinnerVariant.Pulse, SpinnerSize.Large, new[]{"#4f46e5","#a855f7","#ec4899"}, 3, null),

            Make("Pulse ✓",           "Pulse",    "Large",   new[]{"#10b981"}, 0, "✓",
                "<Spinner Variant=\"SpinnerVariant.Pulse\"\n         Colors=\"@(new[]{\\\"#10b981\\\"})\" CenterIcon=\"✓\" />",
                b => b.OpenComponent<Spinner>(0), SpinnerVariant.Pulse, SpinnerSize.Large, new[]{"#10b981"}, 0, "✓"),

            Make("Pulse XXL",         "Pulse",    "XXLarge", new[]{"#8b5cf6","#ec4899"}, 3, "",
                "<Spinner Variant=\"SpinnerVariant.Pulse\"\n         Size=\"SpinnerSize.XXLarge\"\n         Colors=\"@(new[]{\\\"#8b5cf6\\\",\\\"#ec4899\\\"})\" RingCount=\"3\" />",
                b => b.OpenComponent<Spinner>(0), SpinnerVariant.Pulse, SpinnerSize.XXLarge, new[]{"#8b5cf6","#ec4899"}, 3, null),

            Make("Wave ocean",        "Wave",     "Large",   new[]{"#06b6d4","#3b82f6"}, 4, "",
                "<Spinner Variant=\"SpinnerVariant.Wave\"\n         Colors=\"@(new[]{\\\"#06b6d4\\\",\\\"#3b82f6\\\"})\" RingCount=\"4\" />",
                b => b.OpenComponent<Spinner>(0), SpinnerVariant.Wave, SpinnerSize.Large, new[]{"#06b6d4","#3b82f6"}, 4, null),

            Make("Wave 🔥",           "Wave",     "Large",   new[]{"#f59e0b","#f97316"}, 3, "🔥",
                "<Spinner Variant=\"SpinnerVariant.Wave\"\n         Colors=\"@(new[]{\\\"#f59e0b\\\",\\\"#f97316\\\"})\" CenterIcon=\"🔥\" RingCount=\"3\" />",
                b => b.OpenComponent<Spinner>(0), SpinnerVariant.Wave, SpinnerSize.Large, new[]{"#f59e0b","#f97316"}, 3, "🔥"),

            Make("Gradient cool",     "Gradient", "Large",   new[]{"#4f46e5","#22d3ee"}, 0, "",
                "<Spinner Variant=\"SpinnerVariant.Gradient\"\n         Colors=\"@(new[]{\\\"#4f46e5\\\",\\\"#22d3ee\\\"})\" />",
                b => b.OpenComponent<Spinner>(0), SpinnerVariant.Gradient, SpinnerSize.Large, new[]{"#4f46e5","#22d3ee"}, 0, null),

            Make("Gradient warm 🌟",  "Gradient", "Large",   new[]{"#ef4444","#f59e0b"}, 0, "🌟",
                "<Spinner Variant=\"SpinnerVariant.Gradient\"\n         Colors=\"@(new[]{\\\"#ef4444\\\",\\\"#f59e0b\\\"})\" CenterIcon=\"🌟\" />",
                b => b.OpenComponent<Spinner>(0), SpinnerVariant.Gradient, SpinnerSize.Large, new[]{"#ef4444","#f59e0b"}, 0, "🌟"),
        };
    }

    private static GalleryPreset Make(
        string name, string variant, string size, string[] colors, int ringCount, string icon, string code,
        Action<Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder> _ignored,
        SpinnerVariant variantEnum, SpinnerSize sizeEnum, string[] colorArr, int rings, string? centerIcon)
    {
        RenderFragment render = builder =>
        {
            builder.OpenComponent<Spinner>(0);
            builder.AddAttribute(1, "Variant", variantEnum);
            builder.AddAttribute(2, "Size", sizeEnum);
            if (colorArr.Length > 0)
                builder.AddAttribute(3, "Colors", (IReadOnlyList<string>)colorArr);
            if (rings > 0)
                builder.AddAttribute(4, "RingCount", rings);
            if (!string.IsNullOrEmpty(centerIcon))
                builder.AddAttribute(5, "CenterIcon", centerIcon);
            builder.CloseComponent();
        };
        return new GalleryPreset(name, variant, size, colors, ringCount, icon, code, render);
    }

    private async Task CopyPresetCode(GalleryPreset preset)
    {
        await JS.InvokeVoidAsync("CoreBlaze.copy", preset.Code);
        _copiedPreset = preset.Name;
        StateHasChanged();
        await Task.Delay(1800);
        _copiedPreset = null;
    }
}
