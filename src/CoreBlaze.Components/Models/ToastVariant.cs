namespace CoreBlaze.Components.Models;

/// <summary>Visual variant of a toast notification.</summary>
public enum ToastVariant
{
    /// <summary>Neutral / informational message.</summary>
    Info = 0,
    /// <summary>Successful operation confirmation.</summary>
    Success = 1,
    /// <summary>Non-blocking warning.</summary>
    Warning = 2,
    /// <summary>Error / failure notification.</summary>
    Error = 3,
}
