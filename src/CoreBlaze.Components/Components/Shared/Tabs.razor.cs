using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Components.Shared;

/// <summary>
/// Tab container. Declare <c>&lt;Tab Title="…"&gt;</c> children directly inside.
/// Active tab is two-way bindable via <c>@bind-ActiveTitle</c>, or controlled via the
/// built-in header.
/// </summary>
public partial class Tabs : ComponentBase
{
    private readonly List<Tab> _tabs = new();
    private Tab? _active;

    /// <summary>The child <c>&lt;Tab&gt;</c> definitions.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Extra CSS classes on the root element.</summary>
    [Parameter] public string? CssClass { get; set; }

    /// <summary>The title of the currently active tab.</summary>
    [Parameter] public string? ActiveTitle { get; set; }

    /// <summary>Two-way binding companion for <see cref="ActiveTitle"/>.</summary>
    [Parameter] public EventCallback<string?> ActiveTitleChanged { get; set; }

    /// <summary>Fires whenever the active tab changes.</summary>
    [Parameter] public EventCallback<Tab> OnTabChanged { get; set; }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (ActiveTitle is not null)
        {
            var match = _tabs.FirstOrDefault(t => t.Title == ActiveTitle && !t.Disabled);
            if (match is not null) _active = match;
        }
    }

    internal void RegisterTab(Tab tab)
    {
        if (!_tabs.Contains(tab))
        {
            _tabs.Add(tab);
            // Activate the first non-disabled tab automatically.
            if (_active is null && !tab.Disabled)
                _active = tab;
            StateHasChanged();
        }
    }

    internal void UnregisterTab(Tab tab)
    {
        _tabs.Remove(tab);
        if (ReferenceEquals(_active, tab))
            _active = _tabs.FirstOrDefault(t => !t.Disabled);
        StateHasChanged();
    }

    private async Task ActivateTab(Tab tab)
    {
        if (tab.Disabled || ReferenceEquals(tab, _active)) return;
        _active = tab;
        await ActiveTitleChanged.InvokeAsync(tab.Title);
        await OnTabChanged.InvokeAsync(tab);
    }

    /// <summary>Programmatically activate the tab whose <see cref="Tab.Title"/> matches.</summary>
    public async Task SetActiveAsync(string title)
    {
        var tab = _tabs.FirstOrDefault(t => t.Title == title && !t.Disabled);
        if (tab is not null) await ActivateTab(tab);
    }
}
