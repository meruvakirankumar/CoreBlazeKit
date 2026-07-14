using CoreBlaze.Components.Grid;
using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Demo.Components.Pages;

public partial class EditableGridDemo : ComponentBase
{
    // ── Employee model ──────────────────────────────────────────────────────

    public class Employee
    {
        public string Name       { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public DateTime HireDate { get; set; } = DateTime.Today;
        public decimal Salary    { get; set; }
        public bool IsActive     { get; set; } = true;
    }

    // ── Product model ───────────────────────────────────────────────────────

    public class Product
    {
        public string  Name     { get; set; } = string.Empty;
        public string  Category { get; set; } = string.Empty;
        public decimal Price    { get; set; }
        public int     Qty      { get; set; }
        public bool    InStock  { get; set; } = true;
    }

    // ── Data ────────────────────────────────────────────────────────────────

    private IList<Employee> _employees = new List<Employee>
    {
        new() { Name = "Alice Chen",    Department = "Engineering", HireDate = new DateTime(2019, 3, 12), Salary = 98_000m, IsActive = true  },
        new() { Name = "Bob Martinez",  Department = "Marketing",   HireDate = new DateTime(2021, 7, 1),  Salary = 72_500m, IsActive = true  },
        new() { Name = "Carol Smith",   Department = "Engineering", HireDate = new DateTime(2018, 11, 5), Salary = 115_000m, IsActive = true },
        new() { Name = "David Lee",     Department = "HR",          HireDate = new DateTime(2022, 2, 28), Salary = 61_000m, IsActive = false },
        new() { Name = "Eva Patel",     Department = "Finance",     HireDate = new DateTime(2020, 9, 14), Salary = 88_500m, IsActive = true  },
    };

    private IList<Product> _products = new List<Product>
    {
        new() { Name = "Wireless Mouse",   Category = "Electronics", Price = 29.99m,  Qty = 120, InStock = true  },
        new() { Name = "Mechanical Keyboard", Category = "Electronics", Price = 79.99m, Qty = 45, InStock = true },
        new() { Name = "USB-C Hub",        Category = "Electronics", Price = 49.99m,  Qty = 0,   InStock = false },
        new() { Name = "Desk Mat XL",      Category = "Accessories", Price = 24.99m,  Qty = 80,  InStock = true  },
        new() { Name = "Monitor Stand",    Category = "Furniture",   Price = 59.99m,  Qty = 30,  InStock = true  },
    };

    private readonly IReadOnlyList<string> _departments =
        new[] { "Engineering", "Marketing", "HR", "Finance", "Design", "Sales" };

    private readonly IReadOnlyList<string> _categories =
        new[] { "Electronics", "Accessories", "Furniture", "General" };

    // ── Event handlers ──────────────────────────────────────────────────────

    private string? _lastEvent;

    private void OnEmployeeChanged(Employee emp)
    {
        _lastEvent = $"✏  Changed: {emp.Name} ({emp.Department}) — Salary {emp.Salary:C0}";
    }

    private void OnRowAdded(Employee emp)
    {
        _lastEvent = $"+ New row added";
    }

    private void OnRowDeleted(Employee emp)
    {
        _lastEvent = $"✕  Deleted: {emp.Name}";
    }

    private Product NewProduct() =>
        new Product { Name = string.Empty, Category = "General", Price = 0, Qty = 0, InStock = true };

    // ── Usage code sample ───────────────────────────────────────────────────

    private const string _code = """
        @using CoreBlaze.Components.Grid

        <!-- Minimal usage -->
        <EditableGrid T="Employee" @bind-Items="_employees">
            <EditableGridColumn T="Employee" Field="Name"   Title="Name" />
            <EditableGridColumn T="Employee" Field="Salary" Title="Salary" Align="GridAlign.Right" />
        </EditableGrid>

        <!-- Dropdown column via Options -->
        <EditableGridColumn T="Employee" Field="Department" Title="Dept"
                            Options="@(new[]{"Eng","HR","Finance"})" />

        <!-- Read-only column -->
        <EditableGridColumn T="Employee" Field="Id" Title="ID" Readonly="true" />

        <!-- Custom cell template (badge) -->
        <EditableGridColumn T="Employee" Field="IsActive" Title="Status">
            <Template Context="emp">
                <span class="badge @(emp.IsActive ? "bg-success" : "bg-secondary")">
                    @(emp.IsActive ? "Active" : "Inactive")
                </span>
            </Template>
        </EditableGridColumn>

        <!-- Row factory for Add row -->
        <EditableGrid T="Product" @bind-Items="_products"
                      NewRowFactory="() => new Product { Category = \"General\" }"
                      OnChange="OnProductChanged"
                      OnRowAdd="OnProductAdded"
                      OnRowDelete="OnProductDeleted">
            ...
        </EditableGrid>
        """;
}
