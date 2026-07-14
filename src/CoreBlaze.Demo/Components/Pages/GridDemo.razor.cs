using CoreBlaze.Components.Grid;
using CoreBlaze.Components.Models;
using CoreBlaze.Components.Services;
using CoreBlaze.Demo.Models;
using CoreBlaze.Demo.Services;
using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Demo.Components.Pages;

public partial class GridDemo : ComponentBase
{
    [Inject] private PersonRepository Repo { get; set; } = default!;
    [Inject] private ToastService Toasts { get; set; } = default!;

    private List<Person> _people = new();
    private GridEditMode _mode = GridEditMode.Row;
    private GridSelectionMode _selection = GridSelectionMode.Multiple;
    private IReadOnlyList<Person>? _selectedPeople;
    private string? _lastEvent;
    private DataGrid<Person>? _grid;

    protected override void OnInitialized() => _people = Repo.GetAll();

    private void OnRowSaved(GridRowEventArgs<Person> e)
    {
        _lastEvent = $"Saved [{e.State}] {e.Item.Name}";
        Toasts.Success($"{e.Item.Name} saved.", title: e.State == RowState.Added ? "Row added" : "Row updated");
    }

    private void OnRowDeleted(GridRowEventArgs<Person> e)
    {
        _lastEvent = $"Deleted {e.Item.Name}";
        Toasts.Warning($"{e.Item.Name} was deleted.", title: "Row deleted");
    }

    private void OnRowAdded(Person p)
        => Toasts.Info($"Add mode started.", title: "New row");

    private void OnRowEdited(Person p)
        => _lastEvent = $"Editing: {p.Name}";

    private void OnCancelled()
        => Toasts.Info("Edit cancelled.", title: "Cancelled");

    private void OnCellEdited(GridCellEditEventArgs<Person> args)
        => _lastEvent = $"Cell edit: {args.Field} on {args.Item.Name}";

    private const string _code = """
        @using CoreBlaze.Components.Grid
        @using CoreBlaze.Components.Models
        @using CoreBlaze.Components.Services

        @inject ToastService Toasts

        <DataGrid T="Person"
                  @ref="_grid"
                  Title="People"
                  Data="_people"
                  EditMode="_mode"
                  SelectionMode="_selection"
                  @bind-SelectedItems="_selected"
                  EnableGlobalSearch="true"
                  SearchPlaceholder="Search people…"
                  ShowColumnsMenu="true"
                  ExportFormats="GridExportFormat.All"
                  ExportFileName="people"
                  PdfTitle="People — report"
                  PageSize="5"
                  OnRowSaved="OnRowSaved"
                  OnRowDeleted="OnRowDeleted">
            <Columns>
                <GridColumn T="Person" Field="Id"         Title="ID"   Width="4rem" Editable="false" />
                <GridColumn T="Person" Field="Name"       Title="Name" />
                <GridColumn T="Person" Field="Age"        Title="Age" />

                <!-- Custom EditTemplate — use any input inline while editing -->
                <GridColumn T="Person" Field="Department" Title="Department">
                    <EditTemplate Context="row">
                        <Dropdown TItem="string" TValue="string"
                                  Items="_departments"
                                  ValueSelector="x => x"
                                  TextSelector="x => x"
                                  Value="row.Department"
                                  ValueChanged="v => row.Department = v"
                                  ValueExpression="() => row.Department" />
                    </EditTemplate>
                </GridColumn>

                <GridColumn T="Person" Field="HireDate" Title="Hire date" Format="d" />

                <!-- Custom display Template — render bool as a coloured badge -->
                <GridColumn T="Person" Field="IsActive" Title="Active" Width="7rem">
                    <Template Context="row">
                        <span class="badge @(row.IsActive ? "bg-success" : "bg-secondary")">
                            @(row.IsActive ? "Active" : "Inactive")
                        </span>
                    </Template>
                </GridColumn>
            </Columns>
        </DataGrid>

        @code {
            List<Person> _people = LoadPeople();
            IReadOnlyList<Person>? _selected;

            // Toggle these to demo different modes
            GridEditMode _mode = GridEditMode.Row;
            GridSelectionMode _selection = GridSelectionMode.Multiple;

            DataGrid<Person>? _grid;
            readonly string[] _departments = { "Engineering", "Sales", "Marketing", "Finance", "Support" };

            void OnRowSaved(GridRowEventArgs<Person> e)
            {
                var verb = e.State == RowState.Added ? "added" : "updated";
                Toasts.Success($"{e.Item.Name} was {verb}.", title: $"Row {verb}");
            }

            void OnRowDeleted(GridRowEventArgs<Person> e)
                => Toasts.Warning($"{e.Item.Name} was deleted.", title: "Row deleted");
        }
        """;
}
