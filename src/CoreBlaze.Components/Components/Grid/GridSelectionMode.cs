namespace CoreBlaze.Components.Grid;

/// <summary>Row-selection behaviour for <see cref="DataGrid{T}"/>.</summary>
public enum GridSelectionMode
{
    /// <summary>No selection column is rendered.</summary>
    None = 0,

    /// <summary>Only one row can be selected at a time (radio button column).</summary>
    Single = 1,

    /// <summary>Any number of rows can be selected (checkbox column with select-all in the header).</summary>
    Multiple = 2,
}
