using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Components.Shared;

/// <summary>
/// Defines one tab inside a <see cref="Tabs"/> container. Acts as a metadata
/// component — its <see cref="ChildContent"/> is rendered by the parent only
/// when this tab is active.
/// </summary>
public partial class Tab : ComponentBase, IDisposable
{
    /// <summary>The parent <see cref="Tabs"/> container.</summary>
    [CascadingParameter] private Tabs? Parent { get; set; }

    /// <summary>Tab header label.</summary>
    [Parameter, EditorRequired] public string Title { get; set; } = string.Empty;

    /// <summary>Optional icon rendered before the title (any inline markup).</summary>
    [Parameter] public string? Icon { get; set; }

    /// <summary>Content rendered when this tab is active.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Whether the tab is disabled (not selectable).</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (Parent is null)
            throw new InvalidOperationException($"{nameof(Tab)} must be a child of {nameof(Tabs)}.");
        Parent.RegisterTab(this);
    }

    /// <inheritdoc />
    public void Dispose() => Parent?.UnregisterTab(this);
}
