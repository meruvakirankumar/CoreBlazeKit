using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Demo.Components.Pages.Inputs;

public partial class TextInputDemo : ComponentBase
{
    private string? _name;
    private string? _email;
    private string? _password;
    private string? _url;
    private string? _lastEvent;
    private string? _errorText;  // clears the error example once the user types something

    private void OnNameChanged(string? v) => _lastEvent = $"Name changed to '{v}'";

    private const string _code = """
        @using CoreBlaze.Components.Inputs

        <!-- Playground: name, required, max length + change event -->
        <TextInput @bind-Value="_name"
                   Label="Name"
                   Placeholder="Type your name"
                   MaxLength="80"
                   Required="true"
                   OnChange="OnNameChanged" />

        <!-- Email — HTML5 type=email -->
        <TextInput @bind-Value="_email"
                   Label="Email"
                   InputType="email"
                   Placeholder="you@example.com" />

        <!-- Password — HTML5 type=password -->
        <TextInput @bind-Value="_password"
                   Label="Password"
                   InputType="password"
                   Placeholder="At least 6 characters" />

        <!-- URL — HTML5 type=url -->
        <TextInput @bind-Value="_url"
                   Label="Website"
                   InputType="url"
                   Placeholder="https://example.com" />

        <!-- Disabled — read-only value, no interaction -->
        <TextInput Label="Disabled example"
                   Value="You can't edit me"
                   Disabled="true" />

        <!-- Error state — external message rendered below the input -->
        <TextInput Label="Error example"
                   Value="Broken value"
                   ErrorMessage="Something went wrong." />

        @code {
            string? _name;
            string? _email;
            string? _password;
            string? _url;

            void OnNameChanged(string? v)
            {
                // use the new value — e.g. validate, filter, update state
            }
        }
        """;
}
