namespace CoreBlaze.Components.Models;

/// <summary>A single toast notification managed by <c>ToastService</c>.</summary>
public sealed class Toast
{
    /// <summary>Stable identifier — used for dismiss/lookup.</summary>
    public string Id { get; } = Guid.NewGuid().ToString("N");

    /// <summary>Visual variant.</summary>
    public ToastVariant Variant { get; init; } = ToastVariant.Info;

    /// <summary>Optional bold title above the message.</summary>
    public string? Title { get; init; }

    /// <summary>Body text of the toast.</summary>
    public required string Message { get; init; }

    /// <summary>How long the toast remains visible. <see langword="null"/> means it stays until dismissed.</summary>
    public TimeSpan? Duration { get; init; } = TimeSpan.FromSeconds(4);

    /// <summary>Whether the user can dismiss the toast with an "×" button.</summary>
    public bool Dismissible { get; init; } = true;

    /// <summary>UTC timestamp when the toast was created.</summary>
    public DateTime CreatedUtc { get; } = DateTime.UtcNow;
}
