using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using CoreBlaze.Components.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace CoreBlaze.Components.Base;

/// <summary>
/// Abstract base class for every CoreBlaze input control.
/// <para>
/// Provides:
/// <list type="bullet">
///   <item>Two-way binding via <c>@bind-Value</c> (<see cref="Value"/> + <see cref="ValueChanged"/> + <see cref="ValueExpression"/>).</item>
///   <item>Automatic <see cref="EditContext"/> integration when hosted in an <c>EditForm</c>.</item>
///   <item>Label, placeholder, disabled, required, custom CSS and unmatched-attribute splatting.</item>
///   <item><see cref="OnChange"/>, <see cref="OnBlur"/>, <see cref="OnFocus"/> events.</item>
///   <item>Uniform validation-message surface (external <see cref="ErrorMessage"/>,
///         parse errors, and DataAnnotations from the surrounding form).</item>
/// </list>
/// </para>
/// </summary>
public abstract class BaseInputComponent<TValue> : ComponentBase, IDisposable
{
    private bool _hasInitializedParameters;
    private EditContext? _previousEditContext;
    private Expression<Func<TValue?>>? _previousValueExpression;

    /// <summary>Cascaded <see cref="EditContext"/> from an <c>EditForm</c>. May be null.</summary>
    [CascadingParameter] protected EditContext? EditContext { get; set; }

    /// <summary>The bound value. Use <c>@bind-Value</c> from the consuming component.</summary>
    [Parameter] public TValue? Value { get; set; }

    /// <summary>Event callback invoked when the value changes (paired with <see cref="Value"/>).</summary>
    [Parameter] public EventCallback<TValue?> ValueChanged { get; set; }

    /// <summary>Expression identifying the bound field (auto-populated by <c>@bind-Value</c>).</summary>
    [Parameter] public Expression<Func<TValue?>>? ValueExpression { get; set; }

    /// <summary>Text rendered above / beside the input as a &lt;label&gt;.</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>HTML placeholder attribute.</summary>
    [Parameter] public string? Placeholder { get; set; }

    /// <summary>Disables user interaction.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>Marks the field as required (adds required styling + <c>aria-required</c>).</summary>
    [Parameter] public bool Required { get; set; }

    /// <summary>Extra CSS classes appended to the root element.</summary>
    [Parameter] public string? CssClass { get; set; }

    /// <summary>Explicit error message. Displayed alongside any validation-context messages.</summary>
    [Parameter] public string? ErrorMessage { get; set; }

    /// <summary>Fires after the value has changed and callbacks have been invoked.</summary>
    [Parameter] public EventCallback<TValue?> OnChange { get; set; }

    /// <summary>Fires when the input loses focus.</summary>
    [Parameter] public EventCallback<FocusEventArgs> OnBlur { get; set; }

    /// <summary>Fires when the input receives focus.</summary>
    [Parameter] public EventCallback<FocusEventArgs> OnFocus { get; set; }

    /// <summary>Captures any extra HTML attributes the caller wants to splat onto the input element.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>The <see cref="FieldIdentifier"/> derived from <see cref="ValueExpression"/>.</summary>
    protected FieldIdentifier FieldIdentifier { get; private set; }

    /// <summary>A stable HTML id used by the input and its associated label.</summary>
    protected string InputId { get; } = $"cb-{Guid.NewGuid():N}";

    /// <summary>Optional culture used for parsing / formatting. Defaults to <see cref="CultureInfo.CurrentCulture"/>.</summary>
    protected virtual CultureInfo Culture => CultureInfo.CurrentCulture;

    /// <summary>Set by <see cref="CurrentValueAsString"/> when a parse fails.</summary>
    protected string? ParsingErrorMessage { get; private set; }

    /// <summary>
    /// The current value as an object convenience wrapper. Assigning triggers
    /// change notification (see <see cref="SetValueAsync(TValue?)"/>).
    /// </summary>
    protected TValue? CurrentValue
    {
        get => Value;
        set => _ = SetValueAsync(value);
    }

    /// <summary>String-facing counterpart of <see cref="CurrentValue"/> used by text-based inputs.</summary>
    protected string? CurrentValueAsString
    {
        get => FormatValueAsString(Value);
        set
        {
            if (TryParseValueFromString(value, out var parsed, out var error))
            {
                ParsingErrorMessage = null;
                _ = SetValueAsync(parsed);
            }
            else
            {
                ParsingErrorMessage = error;
                if (EditContext is not null && FieldIdentifier.Model is not null)
                    EditContext.NotifyValidationStateChanged();
            }
        }
    }

    /// <summary>True when at least one message would be rendered in the error area.</summary>
    protected bool HasValidationErrors =>
        !string.IsNullOrEmpty(ErrorMessage)
        || !string.IsNullOrEmpty(ParsingErrorMessage)
        || (EditContext is not null
            && FieldIdentifier.Model is not null
            && EditContext.GetValidationMessages(FieldIdentifier).Any());

    /// <summary>All messages currently applicable to the field (external, parse, and DataAnnotations).</summary>
    protected IEnumerable<string> ValidationMessages
    {
        get
        {
            if (!string.IsNullOrEmpty(ErrorMessage)) yield return ErrorMessage!;
            if (!string.IsNullOrEmpty(ParsingErrorMessage)) yield return ParsingErrorMessage!;
            if (EditContext is not null && FieldIdentifier.Model is not null)
            {
                foreach (var m in EditContext.GetValidationMessages(FieldIdentifier))
                    yield return m;
            }
        }
    }

    /// <summary>The first validation message, or <see langword="null"/> if none.</summary>
    protected string? FirstValidationMessage => ValidationMessages.FirstOrDefault();

    /// <summary>The BEM root class name — override in derived components.</summary>
    protected virtual string BaseCssClass => "cb-input";

    /// <summary>Composed CSS class for the root element (disabled / invalid / required modifiers + user classes).</summary>
    protected string RootCssClass => CssBuilder.Default(BaseCssClass)
        .AddClass($"{BaseCssClass}--disabled", Disabled)
        .AddClass($"{BaseCssClass}--invalid", HasValidationErrors)
        .AddClass($"{BaseCssClass}--required", Required)
        .AddClass(CssClass)
        .Build();

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (!_hasInitializedParameters)
        {
            if (ValueExpression is not null)
                FieldIdentifier = FieldIdentifier.Create(ValueExpression);
            _previousEditContext = EditContext;
            _previousValueExpression = ValueExpression;
            _hasInitializedParameters = true;
        }
        else
        {
            if (!ReferenceEquals(ValueExpression, _previousValueExpression))
            {
                FieldIdentifier = ValueExpression is null ? default : FieldIdentifier.Create(ValueExpression);
                _previousValueExpression = ValueExpression;
            }
            if (EditContext != _previousEditContext) _previousEditContext = EditContext;
        }
    }

    /// <summary>Formats a value for text-based rendering. Override for custom formatting.</summary>
    protected virtual string? FormatValueAsString(TValue? value)
        => value is IFormattable f ? f.ToString(null, Culture) : value?.ToString();

    /// <summary>
    /// Parses a user-typed string into <typeparamref name="TValue"/>. The default implementation
    /// uses <see cref="Convert.ChangeType(object?, Type, IFormatProvider?)"/> on the underlying type
    /// (respecting <see cref="Nullable{T}"/>). Override for richer parsing.
    /// </summary>
    protected virtual bool TryParseValueFromString(
        string? value,
        [MaybeNullWhen(false)] out TValue result,
        [NotNullWhen(false)] out string? validationErrorMessage)
    {
        try
        {
            if (string.IsNullOrEmpty(value))
            {
                result = default!;
                validationErrorMessage = null;
                return true;
            }

            var target = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
            if (target == typeof(string))
            {
                result = (TValue)(object)value;
                validationErrorMessage = null;
                return true;
            }

            var converted = Convert.ChangeType(value, target, Culture);
            result = (TValue?)converted!;
            validationErrorMessage = null;
            return true;
        }
        catch
        {
            result = default!;
            validationErrorMessage = $"The value '{value}' is not valid for {typeof(TValue).Name}.";
            return false;
        }
    }

    /// <summary>
    /// Central mutation point. Skips no-op writes, notifies the EditContext, invokes
    /// <see cref="ValueChanged"/> and finally <see cref="OnChange"/>.
    /// </summary>
    protected virtual async Task SetValueAsync(TValue? newValue)
    {
        if (EqualityComparer<TValue?>.Default.Equals(Value, newValue)) return;
        Value = newValue;
        await ValueChanged.InvokeAsync(newValue);
        if (EditContext is not null && FieldIdentifier.Model is not null)
            EditContext.NotifyFieldChanged(FieldIdentifier);
        await OnChange.InvokeAsync(newValue);
    }

    /// <summary>Convenience handler that wires up to <c>@onblur</c>.</summary>
    protected Task HandleBlurAsync(FocusEventArgs args) => OnBlur.InvokeAsync(args);

    /// <summary>Convenience handler that wires up to <c>@onfocus</c>.</summary>
    protected Task HandleFocusAsync(FocusEventArgs args) => OnFocus.InvokeAsync(args);

    /// <inheritdoc />
    public virtual void Dispose() => GC.SuppressFinalize(this);
}
