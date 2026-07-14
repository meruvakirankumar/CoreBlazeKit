namespace CoreBlaze.Components.Shared;

/// <summary>Where the <see cref="Toaster"/> anchor sits within the viewport.</summary>
public enum ToastPosition
{
    /// <summary>Top-right corner (default).</summary>
    TopRight = 0,
    /// <summary>Top-left corner.</summary>
    TopLeft = 1,
    /// <summary>Bottom-right corner.</summary>
    BottomRight = 2,
    /// <summary>Bottom-left corner.</summary>
    BottomLeft = 3,
    /// <summary>Top center.</summary>
    TopCenter = 4,
    /// <summary>Bottom center.</summary>
    BottomCenter = 5,
}
