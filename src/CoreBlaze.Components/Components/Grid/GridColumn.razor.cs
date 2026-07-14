using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Components.Grid;

/// <summary>
/// Declarative column definition for <see cref="DataGrid{T}"/>. Registers itself with
/// the parent grid on init and unregisters on dispose. Non-visual by itself.
/// </summary>
public partial class GridColumn<T> : ComponentBase, IDisposable where T : class
{
    /// <summary>Injected cascade — the parent grid this column belongs to.</summary>
    [CascadingParameter] public DataGrid<T>? Parent { get; set; }

    /// <summary>The bound property name on <typeparamref name="T"/>. Required unless <see cref="Template"/> is used.</summary>
    [Parameter] public string? Field { get; set; }

    /// <summary>Header text. Falls back to <see cref="Field"/> when not set.</summary>
    [Parameter] public string? Title { get; set; }

    /// <summary>Optional column width (any valid CSS width, e.g. <c>10rem</c>).</summary>
    [Parameter] public string? Width { get; set; }

    /// <summary>When <see langword="true"/> the header shows a sort toggle (shift-click adds a secondary sort).</summary>
    [Parameter] public bool Sortable { get; set; } = true;

    /// <summary>When <see langword="true"/> a filter input appears in the filter row and the column is included in global search.</summary>
    [Parameter] public bool Filterable { get; set; } = true;

    /// <summary>When <see langword="true"/> the column is visible. Set to <see langword="false"/> to hide by default.</summary>
    [Parameter] public bool Visible { get; set; } = true;

    /// <summary>Whether the user can toggle this column from the "Columns" menu. Set to <see langword="false"/> to lock the column always-on.</summary>
    [Parameter] public bool Hideable { get; set; } = true;

    /// <summary>When <see langword="true"/> the column is editable in edit mode.</summary>
    [Parameter] public bool Editable { get; set; } = true;

    /// <summary>Optional format string passed to <see cref="IFormattable.ToString(string, IFormatProvider)"/>.</summary>
    [Parameter] public string? Format { get; set; }

    /// <summary>Custom display template. Receives the row item.</summary>
    [Parameter] public RenderFragment<T>? Template { get; set; }

    /// <summary>Custom edit template. Receives the row item.</summary>
    [Parameter] public RenderFragment<T>? EditTemplate { get; set; }

    /// <summary>Custom header template.</summary>
    [Parameter] public RenderFragment? HeaderTemplate { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (Parent is null)
            throw new InvalidOperationException($"{nameof(GridColumn<T>)} must be a child of a DataGrid.");
        Parent.AddColumn(this);
    }

    /// <inheritdoc />
    public void Dispose() => Parent?.RemoveColumn(this);
}
