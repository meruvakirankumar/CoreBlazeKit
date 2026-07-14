using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace CoreBlaze.Demo.Components.Pages.Inputs;

public partial class FileUploadDemo : ComponentBase
{
    private IReadOnlyList<IBrowserFile>? _files;
    private IReadOnlyList<IBrowserFile>? _avatar;

    private void OnFilesSelected(IReadOnlyList<IBrowserFile> files)
    {
        // Real apps would open a stream via file.OpenReadStream(maxAllowedSize) and process it.
    }

    private static string FormatSize(long bytes)
    {
        string[] units = { "B", "KB", "MB", "GB" };
        double size = bytes;
        var i = 0;
        while (size >= 1024 && i < units.Length - 1) { size /= 1024; i++; }
        return $"{size:0.##} {units[i]}";
    }

    private const string _code = """
        @using CoreBlaze.Components.Inputs
        @using Microsoft.AspNetCore.Components.Forms

        <!-- Playground — multiple files, mixed accept filter -->
        <FileUpload @bind-Value="_files"
                    Label="Attachments"
                    Placeholder="Drop images or documents here"
                    Accept=".pdf,.png,.jpg,.jpeg,.txt"
                    Multiple="true"
                    OnFilesSelected="OnFilesSelected" />

        <!-- Single-file variant — avatar picker -->
        <FileUpload @bind-Value="_avatar"
                    Label="Avatar"
                    Placeholder="Drop an image"
                    Accept="image/*"
                    Multiple="false" />

        <!-- Disabled — uploads locked -->
        <FileUpload Label="Disabled example"
                    Placeholder="Uploads are disabled"
                    Disabled="true" />

        @code {
            IReadOnlyList<IBrowserFile>? _files;
            IReadOnlyList<IBrowserFile>? _avatar;

            void OnFilesSelected(IReadOnlyList<IBrowserFile> files)
            {
                foreach (var f in files)
                {
                    using var stream = f.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024);
                    // … read / upload / persist
                }
            }
        }
        """;
}
