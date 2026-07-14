namespace CoreBlaze.Components.Models;

/// <summary>Supported filter operators for the DataGrid filter row.</summary>
public enum FilterOperator
{
    /// <summary>String contains (case-insensitive).</summary>
    Contains,
    /// <summary>Equal to.</summary>
    Equals,
    /// <summary>Not equal to.</summary>
    NotEquals,
    /// <summary>Starts with (strings).</summary>
    StartsWith,
    /// <summary>Ends with (strings).</summary>
    EndsWith,
    /// <summary>Greater than.</summary>
    GreaterThan,
    /// <summary>Greater than or equal.</summary>
    GreaterThanOrEqual,
    /// <summary>Less than.</summary>
    LessThan,
    /// <summary>Less than or equal.</summary>
    LessThanOrEqual,
}

/// <summary>Describes a filter applied to a single column.</summary>
public sealed class FilterDescriptor
{
    /// <summary>The bound property name of the filtered column.</summary>
    public string Field { get; init; } = string.Empty;

    /// <summary>The operator to apply.</summary>
    public FilterOperator Operator { get; init; } = FilterOperator.Contains;

    /// <summary>The value to filter against (may be null).</summary>
    public object? Value { get; init; }
}
