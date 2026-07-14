using CoreBlaze.Components.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace CoreBlaze.Components.Inputs;

/// <summary>
/// A drag-and-drop-enabled file input. Bound value is the currently-selected list of
/// <see cref="IBrowserFile"/> — the consumer decides whether to stream / upload them.
/// </summary>
public partial class FileUpload : BaseInputComponent<IReadOnlyList<IBrowserFile>>
{
    /// <summary>Allow multiple files.</summary>
    [Parameter] public bool Multiple { get; set; } = true;

    /// <summary>Optional <c>accept</c> filter (e.g. <c>"image/*,.pdf"</c>).</summary>
    [Parameter] public string? Accept { get; set; }

    /// <summary>Fires after files have been selected (or dropped).</summary>
    [Parameter] public EventCallback<IReadOnlyList<IBrowserFile>> OnFilesSelected { get; set; }

    /// <inheritdoc />
    protected override string BaseCssClass => "cb-fileupload";

    private bool IsDragOver { get; set; }

    private async Task OnFilesSelectedAsync(InputFileChangeEventArgs e)
    {
        var files = Multiple
            ? e.GetMultipleFiles(maximumFileCount: 100)
            : new List<IBrowserFile> { e.File };
        await SetValueAsync(files);
        await OnFilesSelected.InvokeAsync(files);
    }
}
