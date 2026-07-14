using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Components.Grid;

/// <summary>Pager UI for <see cref="DataGrid{T}"/>.</summary>
public partial class GridPager : ComponentBase
{
    /// <summary>The 1-based current page.</summary>
    [Parameter] public int Page { get; set; } = 1;

    /// <summary>Number of items per page.</summary>
    [Parameter] public int PageSize { get; set; } = 10;

    /// <summary>Total number of items across all pages.</summary>
    [Parameter] public int TotalItems { get; set; }

    /// <summary>Selectable page-size values.</summary>
    [Parameter] public IReadOnlyList<int> PageSizeOptions { get; set; } = new[] { 5, 10, 20, 50 };

    /// <summary>Raised when the user requests a new page.</summary>
    [Parameter] public EventCallback<int> PageChanged { get; set; }

    /// <summary>Raised when the user picks a different page size.</summary>
    [Parameter] public EventCallback<int> PageSizeChanged { get; set; }

    private int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalItems / (double)Math.Max(PageSize, 1)));

    private Task GoTo(int page)
    {
        var target = Math.Clamp(page, 1, TotalPages);
        if (target == Page) return Task.CompletedTask;
        return PageChanged.InvokeAsync(target);
    }

    private Task OnPageSizeChanged(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out var size) && size > 0)
            return PageSizeChanged.InvokeAsync(size);
        return Task.CompletedTask;
    }
}
