using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Components.Grid;

/// <summary>
/// A lightweight read-only data table with optional sorting, striped rows,
/// hover highlight, and a custom cell / header template slot.
/// Use <see cref="DataGrid{T}"/> when you need editing, filtering, paging, or export.
/// </summary>
/// <typeparam name="T">Row item type.</typeparam>
public partial class SimpleGrid<T> : ComponentBase where T : class
{
    private readonly List<SimpleGridColumn<T>> _columns = new();
    private List<T> _sorted = new();
    private bool _ready;
    private string? _sortField;
    private bool _sortAsc = true;

    // ── Parameters ──────────────────────────────────────────────────────────

    /// <summary>The data to display.</summary>
    [Parameter, EditorRequired] public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

    /// <summary>Column definitions — place <see cref="SimpleGridColumn{T}"/> children here.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Optional title shown in the toolbar above the table.</summary>
    [Parameter] public string? Title { get; set; }

    /// <summary>Alternating row background colour.</summary>
    [Parameter] public bool Striped { get; set; }

    /// <summary>Cell borders.</summary>
    [Parameter] public bool Bordered { get; set; }

    /// <summary>Row highlight on hover.</summary>
    [Parameter] public bool Hoverable { get; set; } = true;

    /// <summary>Compact row padding.</summary>
    [Parameter] public bool Dense { get; set; }

    /// <summary>Show row count in the footer bar.</summary>
    [Parameter] public bool ShowFooter { get; set; } = true;

    /// <summary>Message displayed when <see cref="Items"/> is empty.</summary>
    [Parameter] public string? EmptyMessage { get; set; }

    /// <summary>Custom empty-state template (overrides <see cref="EmptyMessage"/>).</summary>
    [Parameter] public RenderFragment? EmptyContent { get; set; }

    /// <summary>Extra CSS class on the root wrapper.</summary>
    [Parameter] public string? CssClass { get; set; }

    /// <summary>Per-row CSS class selector.</summary>
    [Parameter] public Func<T, string?>? RowCssClass { get; set; }

    /// <summary>Raised when a row is clicked.</summary>
    [Parameter] public EventCallback<T> OnRowClick { get; set; }

    // ── Column registration ──────────────────────────────────────────────────

    internal void AddColumn(SimpleGridColumn<T> col)
    {
        if (!_columns.Contains(col))
        {
            _columns.Add(col);
            _ready = true;
            ApplySort();
            StateHasChanged();
        }
    }

    internal void RemoveColumn(SimpleGridColumn<T> col)
    {
        _columns.Remove(col);
        StateHasChanged();
    }

    // ── Lifecycle ────────────────────────────────────────────────────────────

    protected override void OnParametersSet()
    {
        ApplySort();
    }

    // ── Sort ─────────────────────────────────────────────────────────────────

    private void ToggleSort(SimpleGridColumn<T> col)
    {
        if (!col.Sortable || col.Field is null) return;

        if (_sortField == col.Field)
            _sortAsc = !_sortAsc;
        else
        {
            _sortField = col.Field;
            _sortAsc = true;
        }
        ApplySort();
    }

    private void ApplySort()
    {
        var source = Items ?? Enumerable.Empty<T>();

        if (_sortField is not null)
        {
            var accessor = CoreBlaze.Components.Utilities.PropertyAccessor.TryFor(typeof(T), _sortField);
            if (accessor is not null)
            {
                _sorted = _sortAsc
                    ? source.OrderBy(x => accessor.Value.Getter(x!)).ToList()
                    : source.OrderByDescending(x => accessor.Value.Getter(x!)).ToList();
            }
            else
            {
                _sorted = source.ToList();
            }
        }
        else
        {
            _sorted = source.ToList();
        }
    }

    // ── Rendering helpers ─────────────────────────────────────────────────────

    private IReadOnlyList<SimpleGridColumn<T>> _visibleCols =>
        _columns.Where(c => c.Visible).ToList();

    private string SortIconClass(SimpleGridColumn<T> col)
    {
        if (_sortField != col.Field) return "cb-sg__sort-icon--none";
        return _sortAsc ? "cb-sg__sort-icon--asc" : "cb-sg__sort-icon--desc";
    }

    private static string AlignClass(SimpleGridColumn<T> col) => col.Align switch
    {
        GridAlign.Center => "cb-sg--center",
        GridAlign.Right  => "cb-sg--right",
        _                => string.Empty,
    };

    private static string GetValue(SimpleGridColumn<T> col, T item)
    {
        if (col.Field is null) return string.Empty;
        var accessor = CoreBlaze.Components.Utilities.PropertyAccessor.TryFor(typeof(T), col.Field);
        if (accessor is null) return string.Empty;
        var raw = accessor.Value.Getter(item!);
        if (raw is null) return string.Empty;
        if (col.Format is not null && raw is IFormattable f)
            return f.ToString(col.Format, null);
        return raw.ToString() ?? string.Empty;
    }

    private void RowClicked(T item)
    {
        if (OnRowClick.HasDelegate)
            OnRowClick.InvokeAsync(item);
    }
}
