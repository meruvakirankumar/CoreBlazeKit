using CoreBlaze.Demo.Services;
using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Demo.Components.Pages.Inputs;

public partial class DropdownDemo : ComponentBase
{
    [Inject] private PersonRepository Repo { get; set; } = default!;

    private string? _department;
    private int _priorityLevel = 2;
    private string? _errorDept;  // error clears once an option is picked

    public record Priority(int Level, string Name);

    private readonly Priority[] _priorities =
    {
        new(1, "Low"),
        new(2, "Normal"),
        new(3, "High"),
        new(4, "Critical")
    };

    private static string PriorityText(Priority p) => $"{p.Level} — {p.Name}";

    private const string _code = """
        @using CoreBlaze.Components.Inputs

        <!-- Playground — simple string list, TItem == TValue -->
        <Dropdown TItem="string" TValue="string"
                  Items="_departments"
                  ValueSelector="x => x"
                  TextSelector="x => x"
                  @bind-Value="_department"
                  Label="Department"
                  Placeholder="Pick one…" />

        <!-- Object list with a distinct value type (Priority → int) -->
        <Dropdown TItem="Priority" TValue="int"
                  Items="_priorities"
                  ValueSelector="p => p.Level"
                  TextSelector="PriorityText"
                  @bind-Value="_priorityLevel"
                  Label="Priority"
                  Placeholder="Select priority…" />

        <!-- Disabled with a preset value -->
        <Dropdown TItem="string" TValue="string"
                  Items="_departments"
                  ValueSelector="x => x"
                  TextSelector="x => x"
                  Value="@("Engineering")"
                  Label="Disabled example"
                  Disabled="true" />

        <!-- Required + error message -->
        <Dropdown TItem="string" TValue="string"
                  Items="_departments"
                  ValueSelector="x => x"
                  TextSelector="x => x"
                  Label="Error example"
                  Placeholder="Choose…"
                  ErrorMessage="Please pick a department." />

        @code {
            string? _department;
            int _priorityLevel = 2;

            readonly string[] _departments = { "Engineering", "Sales", "Support" };

            record Priority(int Level, string Name);
            readonly Priority[] _priorities =
            {
                new(1, "Low"), new(2, "Normal"),
                new(3, "High"), new(4, "Critical")
            };

            static string PriorityText(Priority p) => $"{p.Level} — {p.Name}";
        }
        """;
}
