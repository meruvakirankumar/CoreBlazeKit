namespace CoreBlaze.Components.Models;

/// <summary>Direction for a sort operation.</summary>
public enum GridSortDirection
{
    /// <summary>No sort applied.</summary>
    None = 0,
    /// <summary>Ascending order.</summary>
    Ascending = 1,
    /// <summary>Descending order.</summary>
    Descending = 2,
}

/// <summary>Describes a single column's sort state.</summary>
public sealed class SortDescriptor
{
    /// <summary>The bound property name of the sorted column.</summary>
    public string Field { get; init; } = string.Empty;

    /// <summary>The direction of the sort.</summary>
    public GridSortDirection Direction { get; init; } = GridSortDirection.Ascending;
}
