namespace CoreBlaze.Components.Models;

/// <summary>
/// Represents the persistence state of a row tracked by <c>DataGrid&lt;T&gt;</c>.
/// </summary>
public enum RowState
{
    /// <summary>Row is unchanged.</summary>
    Unchanged = 0,
    /// <summary>Row was added in the current session and not yet persisted.</summary>
    Added = 1,
    /// <summary>Row has pending edits.</summary>
    Modified = 2,
    /// <summary>Row was marked for deletion.</summary>
    Deleted = 3,
}
