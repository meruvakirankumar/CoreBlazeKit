namespace CoreBlaze.Components.Grid;

/// <summary>Event arguments raised when a single cell enters edit mode.</summary>
public sealed class GridCellEditEventArgs<T>
{
    /// <summary>The row item whose cell is being edited.</summary>
    public T Item { get; }

    /// <summary>The bound field name of the column being edited.</summary>
    public string? Field { get; }

    /// <summary>Creates a new instance.</summary>
    public GridCellEditEventArgs(T item, string? field)
    {
        Item = item;
        Field = field;
    }
}
