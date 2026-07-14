using CoreBlaze.Components.Shared;
using CoreBlaze.Demo.Services;
using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Demo.Components.Pages;

public partial class DisplayDemo : ComponentBase
{
    [Inject] private PersonRepository Repo { get; set; } = default!;

    // Toggle state
    private bool _notifications = true;
    private bool _darkMode;
    private bool _analytics;

    // Progress
    private double _uploadProgress = 65;
    private bool _uploading;
    private System.Threading.CancellationTokenSource? _cts;

    private async Task StartUploadAsync()
    {
        _uploading = true;
        _uploadProgress = 0;
        _cts = new System.Threading.CancellationTokenSource();
        try
        {
            for (var i = 1; i <= 100; i++)
            {
                await Task.Delay(30, _cts.Token);
                _uploadProgress = i;
                StateHasChanged();
            }
        }
        catch (TaskCanceledException) { }
        finally { _uploading = false; }
    }

    private void StopUpload() { _cts?.Cancel(); _uploading = false; }
}
