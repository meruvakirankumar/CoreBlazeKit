using System.ComponentModel.DataAnnotations;

namespace CoreBlaze.Components.Services;

/// <summary>
/// Small helpers around <see cref="Validator"/> so components / view-models
/// can trigger DataAnnotations validation outside of an <c>EditForm</c>.
/// Bonus scaffold — extend to plug in FluentValidation or custom rule engines.
/// </summary>
public sealed class ValidationService
{
    /// <summary>
    /// Validates all DataAnnotations on the given instance.
    /// Returns <see langword="true"/> when the object is valid.
    /// </summary>
    public bool TryValidate(object instance, out IReadOnlyList<ValidationResult> results)
    {
        var context = new ValidationContext(instance);
        var list = new List<ValidationResult>();
        var ok = Validator.TryValidateObject(instance, context, list, validateAllProperties: true);
        results = list;
        return ok;
    }
}
