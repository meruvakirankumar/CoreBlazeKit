using CoreBlaze.Components.Services;
using CoreBlaze.Components.Shared;
using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Demo.Components.Pages;

public partial class FeedbackDemo : ComponentBase
{
    [Inject] private ToastService Toasts { get; set; } = default!;

    private bool _modalOpen;
    private ModalSize _size = ModalSize.Medium;

    private string GetTitle() => $"Modal — {_size} size";

    private void OnConfirm()
    {
        _modalOpen = false;
        Toasts.Success("You confirmed the action.", title: "Confirmed");
    }

    private const string _code = """
        @using CoreBlaze.Components.Models
        @using CoreBlaze.Components.Services
        @using CoreBlaze.Components.Shared

        @inject ToastService Toasts

        <!-- Toasts — fire from anywhere via ToastService -->
        <button @onclick="@(() => Toasts.Info("Just so you know.", "Tip"))">Info</button>
        <button @onclick="@(() => Toasts.Success("Your changes were saved.", "Saved"))">Success</button>
        <button @onclick="@(() => Toasts.Warning("You have unsaved changes.", "Careful"))">Warning</button>
        <button @onclick="@(() => Toasts.Error("Failed to save the row.", "Save failed"))">Error</button>

        <!-- Sticky toast (no auto-dismiss) — pass Duration = TimeSpan.Zero -->
        <button @onclick="@(() => Toasts.Info("Stays until dismissed.", duration: TimeSpan.Zero))">Sticky</button>

        <!-- Clear the whole queue -->
        <button @onclick="Toasts.Clear">Clear all</button>

        <!-- Anchor the Toaster once in your app's main layout (outside routed area) -->
        <Toaster Position="ToastPosition.TopRight" />

        <!-- Modal — two-way bindable via @bind-IsOpen; 4 size presets -->
        <button @onclick="@(() => { _size = ModalSize.Small;      _modalOpen = true; })">Small</button>
        <button @onclick="@(() => { _size = ModalSize.Medium;     _modalOpen = true; })">Medium</button>
        <button @onclick="@(() => { _size = ModalSize.Large;      _modalOpen = true; })">Large</button>
        <button @onclick="@(() => { _size = ModalSize.ExtraLarge; _modalOpen = true; })">XL</button>

        <Modal @bind-IsOpen="_modalOpen" Title="Confirm delete" Size="_size">
            <ChildContent>
                Are you sure you want to delete this row?
            </ChildContent>
            <Footer>
                <button class="btn btn-outline-secondary"
                        @onclick="@(() => _modalOpen = false)">Cancel</button>
                <button class="btn btn-danger" @onclick="OnConfirmDelete">Delete</button>
            </Footer>
        </Modal>

        @code {
            bool _modalOpen;
            ModalSize _size = ModalSize.Medium;

            void OnConfirmDelete()
            {
                _modalOpen = false;
                Toasts.Warning("Row deleted.", "Deleted");
            }
        }
        """;
}
