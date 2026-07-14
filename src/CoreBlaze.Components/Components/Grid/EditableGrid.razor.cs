using CoreBlaze.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Components.Grid;

/// <summary>
/// A fully inline-editable grid where every cell is a live input.
/// Supports text, number, date, checkbox, and dropdown columns with automatic
/// type detection. Use <see cref="DataGrid{T}"/> for sorting, filtering, paging,
/// and export; use this for compact editable forms displayed as a table.
/// </summary>
/// <typeparam name="T">Row item type. Must have public settable properties.</typeparam>
public partial class EditableGrid<T> : ComponentBase where T : class
{
    private readonly List<EditableGridColumn<T>> _columns = new();
    private bool _ready;
    private int _changeCount;

    // ── Parameters ───────────────────────────────────────────────────────────

    /// <summary>The data collection. Use <c>@bind-Items</c> for two-way binding.</summary>
    [Parameter, EditorRequired] public IList<T> Items { get; set; } = new List<T>();

    /// <summary>Fires when <see cref="Items"/> is replaced (row added/deleted).</summary>
    [Parameter] public EventCallback<IList<T>> ItemsChanged { get; set; }

    /// <summary>Column definitions — place <see cref="EditableGridColumn{T}"/> children here.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Optional title shown in the toolbar.</summary>
    [Parameter] public string? Title { get; set; }

    /// <summary>Factory that creates a new blank row when the user clicks "Add row".</summary>
    [Parameter] public Func<T>? NewRowFactory { get; set; }

    /// <summary>Show the "+ Add row" button.</summary>
    [Parameter] public bool AllowAdd { get; set; } = true;

    /// <summary>Show per-row delete buttons.</summary>
    [Parameter] public bool AllowDelete { get; set; } = true;

    /// <summary>Alternating row background.</summary>
    [Parameter] public bool Striped { get; set; }

    /// <summary>Compact row padding.</summary>
    [Parameter] public bool Dense { get; set; }

    /// <summary>Show row count + unsaved changes count in the footer.</summary>
    [Parameter] public bool ShowFooter { get; set; } = true;

    /// <summary>Message shown when the collection is empty.</summary>
    [Parameter] public string? EmptyMessage { get; set; }

    /// <summary>Extra CSS class on the root wrapper.</summary>
    [Parameter] public string? CssClass { get; set; }

    /// <summary>Fired after any cell value changes. Receives the modified row.</summary>
    [Parameter] public EventCallback<T> OnChange { get; set; }

    /// <summary>Fired after a row is added. Receives the new row.</summary>
    [Parameter] public EventCallback<T> OnRowAdd { get; set; }

    /// <summary>Fired after a row is deleted. Receives the deleted row.</summary>
    [Parameter] public EventCallback<T> OnRowDelete { get; set; }

    // ── Column registration ───────────────────────────────────────────────────

    internal void AddColumn(EditableGridColumn<T> col)
    {
        if (!_columns.Contains(col))
        {
            _columns.Add(col);
            _ready = true;
            StateHasChanged();
        }
    }

    internal void RemoveColumn(EditableGridColumn<T> col)
    {
        _columns.Remove(col);
        StateHasChanged();
    }

    // ── Row operations ────────────────────────────────────────────────────────

    private async Task AddRow()
    {
        T newItem = NewRowFactory is not null
            ? NewRowFactory()
            : Activator.CreateInstance<T>();

        Items.Add(newItem);
        await ItemsChanged.InvokeAsync(Items);
        await OnRowAdd.InvokeAsync(newItem);
        StateHasChanged();
    }

    private async Task DeleteRow(T item)
    {
        Items.Remove(item);
        await ItemsChanged.InvokeAsync(Items);
        await OnRowDelete.InvokeAsync(item);
        StateHasChanged();
    }

    // ── Cell value ───────────────────────────────────────────────────────────

    internal sealed record CellInfo(
        bool IsCheckbox, bool IsDate, bool IsNumber, bool HasOptions, bool IsReadonly,
        string InputType, string DisplayValue, bool CheckedValue, string DateValue,
        IReadOnlyList<string>? Options);

    internal CellInfo GetCellInfo(EditableGridColumn<T> col, T item)
    {
        if (col.Field is null)
            return new(false, false, false, false, true, "text", "", false, "", null);

        var accessor = PropertyAccessor.TryFor(typeof(T), col.Field);
        if (accessor is null)
            return new(false, false, false, false, true, "text", col.Field, false, "", null);

        var raw = accessor.Value.Getter(item!);
        var pt = Nullable.GetUnderlyingType(accessor.Value.PropertyType) ?? accessor.Value.PropertyType;
        bool hasSetter = accessor.Value.Setter is not null;

        bool isCheckbox = pt == typeof(bool);
        bool isDate     = pt == typeof(DateTime) || pt == typeof(DateOnly) || pt == typeof(DateTimeOffset);
        bool isNumber   = pt == typeof(int) || pt == typeof(long) || pt == typeof(short)
                       || pt == typeof(decimal) || pt == typeof(double) || pt == typeof(float);

        bool isReadonly = col.Readonly || !hasSetter;
        bool hasOptions = col.Options?.Count > 0;

        string inputType = col.InputType
            ?? (isNumber ? "number" : isDate ? "date" : isCheckbox ? "checkbox" : "text");

        string displayValue = raw is null ? "" :
            col.Format is not null && raw is IFormattable f
                ? f.ToString(col.Format, null)
                : raw.ToString() ?? "";

        bool checkedValue = raw is true;

        string dateValue = raw switch
        {
            DateTime dt => dt.ToString("yyyy-MM-dd"),
            DateOnly  d  => d.ToString("yyyy-MM-dd"),
            _ => displayValue,
        };

        return new CellInfo(isCheckbox, isDate, isNumber, hasOptions, isReadonly,
            inputType, displayValue, checkedValue, dateValue, col.Options);
    }

    private async Task SetValueAsync(T item, EditableGridColumn<T> col, object? rawInput)
    {
        if (col.Field is null || col.Readonly) return;
        var accessor = PropertyAccessor.TryFor(typeof(T), col.Field);
        if (accessor?.Setter is null) return;

        var pt = Nullable.GetUnderlyingType(accessor.Value.PropertyType) ?? accessor.Value.PropertyType;

        object? converted;
        try
        {
            string? str = rawInput?.ToString();
            if (str is null or "")
            {
                converted = pt.IsValueType ? Activator.CreateInstance(pt) : null;
            }
            else if (pt == typeof(bool))
            {
                converted = rawInput is bool b ? b : bool.Parse(str);
            }
            else if (pt == typeof(DateTime))
            {
                converted = DateTime.Parse(str);
            }
            else if (pt == typeof(DateOnly))
            {
                converted = DateOnly.Parse(str);
            }
            else if (pt == typeof(DateTimeOffset))
            {
                converted = DateTimeOffset.Parse(str);
            }
            else
            {
                converted = Convert.ChangeType(str, pt, System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        catch
        {
            return; // Ignore unconvertible input
        }

        accessor.Value.Setter!(item!, converted);
        _changeCount++;
        await OnChange.InvokeAsync(item);
        StateHasChanged();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private IReadOnlyList<EditableGridColumn<T>> _visibleCols =>
        _columns.Where(c => c.Visible).ToList();

    private static string AlignClass(EditableGridColumn<T> col) => col.Align switch
    {
        GridAlign.Center => "cb-eg--center",
        GridAlign.Right  => "cb-eg--right",
        _                => string.Empty,
    };
}
