using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CoreBlaze.Demo.Components.Shared;

public partial class CodeSample : ComponentBase, IAsyncDisposable
{
    private bool _copied;
    private bool _expanded = true;
    private CancellationTokenSource? _resetCts;

    /// <summary>The code snippet to display.</summary>
    [Parameter, EditorRequired] public string Code { get; set; } = string.Empty;

    /// <summary>Language label shown in the toolbar (default <c>razor</c>).</summary>
    [Parameter] public string Language { get; set; } = "razor";

    private async Task CopyAsync()
    {
        try
        {
            await JS.InvokeVoidAsync("CoreBlaze.copy", Code);
        }
        catch
        {
            // Clipboard permission blocked — ignore, no user-visible failure needed.
        }

        _copied = true;
        StateHasChanged();

        _resetCts?.Cancel();
        _resetCts = new CancellationTokenSource();
        var ct = _resetCts.Token;
        _ = Task.Delay(1500, ct).ContinueWith(t =>
        {
            if (t.IsCanceled) return;
            _copied = false;
            InvokeAsync(StateHasChanged);
        }, TaskScheduler.Default);
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        _resetCts?.Cancel();
        _resetCts?.Dispose();
        return ValueTask.CompletedTask;
    }
}
