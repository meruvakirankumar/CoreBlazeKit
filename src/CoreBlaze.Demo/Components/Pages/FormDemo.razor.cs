using CoreBlaze.Demo.Models;
using CoreBlaze.Demo.Services;
using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Demo.Components.Pages;

public partial class FormDemo : ComponentBase
{
    [Inject] private PersonRepository Repo { get; set; } = default!;

    private Person _person = new() { HireDate = DateTime.Today };
    private bool _submitted;

    private void OnValidSubmit() => _submitted = true;

    private void Reset()
    {
        _person = new Person { HireDate = DateTime.Today };
        _submitted = false;
    }

    private const string _code = """
        @using System.ComponentModel.DataAnnotations
        @using CoreBlaze.Components.Inputs

        <EditForm Model="_person" OnValidSubmit="OnValidSubmit" class="row g-3">
            <DataAnnotationsValidator />

            <!-- Text input, required -->
            <div class="col-md-6">
                <TextInput @bind-Value="_person.Name" Label="Name"
                           Placeholder="Full name" Required="true" />
            </div>

            <!-- Numeric input, bounded -->
            <div class="col-md-6">
                <NumberInput TNumber="int" @bind-Value="_person.Age"
                             Label="Age" Min="0" Max="130" />
            </div>

            <!-- Single-select dropdown, required -->
            <div class="col-md-6">
                <Dropdown TItem="string" TValue="string"
                          Items="_departments"
                          ValueSelector="x => x"
                          TextSelector="x => x"
                          @bind-Value="_person.Department"
                          Label="Department"
                          Placeholder="Pick one…"
                          Required="true" />
            </div>

            <!-- Date picker -->
            <div class="col-md-6">
                <DatePicker @bind-Value="_person.HireDate" Label="Hire date" />
            </div>

            <!-- Plain checkbox -->
            <div class="col-12">
                <div class="form-check">
                    <input type="checkbox" class="form-check-input" id="active" @bind="_person.IsActive" />
                    <label class="form-check-label" for="active">Currently active</label>
                </div>
            </div>

            <!-- Submit / reset -->
            <div class="col-12">
                <button type="submit" class="btn btn-primary">Submit</button>
                <button type="button" class="btn btn-outline-secondary" @onclick="Reset">Reset</button>
            </div>
        </EditForm>

        @code {
            class Person
            {
                [Required(ErrorMessage = "Name is required.")]
                [StringLength(80, MinimumLength = 2, ErrorMessage = "Name must be 2–80 characters.")]
                public string? Name { get; set; }

                [Range(0, 130, ErrorMessage = "Age must be between 0 and 130.")]
                public int Age { get; set; }

                [Required(ErrorMessage = "Please pick a department.")]
                public string? Department { get; set; }

                public DateTime? HireDate { get; set; }
                public bool IsActive { get; set; } = true;
            }

            Person _person = new() { HireDate = DateTime.Today };
            readonly string[] _departments = { "Engineering", "Sales", "Marketing", "Finance", "Support" };

            void OnValidSubmit() { /* persist / call API */ }
            void Reset() => _person = new() { HireDate = DateTime.Today };
        }
        """;
}
