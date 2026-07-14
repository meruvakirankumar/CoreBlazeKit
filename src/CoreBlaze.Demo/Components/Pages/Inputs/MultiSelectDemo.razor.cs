using CoreBlaze.Demo.Services;
using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Demo.Components.Pages.Inputs;

public partial class MultiSelectDemo : ComponentBase
{
    [Inject] private PersonRepository Repo { get; set; } = default!;

    private IReadOnlyList<string>? _selected;
    private IReadOnlyList<string>? _selected2;
    private readonly IReadOnlyList<string> _disabledValue = new[] { "Engineering", "Support" };

    private const string _code = """
        @using CoreBlaze.Components.Inputs

        <!-- Chip mode (default) — selections shown as removable pills -->
        <MultiSelectDropdown TItem="string" TValue="string"
                             Items="_departments"
                             ValueSelector="x => x"
                             TextSelector="x => x"
                             @bind-Value="_selected"
                             Label="Departments visited (chips)"
                             Placeholder="Select departments…"
                             DisplayMode="MultiSelectDisplayMode.Chips" />

        <!-- Text-summary mode — comma-separated string, same underlying value -->
        <MultiSelectDropdown TItem="string" TValue="string"
                             Items="_departments"
                             ValueSelector="x => x"
                             TextSelector="x => x"
                             @bind-Value="_selected"
                             Label="Departments visited (text)"
                             DisplayMode="MultiSelectDisplayMode.Text" />

        <!-- Disabled with a preset selection -->
        <MultiSelectDropdown TItem="string" TValue="string"
                             Items="_departments"
                             ValueSelector="x => x"
                             TextSelector="x => x"
                             Value="_preset"
                             Label="Disabled example"
                             Disabled="true" />

        <!-- Without the Select-all / Clear toolbar -->
        <MultiSelectDropdown TItem="string" TValue="string"
                             Items="_departments"
                             ValueSelector="x => x"
                             TextSelector="x => x"
                             @bind-Value="_selected2"
                             Label="No Select-all toolbar"
                             Placeholder="Pick some…"
                             ShowSelectAll="false" />

        @code {
            IReadOnlyList<string>? _selected;
            IReadOnlyList<string>? _selected2;
            readonly IReadOnlyList<string> _preset = new[] { "Engineering", "Support" };
            readonly string[] _departments = { "Engineering", "Sales", "Marketing", "Finance", "Support" };
        }
        """;
}
