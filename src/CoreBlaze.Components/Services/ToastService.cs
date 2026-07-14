using CoreBlaze.Components.Models;

namespace CoreBlaze.Components.Services;

/// <summary>
/// Global notification service. Consumers inject <see cref="ToastService"/> and
/// call <see cref="Info(string, string?, TimeSpan?)"/> / <see cref="Success"/> /
/// <see cref="Warning"/> / <see cref="Error"/>. A <c>Toaster</c> component
/// somewhere in the layout subscribes to <see cref="Changed"/> and renders the
/// current toast queue.
/// </summary>
public sealed class ToastService
{
    private readonly List<Toast> _toasts = new();
    private readonly object _lock = new();

    /// <summary>Fires whenever the toast list changes (added or removed).</summary>
    public event Action? Changed;

    /// <summary>The current toast queue (most recent last).</summary>
    public IReadOnlyList<Toast> Toasts
    {
        get { lock (_lock) return _toasts.ToArray(); }
    }

    /// <summary>Show a pre-built toast.</summary>
    public void Show(Toast toast)
    {
        lock (_lock) _toasts.Add(toast);
        Changed?.Invoke();

        if (toast.Duration is { } d && d > TimeSpan.Zero)
        {
            _ = ScheduleDismissAsync(toast.Id, d);
        }
    }

    /// <summary>Show an <see cref="ToastVariant.Info"/> toast.</summary>
    public void Info(string message, string? title = null, TimeSpan? duration = null)
        => Show(new Toast { Variant = ToastVariant.Info, Message = message, Title = title, Duration = duration ?? TimeSpan.FromSeconds(4) });

    /// <summary>Show a <see cref="ToastVariant.Success"/> toast.</summary>
    public void Success(string message, string? title = null, TimeSpan? duration = null)
        => Show(new Toast { Variant = ToastVariant.Success, Message = message, Title = title, Duration = duration ?? TimeSpan.FromSeconds(4) });

    /// <summary>Show a <see cref="ToastVariant.Warning"/> toast.</summary>
    public void Warning(string message, string? title = null, TimeSpan? duration = null)
        => Show(new Toast { Variant = ToastVariant.Warning, Message = message, Title = title, Duration = duration ?? TimeSpan.FromSeconds(5) });

    /// <summary>Show a <see cref="ToastVariant.Error"/> toast (default: no auto-dismiss).</summary>
    public void Error(string message, string? title = null, TimeSpan? duration = null)
        => Show(new Toast { Variant = ToastVariant.Error, Message = message, Title = title, Duration = duration ?? TimeSpan.FromSeconds(6) });

    /// <summary>Dismiss the toast with the given id.</summary>
    public void Dismiss(string id)
    {
        bool changed;
        lock (_lock) changed = _toasts.RemoveAll(t => t.Id == id) > 0;
        if (changed) Changed?.Invoke();
    }

    /// <summary>Dismiss every toast.</summary>
    public void Clear()
    {
        bool changed;
        lock (_lock)
        {
            changed = _toasts.Count > 0;
            _toasts.Clear();
        }
        if (changed) Changed?.Invoke();
    }

    private async Task ScheduleDismissAsync(string id, TimeSpan delay)
    {
        try { await Task.Delay(delay); } catch { return; }
        Dismiss(id);
    }
}
