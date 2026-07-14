using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Text;
using ClosedXML.Excel;
using CoreBlaze.Components.Models;
using CoreBlaze.Components.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CoreBlaze.Components.Grid;

/// <summary>
/// A flexible, generic data grid with sorting, per-column filtering, global
/// search, paging, row / cell editing (Add / Edit / Delete / Save / Cancel),
/// optional row selection, and CSV export.
/// </summary>
/// <typeparam name="T">The row item type. Must have a public parameter-less constructor for Add.</typeparam>
public partial class DataGrid<T> : ComponentBase where T : class
{
    static DataGrid()
    {
        // QuestPDF requires a license selection before any use. The Community licence covers
        // OSS and small commercial use (annual revenue ≤ USD 1M). Consumers with larger revenue
        // should set QuestPDF.Settings.License themselves before rendering.
        QuestPDF.Settings.License = LicenseType.Community;
    }

    private readonly List<GridColumn<T>> _columns = new();
    private readonly Dictionary<T, RowState> _rowStates = new();
    private readonly Dictionary<string, string?> _filters = new(StringComparer.Ordinal);
    private readonly HashSet<T> _selected = new();
    private readonly HashSet<string> _hiddenFields = new(StringComparer.Ordinal);
    private readonly List<SortDescriptor> _sortChain = new();

    // Validation error messages keyed by property name
    private readonly Dictionary<string, string[]> _editErrors = new(StringComparer.Ordinal);

    private IReadOnlyList<T> _pageData = Array.Empty<T>();
    private int _filteredCount;
    private int _page = 1;
    private int _pageSize = 10;
    private string? _search;

    private T? _editingItem;
    private T? _originalCopy;
    private GridColumn<T>? _editingColumn;
    private bool _isAdding;
    private bool _columnsMenuOpen;

    [Inject] private IJSRuntime JS { get; set; } = default!;

    /// <summary>Optional grid title rendered in the toolbar.</summary>
    [Parameter] public string? Title { get; set; }

    /// <summary>Extra classes for the root element.</summary>
    [Parameter] public string? CssClass { get; set; }

    /// <summary>Text displayed when there are no rows to show.</summary>
    [Parameter] public string? EmptyText { get; set; }

    /// <summary>The data source.</summary>
    [Parameter, EditorRequired] public IEnumerable<T>? Data { get; set; }

    /// <summary>Two-way binding companion for <see cref="Data"/> (for Add / Delete).</summary>
    [Parameter] public EventCallback<IEnumerable<T>> DataChanged { get; set; }

    /// <summary>The column definitions (as <c>&lt;GridColumn T="..."&gt;</c> children).</summary>
    [Parameter] public RenderFragment? Columns { get; set; }

    /// <summary>Additional toolbar content rendered to the right of the built-in buttons.</summary>
    [Parameter] public RenderFragment? Toolbar { get; set; }

    /// <summary>Enables paging.</summary>
    [Parameter] public bool Pageable { get; set; } = true;

    /// <summary>Initial page size.</summary>
    [Parameter] public int PageSize { get; set; } = 10;

    /// <summary>Shows filter inputs under the header row.</summary>
    [Parameter] public bool ShowFilterRow { get; set; } = true;

    /// <summary>Shows a global search box in the toolbar (searches every filterable column).</summary>
    [Parameter] public bool EnableGlobalSearch { get; set; }

    /// <summary>Placeholder text for the global search box.</summary>
    [Parameter] public string SearchPlaceholder { get; set; } = "Search…";

    /// <summary>Shows a "Columns" dropdown in the toolbar for toggling column visibility.</summary>
    [Parameter] public bool ShowColumnsMenu { get; set; } = true;

    /// <summary>Editing mode.</summary>
    [Parameter] public GridEditMode EditMode { get; set; } = GridEditMode.Row;

    /// <summary>Row-selection behaviour. Adds a select column when not <see cref="GridSelectionMode.None"/>.</summary>
    [Parameter] public GridSelectionMode SelectionMode { get; set; } = GridSelectionMode.None;

    /// <summary>Two-way bindable list of currently selected items.</summary>
    [Parameter] public IReadOnlyList<T>? SelectedItems { get; set; }

    /// <summary>Fires whenever the selection changes.</summary>
    [Parameter] public EventCallback<IReadOnlyList<T>> SelectedItemsChanged { get; set; }

    /// <summary>
    /// Enabled export formats. When non-<see cref="GridExportFormat.None"/> the toolbar
    /// renders an "Export" dropdown offering the selected formats.
    /// </summary>
    [Parameter] public GridExportFormat ExportFormats { get; set; } = GridExportFormat.None;

    /// <summary>File name (without extension) for exports. Defaults to <c>"export"</c>.</summary>
    [Parameter] public string? ExportFileName { get; set; }

    /// <summary>Title shown on the first page of the PDF export. Falls back to <see cref="Title"/>.</summary>
    [Parameter] public string? PdfTitle { get; set; }

    private bool _exportMenuOpen;

    private bool CanExportCsv => ExportFormats.HasFlag(GridExportFormat.Csv);
    private bool CanExportExcel => ExportFormats.HasFlag(GridExportFormat.Excel);
    // PDF export uses QuestPDF which requires native font access — not available in WebAssembly.
    private bool CanExportPdf => ExportFormats.HasFlag(GridExportFormat.Pdf) && !OperatingSystem.IsBrowser();
    private bool AnyExportEnabled => ExportFormats != GridExportFormat.None;

    /// <summary>Enables the Add-row command.</summary>
    [Parameter] public bool AllowAdd { get; set; } = true;

    /// <summary>Enables the Edit command.</summary>
    [Parameter] public bool AllowEdit { get; set; } = true;

    /// <summary>Enables the Delete command.</summary>
    [Parameter] public bool AllowDelete { get; set; } = true;

    /// <summary>Renders the built-in command column.</summary>
    [Parameter] public bool ShowCommandColumn { get; set; } = true;

    /// <summary>Fires when a row is saved (Add or Edit).</summary>
    [Parameter] public EventCallback<GridRowEventArgs<T>> OnRowSaved { get; set; }

    /// <summary>Fires when a row is deleted.</summary>
    [Parameter] public EventCallback<GridRowEventArgs<T>> OnRowDeleted { get; set; }

    // ── New event callbacks ──────────────────────────────────────────────────

    /// <summary>Fires when Add mode begins (before the user fills the new row).</summary>
    [Parameter] public EventCallback<T> OnRowAdd { get; set; }

    /// <summary>Fires when a row enters Edit mode.</summary>
    [Parameter] public EventCallback<T> OnRowEdit { get; set; }

    /// <summary>Fires when the user cancels an in-progress edit.</summary>
    [Parameter] public EventCallback OnCancel { get; set; }

    /// <summary>Fires when a single cell enters edit mode (Cell-edit mode only).</summary>
    [Parameter] public EventCallback<GridCellEditEventArgs<T>> OnCellEdit { get; set; }

    // ── Command-button templates ─────────────────────────────────────────────

    /// <summary>Custom template for the Add button in the toolbar. Receives no context.</summary>
    [Parameter] public RenderFragment? AddButtonTemplate { get; set; }

    /// <summary>Custom template for the Edit button in each row. Receives the row item.</summary>
    [Parameter] public RenderFragment<T>? EditButtonTemplate { get; set; }

    /// <summary>Custom template for the Save button shown while editing. Receives the row item.</summary>
    [Parameter] public RenderFragment<T>? SaveButtonTemplate { get; set; }

    /// <summary>Custom template for the Cancel button shown while editing. Receives no context.</summary>
    [Parameter] public RenderFragment? CancelButtonTemplate { get; set; }

    /// <summary>Custom template for the Delete button in each row. Receives the row item.</summary>
    [Parameter] public RenderFragment<T>? DeleteButtonTemplate { get; set; }

    // ── Bulk delete ──────────────────────────────────────────────────────────

    /// <summary>
    /// Shows a "Delete selected" button in the toolbar when rows are selected.
    /// Requires <see cref="SelectionMode"/> to be non-<see cref="GridSelectionMode.None"/>.
    /// </summary>
    [Parameter] public bool AllowBulkDelete { get; set; }

    /// <summary>Tracked persistence state for a row (or <see cref="RowState.Unchanged"/>).</summary>
    public RowState GetRowState(T item) => _rowStates.TryGetValue(item, out var s) ? s : RowState.Unchanged;

    /// <summary>The currently-editing item, if any.</summary>
    public T? EditingItem => _editingItem;

    /// <summary>Whether the grid is currently in an editing state.</summary>
    public bool IsEditing => _editingItem is not null;

    /// <summary>Whether the given row is selected.</summary>
    public bool IsSelected(T item) => _selected.Contains(item);

    /// <summary>Whether every row on the current page is selected.</summary>
    public bool IsAllOnPageSelected => _pageData.Count > 0 && _pageData.All(IsSelected);

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        _pageSize = Math.Max(1, PageSize);
        RecomputePage();
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        // Sync externally-supplied SelectedItems into the internal set.
        if (SelectedItems is not null)
        {
            var wanted = new HashSet<T>(SelectedItems);
            if (!_selected.SetEquals(wanted))
            {
                _selected.Clear();
                foreach (var i in wanted) _selected.Add(i);
            }
        }

        RecomputePage();
    }

    // ---------- Column registration (called by <GridColumn>) ----------

    internal void AddColumn(GridColumn<T> column)
    {
        if (_columns.Contains(column)) return;
        _columns.Add(column);
        StateHasChanged();
    }

    internal void RemoveColumn(GridColumn<T> column)
    {
        _columns.Remove(column);
        StateHasChanged();
    }

    // ---------- Filter / sort / page ----------

    private string? GetFilterText(string? field)
        => field is not null && _filters.TryGetValue(field, out var v) ? v : null;

    private void OnFilterChanged(string? field, string? value)
    {
        if (field is null) return;
        if (string.IsNullOrEmpty(value)) _filters.Remove(field);
        else _filters[field] = value;
        _page = 1;
        RecomputePage();
    }

    private void OnSearchChanged(string? text)
    {
        _search = string.IsNullOrWhiteSpace(text) ? null : text.Trim();
        _page = 1;
        RecomputePage();
    }

    private void OnSortHeaderClicked(GridColumn<T> col, MouseEventArgs args)
    {
        if (!col.Sortable || col.Field is null) return;

        var existing = _sortChain.FirstOrDefault(s => s.Field == col.Field);

        if (!args.ShiftKey)
        {
            // Single-column sort: cycle asc → desc → none
            _sortChain.Clear();
            if (existing is null)
                _sortChain.Add(new SortDescriptor { Field = col.Field, Direction = GridSortDirection.Ascending });
            else if (existing.Direction == GridSortDirection.Ascending)
                _sortChain.Add(new SortDescriptor { Field = col.Field, Direction = GridSortDirection.Descending });
            // else: descending → clear (no sort)
        }
        else
        {
            // Multi-sort: add / flip / remove within the chain
            if (existing is null)
            {
                _sortChain.Add(new SortDescriptor { Field = col.Field, Direction = GridSortDirection.Ascending });
            }
            else if (existing.Direction == GridSortDirection.Ascending)
            {
                var idx = _sortChain.IndexOf(existing);
                _sortChain[idx] = new SortDescriptor { Field = col.Field, Direction = GridSortDirection.Descending };
            }
            else
            {
                _sortChain.Remove(existing);
            }
        }

        RecomputePage();
    }

    private SortDescriptor? SortFor(string? field)
        => field is null ? null : _sortChain.FirstOrDefault(s => s.Field == field);

    private int SortOrdinal(string? field)
    {
        if (field is null) return 0;
        for (var i = 0; i < _sortChain.Count; i++)
            if (_sortChain[i].Field == field) return i + 1;
        return 0;
    }

    // ---------- Column visibility ----------

    /// <summary>The columns currently rendered (respects each column's <c>Visible</c> flag and any user toggles).</summary>
    public IReadOnlyList<GridColumn<T>> VisibleColumns
        => _columns.Where(IsColumnVisible).ToList();

    private bool IsColumnVisible(GridColumn<T> col)
        => col.Visible && (col.Field is null || !_hiddenFields.Contains(col.Field));

    private void ToggleColumn(GridColumn<T> col, bool visible)
    {
        if (col.Field is null || !col.Hideable) return;
        if (visible) _hiddenFields.Remove(col.Field);
        else _hiddenFields.Add(col.Field);
    }

    private void ShowAllColumns()
    {
        _hiddenFields.Clear();
    }

    private void ToggleColumnsMenu() => _columnsMenuOpen = !_columnsMenuOpen;
    private void CloseColumnsMenu() => _columnsMenuOpen = false;

    private void OnPageChanged(int page)
    {
        _page = page;
        RecomputePage();
    }

    private void OnPageSizeChanged(int size)
    {
        _pageSize = size;
        _page = 1;
        RecomputePage();
    }

    private IEnumerable<T> GetFilteredSorted()
    {
        if (Data is null) return Array.Empty<T>();

        IEnumerable<T> query = Data;

        // Per-column filter
        foreach (var f in _filters)
        {
            var accessor = PropertyAccessor.TryFor(typeof(T), f.Key);
            if (accessor is null) continue;
            var text = f.Value ?? string.Empty;
            var g = accessor.Value.Getter;
            query = query.Where(x =>
            {
                var v = g(x);
                return v is not null && v.ToString()!.Contains(text, StringComparison.OrdinalIgnoreCase);
            });
        }

        // Global search — matches any filterable column
        if (!string.IsNullOrEmpty(_search))
        {
            var needle = _search;
            var searchable = _columns
                .Where(c => c.Field is not null && c.Filterable)
                .Select(c => PropertyAccessor.TryFor(typeof(T), c.Field!))
                .Where(a => a is not null)
                .Select(a => a!.Value.Getter)
                .ToList();

            query = query.Where(x =>
                searchable.Any(g =>
                {
                    var v = g(x);
                    return v is not null && v.ToString()!.Contains(needle, StringComparison.OrdinalIgnoreCase);
                }));
        }

        var list = query.ToList();

        // Multi-column sort
        if (_sortChain.Count > 0)
        {
            IOrderedEnumerable<T>? ordered = null;
            var comparer = Comparer<object?>.Create(CompareObjects);
            foreach (var s in _sortChain)
            {
                var accessor = PropertyAccessor.TryFor(typeof(T), s.Field);
                if (accessor is null) continue;
                var g = accessor.Value.Getter;

                if (ordered is null)
                {
                    ordered = s.Direction == GridSortDirection.Ascending
                        ? list.OrderBy(x => g(x), comparer)
                        : list.OrderByDescending(x => g(x), comparer);
                }
                else
                {
                    ordered = s.Direction == GridSortDirection.Ascending
                        ? ordered.ThenBy(x => g(x), comparer)
                        : ordered.ThenByDescending(x => g(x), comparer);
                }
            }
            if (ordered is not null) list = ordered.ToList();
        }

        return list;
    }

    private void RecomputePage()
    {
        var list = GetFilteredSorted() as List<T> ?? GetFilteredSorted().ToList();
        _filteredCount = list.Count;

        if (Pageable)
        {
            var totalPages = Math.Max(1, (int)Math.Ceiling(_filteredCount / (double)_pageSize));
            _page = Math.Clamp(_page, 1, totalPages);
            _pageData = list.Skip((_page - 1) * _pageSize).Take(_pageSize).ToList();
        }
        else
        {
            _pageData = list;
        }
    }

    private static int CompareObjects(object? a, object? b)
    {
        if (a is null && b is null) return 0;
        if (a is null) return -1;
        if (b is null) return 1;
        if (a is IComparable ca && a.GetType() == b.GetType()) return ca.CompareTo(b);
        return string.Compare(a.ToString(), b.ToString(), StringComparison.CurrentCulture);
    }

    // ---------- Selection ----------

    private async Task ToggleSelectAsync(T item, bool selected)
    {
        if (SelectionMode == GridSelectionMode.None) return;

        if (SelectionMode == GridSelectionMode.Single)
        {
            _selected.Clear();
            if (selected) _selected.Add(item);
        }
        else
        {
            if (selected) _selected.Add(item);
            else _selected.Remove(item);
        }

        await NotifySelectionAsync();
    }

    private async Task ToggleSelectAllOnPageAsync(bool selected)
    {
        if (SelectionMode != GridSelectionMode.Multiple) return;

        if (selected)
            foreach (var item in _pageData) _selected.Add(item);
        else
            foreach (var item in _pageData) _selected.Remove(item);

        await NotifySelectionAsync();
    }

    /// <summary>Clears all selection.</summary>
    public async Task ClearSelectionAsync()
    {
        if (_selected.Count == 0) return;
        _selected.Clear();
        await NotifySelectionAsync();
    }

    private Task NotifySelectionAsync()
        => SelectedItemsChanged.InvokeAsync(_selected.ToList());

    // ---------- Bulk delete ----------

    /// <summary>
    /// Deletes all currently selected rows at once.
    /// Fires <see cref="OnRowDeleted"/> for each removed item.
    /// </summary>
    public async Task BulkDeleteAsync()
    {
        if (_selected.Count == 0) return;

        var toDelete = _selected.ToList();
        _selected.Clear();

        foreach (var item in toDelete)
        {
            _rowStates[item] = RowState.Deleted;

            if (Data is IList<T> list)
                list.Remove(item);

            await OnRowDeleted.InvokeAsync(new GridRowEventArgs<T>(item, RowState.Deleted));
        }

        if (Data is not IList<T> && Data is not null)
        {
            var copy = Data.Where(x => !toDelete.Contains(x)).ToList();
            await DataChanged.InvokeAsync(copy);
        }

        RecomputePage();
    }

    // ---------- Validation helpers ----------

    private bool HasEditError(GridColumn<T>? col)
        => col?.Field is not null && _editErrors.ContainsKey(col.Field);

    private IEnumerable<string> GetEditErrors(GridColumn<T>? col)
        => col?.Field is not null && _editErrors.TryGetValue(col.Field, out var msgs)
            ? msgs
            : Enumerable.Empty<string>();

    // ---------- Editing ----------

    private async Task BeginAdd()
    {
        var newItem = Activator.CreateInstance<T>();
        _editingItem = newItem;
        _originalCopy = null;
        _isAdding = true;
        _editErrors.Clear();
        _rowStates[newItem] = RowState.Added;
        await OnRowAdd.InvokeAsync(newItem);
    }

    private async Task BeginEdit(T item)
    {
        _editingItem = item;
        _originalCopy = CloneShallow(item);
        _isAdding = false;
        _editingColumn = null;
        _editErrors.Clear();
        if (!_rowStates.ContainsKey(item))
            _rowStates[item] = RowState.Modified;
        await OnRowEdit.InvokeAsync(item);
    }

    private async Task OnCellDoubleClicked(T item, GridColumn<T> col)
    {
        if (EditMode != GridEditMode.Cell || _isAdding || !AllowEdit) return;
        _editingItem = item;
        _editingColumn = col;
        _originalCopy = CloneShallow(item);
        _editErrors.Clear();
        if (!_rowStates.ContainsKey(item))
            _rowStates[item] = RowState.Modified;
        await OnCellEdit.InvokeAsync(new GridCellEditEventArgs<T>(item, col.Field));
    }

    private async Task OnEditKeyDown(KeyboardEventArgs args)
    {
        if (_editingItem is null) return;
        if (args.Key == "Enter") await CommitAsync();
        else if (args.Key == "Escape") await CancelAsync();
    }

    private async Task CommitAsync()
    {
        if (_editingItem is null) return;

        // ── DataAnnotations validation ───────────────────────────────────────
        var validationContext = new ValidationContext(_editingItem);
        var validationResults = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(
            _editingItem, validationContext, validationResults, validateAllProperties: true);

        if (!isValid)
        {
            _editErrors.Clear();
            foreach (var vr in validationResults)
            {
                var msg = vr.ErrorMessage ?? "Invalid";
                foreach (var memberName in vr.MemberNames)
                {
                    if (!_editErrors.TryGetValue(memberName, out var existing))
                        _editErrors[memberName] = new[] { msg };
                    else
                        _editErrors[memberName] = existing.Append(msg).ToArray();
                }
            }
            return; // stay in edit mode — errors are now displayed inline
        }

        _editErrors.Clear();
        // ── Commit ──────────────────────────────────────────────────────────
        var state = _isAdding ? RowState.Added : RowState.Modified;
        _rowStates[_editingItem] = state;

        if (_isAdding)
        {
            var list = (Data as IList<T>) ?? Data?.ToList() ?? new List<T>();
            if (list is List<T> concrete) concrete.Add(_editingItem);
            else list.Add(_editingItem);
            await DataChanged.InvokeAsync(list);
        }

        await OnRowSaved.InvokeAsync(new GridRowEventArgs<T>(_editingItem, state));

        _editingItem = null;
        _editingColumn = null;
        _originalCopy = null;
        _isAdding = false;
        RecomputePage();
    }

    private async Task CancelAsync()
    {
        if (_editingItem is null) return;

        if (_isAdding)
        {
            _rowStates.Remove(_editingItem);
        }
        else if (_originalCopy is not null)
        {
            RestoreShallow(_editingItem, _originalCopy);
            if (_rowStates.TryGetValue(_editingItem, out var s) && s == RowState.Modified)
                _rowStates.Remove(_editingItem);
        }

        _editErrors.Clear();
        _editingItem = null;
        _editingColumn = null;
        _originalCopy = null;
        _isAdding = false;
        await OnCancel.InvokeAsync();
        RecomputePage();
    }

    private async Task DeleteAsync(T item)
    {
        _rowStates[item] = RowState.Deleted;
        _selected.Remove(item);

        if (Data is IList<T> list)
        {
            list.Remove(item);
            await DataChanged.InvokeAsync(list);
        }
        else if (Data is not null)
        {
            var copy = Data.Where(x => !ReferenceEquals(x, item)).ToList();
            await DataChanged.InvokeAsync(copy);
        }
        await OnRowDeleted.InvokeAsync(new GridRowEventArgs<T>(item, RowState.Deleted));
        RecomputePage();
    }

    // ---------- Export ----------

    /// <summary>
    /// Exports the current filtered / sorted view (not just the current page) as a
    /// CSV file, triggering a browser download.
    /// </summary>
    public async Task ExportCsvAsync()
    {
        var columnDefs = ExportableColumns();
        if (columnDefs.Count == 0) return;

        var sb = new StringBuilder();
        sb.AppendLine(string.Join(",", columnDefs.Select(c => EscapeCsv(HeaderText(c)))));

        foreach (var item in GetFilteredSorted())
        {
            var values = columnDefs.Select(c => EscapeCsv(CellText(c, item, CultureInfo.InvariantCulture)));
            sb.AppendLine(string.Join(",", values));
        }

        await DownloadAsync(BaseFileName + ".csv", "text/csv;charset=utf-8", Encoding.UTF8.GetBytes(sb.ToString()));
    }

    /// <summary>
    /// Exports the current filtered / sorted view as a real Office Open XML
    /// workbook (<c>.xlsx</c>) using ClosedXML.
    /// </summary>
    public async Task ExportExcelAsync()
    {
        var columnDefs = ExportableColumns();
        if (columnDefs.Count == 0) return;

        using var workbook = new XLWorkbook();
        var sheet = workbook.AddWorksheet(SafeSheetName(Title ?? "Data"));

        // Header row
        for (var i = 0; i < columnDefs.Count; i++)
        {
            var cell = sheet.Cell(1, i + 1);
            cell.Value = HeaderText(columnDefs[i]);
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        }

        // Data rows — use ClosedXML's typed setter when possible
        var row = 2;
        foreach (var item in GetFilteredSorted())
        {
            for (var i = 0; i < columnDefs.Count; i++)
            {
                var col = columnDefs[i];
                var cell = sheet.Cell(row, i + 1);
                var accessor = PropertyAccessor.TryFor(typeof(T), col.Field!);
                var value = accessor?.Getter(item);

                switch (value)
                {
                    case null: cell.Clear(); break;
                    case string s: cell.Value = s; break;
                    case bool b: cell.Value = b; break;
                    case DateTime dt: cell.Value = dt; cell.Style.DateFormat.Format = "yyyy-mm-dd"; break;
                    case DateOnly dOnly: cell.Value = dOnly.ToDateTime(TimeOnly.MinValue); cell.Style.DateFormat.Format = "yyyy-mm-dd"; break;
                    case decimal dec: cell.Value = dec; break;
                    case double dbl: cell.Value = dbl; break;
                    case float f: cell.Value = f; break;
                    case int i32: cell.Value = i32; break;
                    case long l: cell.Value = l; break;
                    case short sh: cell.Value = sh; break;
                    case byte by: cell.Value = by; break;
                    default: cell.Value = value.ToString(); break;
                }
            }
            row++;
        }

        sheet.Columns().AdjustToContents();
        sheet.SheetView.FreezeRows(1);

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        await DownloadAsync(BaseFileName + ".xlsx",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ms.ToArray());
    }

    /// <summary>
    /// Exports the current filtered / sorted view as a PDF (using QuestPDF).
    /// The document renders a title, a small metadata block, and a striped table.
    /// </summary>
    public async Task ExportPdfAsync()
    {
        if (OperatingSystem.IsBrowser())
            throw new NotSupportedException(
                "PDF export is not available in WebAssembly. Use CSV or Excel export instead.");

        var columnDefs = ExportableColumns();
        if (columnDefs.Count == 0) return;

        var rows = GetFilteredSorted().ToList();
        var title = PdfTitle ?? Title ?? "Report";
        var generated = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(col =>
                {
                    col.Item().Text(title).FontSize(18).SemiBold();
                    col.Item().Text($"{rows.Count} rows · Generated {generated}")
                        .FontSize(9).FontColor(Colors.Grey.Medium);
                });

                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        foreach (var _ in columnDefs) cols.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        foreach (var col in columnDefs)
                        {
                            header.Cell().Element(HeaderCell).Text(HeaderText(col)).SemiBold();
                        }
                    });

                    var index = 0;
                    foreach (var item in rows)
                    {
                        var isEven = index++ % 2 == 0;
                        foreach (var col in columnDefs)
                        {
                            table.Cell().Element(c => BodyCell(c, isEven)).Text(CellText(col, item, CultureInfo.CurrentCulture));
                        }
                    }

                    static QuestPDF.Infrastructure.IContainer HeaderCell(QuestPDF.Infrastructure.IContainer c)
                        => c.Background(Colors.Grey.Lighten3).PaddingVertical(6).PaddingHorizontal(8);

                    static QuestPDF.Infrastructure.IContainer BodyCell(QuestPDF.Infrastructure.IContainer c, bool isEven)
                        => c.Background(isEven ? Colors.White : Colors.Grey.Lighten5)
                            .PaddingVertical(4).PaddingHorizontal(8)
                            .BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2);
                });

                page.Footer().AlignRight().Text(t =>
                {
                    t.Span("Page ").FontSize(8).FontColor(Colors.Grey.Medium);
                    t.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                    t.Span(" of ").FontSize(8).FontColor(Colors.Grey.Medium);
                    t.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });
        }).GeneratePdf();

        await DownloadAsync(BaseFileName + ".pdf", "application/pdf", pdf);
    }

    private List<GridColumn<T>> ExportableColumns()
        => _columns.Where(c => c.Field is not null && IsColumnVisible(c)).ToList();

    private static string HeaderText(GridColumn<T> col) => col.Title ?? col.Field!;

    private string CellText(GridColumn<T> col, T item, CultureInfo culture)
    {
        var acc = PropertyAccessor.TryFor(typeof(T), col.Field!);
        if (acc is null) return string.Empty;
        var val = acc.Value.Getter(item);
        return val switch
        {
            null => string.Empty,
            IFormattable f when !string.IsNullOrEmpty(col.Format) => f.ToString(col.Format, culture),
            _ => val.ToString() ?? string.Empty
        };
    }

    private string BaseFileName
        => string.IsNullOrWhiteSpace(ExportFileName) ? "export" : ExportFileName!.TrimEnd('.', ' ');

    private async Task DownloadAsync(string fileName, string mimeType, byte[] bytes)
    {
        var b64 = Convert.ToBase64String(bytes);
        _exportMenuOpen = false;
        await JS.InvokeVoidAsync("CoreBlaze.download", fileName, b64, mimeType);
    }

    private void ToggleExportMenu() => _exportMenuOpen = !_exportMenuOpen;
    private void CloseExportMenu() => _exportMenuOpen = false;

    private static string EscapeCsv(string s)
    {
        if (s.Contains(',') || s.Contains('"') || s.Contains('\n') || s.Contains('\r'))
            return "\"" + s.Replace("\"", "\"\"") + "\"";
        return s;
    }

    private static string SafeSheetName(string name)
    {
        // Excel: sheet names ≤ 31 chars, no : \ / ? * [ ]
        var cleaned = new string(name.Where(c => c is not (':' or '\\' or '/' or '?' or '*' or '[' or ']')).ToArray());
        return cleaned.Length > 31 ? cleaned[..31] : cleaned;
    }

    // ---------- Cell rendering ----------

    private RenderFragment RenderDisplayCell(GridColumn<T> col, T item) => builder =>
    {
        if (col.Template is not null)
        {
            builder.AddContent(0, col.Template(item));
            return;
        }

        if (col.Field is null) return;
        var accessor = PropertyAccessor.TryFor(typeof(T), col.Field);
        if (accessor is null) return;

        var value = accessor.Value.Getter(item);
        var text = value switch
        {
            null => string.Empty,
            IFormattable f when !string.IsNullOrEmpty(col.Format) => f.ToString(col.Format, CultureInfo.CurrentCulture),
            _ => value.ToString() ?? string.Empty
        };
        builder.AddContent(1, text);
    };

    private RenderFragment RenderEditCell(GridColumn<T> col, T item) => builder =>
    {
        if (col.EditTemplate is not null)
        {
            builder.AddContent(0, col.EditTemplate(item));
            return;
        }

        if (col.Field is null || !col.Editable)
        {
            builder.AddContent(1, RenderDisplayCell(col, item));
            return;
        }

        var accessor = PropertyAccessor.TryFor(typeof(T), col.Field);
        if (accessor is null) return;

        var current = accessor.Value.Getter(item);
        var propType = accessor.Value.PropertyType;
        var underlying = Nullable.GetUnderlyingType(propType) ?? propType;

        if (underlying == typeof(bool))
        {
            builder.OpenElement(10, "input");
            builder.AddAttribute(11, "type", "checkbox");
            builder.AddAttribute(12, "class", "cb-grid__edit-input");
            builder.AddAttribute(13, "checked", current as bool? ?? false);
            builder.AddAttribute(14, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, e =>
            {
                if (accessor.Value.Setter is null) return;
                accessor.Value.Setter(item, e.Value ?? false);
            }));
            builder.CloseElement();
            return;
        }

        if (underlying == typeof(DateTime))
        {
            var d = current as DateTime? ?? (current is DateTime dt ? dt : (DateTime?)null);
            builder.OpenElement(20, "input");
            builder.AddAttribute(21, "type", "date");
            builder.AddAttribute(22, "class", "cb-grid__edit-input");
            builder.AddAttribute(23, "value", d?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            builder.AddAttribute(24, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, e =>
            {
                if (accessor.Value.Setter is null) return;
                var text = e.Value?.ToString();
                if (string.IsNullOrEmpty(text)) { accessor.Value.Setter(item, null); return; }
                if (DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                    accessor.Value.Setter(item, parsed);
            }));
            builder.CloseElement();
            return;
        }

        if (IsNumeric(underlying))
        {
            builder.OpenElement(30, "input");
            builder.AddAttribute(31, "type", "number");
            builder.AddAttribute(32, "class", "cb-grid__edit-input");
            builder.AddAttribute(33, "value", current?.ToString());
            builder.AddAttribute(34, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, e =>
            {
                if (accessor.Value.Setter is null) return;
                var text = e.Value?.ToString();
                if (string.IsNullOrEmpty(text)) { accessor.Value.Setter(item, null); return; }
                try
                {
                    var converted = Convert.ChangeType(text, underlying, CultureInfo.InvariantCulture);
                    accessor.Value.Setter(item, converted);
                }
                catch { /* keep old value */ }
            }));
            builder.CloseElement();
            return;
        }

        builder.OpenElement(40, "input");
        builder.AddAttribute(41, "type", "text");
        builder.AddAttribute(42, "class", "cb-grid__edit-input");
        builder.AddAttribute(43, "value", current?.ToString() ?? string.Empty);
        builder.AddAttribute(44, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, e =>
        {
            if (accessor.Value.Setter is null) return;
            var text = e.Value?.ToString();
            try
            {
                if (underlying == typeof(string)) accessor.Value.Setter(item, text);
                else if (string.IsNullOrEmpty(text)) accessor.Value.Setter(item, null);
                else accessor.Value.Setter(item, Convert.ChangeType(text, underlying, CultureInfo.CurrentCulture));
            }
            catch { /* ignore */ }
        }));
        builder.CloseElement();
    };

    private static bool IsNumeric(Type t)
        => t == typeof(byte) || t == typeof(sbyte)
        || t == typeof(short) || t == typeof(ushort)
        || t == typeof(int) || t == typeof(uint)
        || t == typeof(long) || t == typeof(ulong)
        || t == typeof(float) || t == typeof(double)
        || t == typeof(decimal);

    private static T CloneShallow(T source)
    {
        var clone = Activator.CreateInstance<T>();
        foreach (var p in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (p.CanRead && p.CanWrite)
                p.SetValue(clone, p.GetValue(source));
        }
        return clone;
    }

    private static void RestoreShallow(T target, T source)
    {
        foreach (var p in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (p.CanRead && p.CanWrite)
                p.SetValue(target, p.GetValue(source));
        }
    }
}

/// <summary>Row event payload for grid callbacks.</summary>
public sealed class GridRowEventArgs<T>
{
    /// <summary>The affected row.</summary>
    public T Item { get; }

    /// <summary>The new persistence state.</summary>
    public RowState State { get; }

    /// <summary>Creates a new instance.</summary>
    public GridRowEventArgs(T item, RowState state)
    {
        Item = item;
        State = state;
    }
}
