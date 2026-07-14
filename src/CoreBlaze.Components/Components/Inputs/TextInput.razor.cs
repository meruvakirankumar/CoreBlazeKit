using CoreBlaze.Components.Base;
using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Components.Inputs;

/// <summary>
/// A single-line text input. Reference implementation of the input pattern:
/// inherits <see cref="BaseInputComponent{TValue}"/>, exposes a couple of
/// text-specific parameters, and renders label + control + validation messages.
/// </summary>
/// <example>
/// <code>
/// &lt;TextInput @bind-Value="model.Name"
///             Label="Full name"
///             Placeholder="Type your name"
///             MaxLength="80"
///             OnChange="OnNameChanged" /&gt;
/// </code>
/// </example>
public partial class TextInput : BaseInputComponent<string?>
{
    /// <summary>HTML input <c>type</c>. Defaults to <c>text</c> — set to <c>password</c>, <c>email</c> etc.</summary>
    [Parameter] public string InputType { get; set; } = "text";

    /// <summary>Optional maximum length. 0 means unlimited (attribute omitted).</summary>
    [Parameter] public int? MaxLength { get; set; }

    /// <inheritdoc />
    protected override string BaseCssClass => "cb-input";
}
