using System.Diagnostics.CodeAnalysis;
using CoreBlaze.Components.Base;
using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Components.Inputs;

/// <summary>A numeric input generic over the numeric type (e.g. <c>int</c>, <c>decimal</c>, <c>double?</c>).</summary>
public partial class NumberInput<TNumber> : BaseInputComponent<TNumber>
{
    /// <summary>Increment step (HTML <c>step</c> attribute).</summary>
    [Parameter] public string? Step { get; set; } = "any";

    /// <summary>Minimum value (HTML <c>min</c> attribute).</summary>
    [Parameter] public string? Min { get; set; }

    /// <summary>Maximum value (HTML <c>max</c> attribute).</summary>
    [Parameter] public string? Max { get; set; }

    /// <summary>
    /// Optional adornment rendered inside the control before the value —
    /// e.g. a currency symbol (<c>"$"</c>, <c>"€"</c>) or a unit.
    /// </summary>
    [Parameter] public string? Prefix { get; set; }

    /// <summary>
    /// Optional adornment rendered inside the control after the value —
    /// e.g. <c>"%"</c>, <c>"kg"</c>, or a currency code.
    /// </summary>
    [Parameter] public string? Suffix { get; set; }

    /// <summary>
    /// When set, parsed values are rounded to this many digits after the decimal
    /// point (using <see cref="MidpointRounding.AwayFromZero"/>). Applies only to
    /// <see cref="decimal"/>, <see cref="double"/>, and <see cref="float"/> — plus
    /// their nullable counterparts.
    /// </summary>
    [Parameter] public int? DecimalPlaces { get; set; }

    private bool HasAdornments => !string.IsNullOrEmpty(Prefix) || !string.IsNullOrEmpty(Suffix);

    /// <inheritdoc />
    protected override bool TryParseValueFromString(
        string? value,
        [MaybeNullWhen(false)] out TNumber result,
        [NotNullWhen(false)] out string? validationErrorMessage)
    {
        var ok = base.TryParseValueFromString(value, out result, out validationErrorMessage);
        if (ok && DecimalPlaces is int dp && result is not null)
        {
            result = RoundIfFractional(result, dp);
        }
        return ok;
    }

    private static TNumber RoundIfFractional(TNumber value, int digits)
    {
        var underlying = Nullable.GetUnderlyingType(typeof(TNumber)) ?? typeof(TNumber);

        if (underlying == typeof(decimal))
            return (TNumber)(object)Math.Round((decimal)(object)value!, digits, MidpointRounding.AwayFromZero);
        if (underlying == typeof(double))
            return (TNumber)(object)Math.Round((double)(object)value!, digits, MidpointRounding.AwayFromZero);
        if (underlying == typeof(float))
            return (TNumber)(object)(float)Math.Round((float)(object)value!, digits, MidpointRounding.AwayFromZero);

        return value;
    }
}


