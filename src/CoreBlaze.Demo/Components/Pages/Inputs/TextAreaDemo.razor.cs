using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Demo.Components.Pages.Inputs;

public partial class TextAreaDemo : ComponentBase
{
    private string? _bio;
    private string? _notes;
    private string? _errorText;  // clears the error once the user starts typing

    private const string _code = """
        @using CoreBlaze.Components.Inputs

        <!-- Playground: 4 rows, unlimited length -->
        <TextArea @bind-Value="_bio"
                  Label="Biography"
                  Placeholder="Tell us about yourself…"
                  Rows="4" />

        <!-- Short notes with a max-length cap -->
        <TextArea @bind-Value="_notes"
                  Label="Short notes (max 120 chars)"
                  Placeholder="Keep it brief"
                  Rows="2"
                  MaxLength="120" />

        <!-- Disabled — read-only multi-line content -->
        <TextArea Label="Disabled example"
                  Value="Existing multi-line content\nthat the user cannot edit."
                  Rows="3"
                  Disabled="true" />

        <!-- Required + error message -->
        <TextArea Label="Error example"
                  Value=""
                  Rows="3"
                  Required="true"
                  ErrorMessage="Please provide a description." />

        @code {
            string? _bio;
            string? _notes;
        }
        """;
}
