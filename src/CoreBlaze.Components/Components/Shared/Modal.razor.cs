using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Components.Shared;

/// <summary>
/// A general-purpose modal dialog with a backdrop, header, body, and optional footer.
/// Two-way bindable via <c>@bind-IsOpen</c>.
/// </summary>
public partial class Modal : ComponentBase
{
    private readonly string _titleId = $"cb-modal-title-{Guid.NewGuid():N}";

    /// <summary>Whether the modal is currently visible.</summary>
    [Parameter] public bool IsOpen { get; set; }

    /// <summary>Two-way binding companion for <see cref="IsOpen"/>.</summary>
    [Parameter] public EventCallback<bool> IsOpenChanged { get; set; }

    /// <summary>Header title. When null and <see cref="ShowCloseButton"/> is false, the header is not rendered.</summary>
    [Parameter] public string? Title { get; set; }

    /// <summary>The modal body — put your form / content here.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Optional footer content (usually action buttons).</summary>
    [Parameter] public RenderFragment? Footer { get; set; }

    /// <summary>Modal width preset.</summary>
    [Parameter] public ModalSize Size { get; set; } = ModalSize.Medium;

    /// <summary>Whether clicking the backdrop closes the modal (default true).</summary>
    [Parameter] public bool CloseOnBackdropClick { get; set; } = true;

    /// <summary>Whether the header shows an "×" close button (default true).</summary>
    [Parameter] public bool ShowCloseButton { get; set; } = true;

    /// <summary>Raised when the user closes the modal (via backdrop, close button, or programmatically).</summary>
    [Parameter] public EventCallback OnClose { get; set; }

    /// <summary>Programmatically close the modal.</summary>
    public async Task CloseAsync()
    {
        if (!IsOpen) return;
        IsOpen = false;
        await IsOpenChanged.InvokeAsync(false);
        await OnClose.InvokeAsync();
    }

    private Task OnBackdropClickAsync()
        => CloseOnBackdropClick ? CloseAsync() : Task.CompletedTask;
}
