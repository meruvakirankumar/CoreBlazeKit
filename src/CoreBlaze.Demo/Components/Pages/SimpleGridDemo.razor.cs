using CoreBlaze.Components.Grid;
using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Demo.Components.Pages;

public partial class SimpleGridDemo : ComponentBase
{
    public sealed record Employee(string Name, string Department, DateTime HireDate, decimal Salary, bool IsActive);

    private Employee? _selected;

    private readonly List<Employee> _employees = new()
    {
        new("Alice Chen",     "Engineering",  new DateTime(2019, 3, 12),  98_000m,  true),
        new("Bob Martinez",   "Marketing",    new DateTime(2021, 7, 1),   72_500m,  true),
        new("Carol Smith",    "Engineering",  new DateTime(2018, 11, 5),  115_000m, true),
        new("David Lee",      "HR",           new DateTime(2022, 2, 28),  61_000m,  false),
        new("Eva Patel",      "Engineering",  new DateTime(2020, 9, 14),  105_000m, true),
        new("Frank Nguyen",   "Finance",      new DateTime(2017, 5, 22),  88_000m,  true),
        new("Grace Johnson",  "Marketing",    new DateTime(2023, 1, 9),   67_000m,  true),
        new("Henry Brown",    "Finance",      new DateTime(2016, 8, 30),  92_000m,  false),
    };

    private List<Employee> _short => _employees.Take(4).ToList();

    private void OnEmployeeClicked(Employee emp)
    {
        _selected = _selected?.Name == emp.Name ? null : emp;
    }

    private const string _code = """
        @using CoreBlaze.Components.Grid

        <!-- Basic usage -->
        <SimpleGrid Items="_employees" Striped Hoverable Title="Employees">
            <Columns>
                <SimpleGridColumn Field="Name"       Title="Name" />
                <SimpleGridColumn Field="Department" Title="Department" />
                <SimpleGridColumn Field="HireDate"   Title="Hire date" Format="dd MMM yyyy" />
                <SimpleGridColumn Field="Salary"     Title="Salary" Align="GridAlign.Right" Format="C0" />
                <SimpleGridColumn Field="IsActive"   Title="Active" Sortable="false">
                    <Template Context="emp">
                        <span class="@(emp.IsActive ? "badge bg-success" : "badge bg-secondary")">
                            @(emp.IsActive ? "Active" : "Inactive")
                        </span>
                    </Template>
                </SimpleGridColumn>
            </Columns>
        </SimpleGrid>

        <!-- Row click -->
        <SimpleGrid Items="_employees" Hoverable OnRowClick="OnEmployeeClicked">
            <Columns>
                <SimpleGridColumn Field="Name"   Title="Name" />
                <SimpleGridColumn Field="Salary" Title="Salary" Align="GridAlign.Right" Format="C0" />
            </Columns>
        </SimpleGrid>

        <!-- Dense + bordered -->
        <SimpleGrid Items="_employees" Dense Bordered>
            <Columns>
                <SimpleGridColumn Field="Name"   Title="Name" />
                <SimpleGridColumn Field="Salary" Title="Salary" Align="GridAlign.Right" Format="C0" />
            </Columns>
        </SimpleGrid>
        """;
}
