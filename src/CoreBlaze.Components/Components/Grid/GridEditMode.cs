namespace CoreBlaze.Components.Grid;

/// <summary>Editing mode for <see cref="DataGrid{T}"/>.</summary>
public enum GridEditMode
{
    /// <summary>All cells in a row become editable when Edit is clicked.</summary>
    Row = 0,
    /// <summary>Only the double-clicked cell becomes editable.</summary>
    Cell = 1,
}
