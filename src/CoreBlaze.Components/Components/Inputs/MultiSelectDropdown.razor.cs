using CoreBlaze.Components.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace CoreBlaze.Components.Inputs;

/// <summary>
/// A dropdown-style multi-select. Selected values are rendered in the trigger
/// (as chips or a text summary), and the checkbox list only appears when the
/// dropdown is open. The bound value is an <see cref="IReadOnlyList{TValue}"/>.
/// </summary>
public partial class MultiSelectDropdown<TItem, TValue> : BaseInputComponent<IReadOnlyList<TValue>>
{
    private bool _isOpen;

    /// <summary>The options to render.</summary>
    [Parameter, EditorRequired] public IEnumerable<TItem>? Items { get; set; }

    /// <summary>Function mapping an item to the bound value.</summary>
    [Parameter, EditorRequired] public Func<TItem, TValue> ValueSelector { get; set; } = default!;

    /// <summary>Function mapping an item to its display text.</summary>
    [Parameter, EditorRequired] public Func<TItem, string> TextSelector { get; set; } = default!;

    /// <summary>How selected values render inside the trigger. Default: <see cref="MultiSelectDisplayMode.Chips"/>.</summary>
    [Parameter] public MultiSelectDisplayMode DisplayMode { get; set; } = MultiSelectDisplayMode.Chips;

    /// <summary>When <see langword="true"/>, the panel shows Select-all / Clear buttons.</summary>
    [Parameter] public bool ShowSelectAll { get; set; } = true;

    /// <inheritdoc />
    protected override string BaseCssClass => "cb-multiselect";

    private bool IsSelected(TValue value)
        => Value is not null && Value.Any(v => EqualityComparer<TValue>.Default.Equals(v, value));

    private string LookupText(TValue value)
    {
        if (Items is null) return value?.ToString() ?? string.Empty;
        var item = Items.FirstOrDefault(i => EqualityComparer<TValue>.Default.Equals(ValueSelector(i), value));
        return item is null ? value?.ToString() ?? string.Empty : TextSelector(item);
    }

    private void ToggleOpen()
    {
        if (Disabled) return;
        _isOpen = !_isOpen;
    }

    private void Close() => _isOpen = false;

    private Task OnTriggerKeyDown(KeyboardEventArgs args)
    {
        if (Disabled) return Task.CompletedTask;
        switch (args.Key)
        {
            case "Enter":
            case " ":
            case "ArrowDown":
                _isOpen = true;
                break;
            case "Escape":
                _isOpen = false;
                break;
        }
        return Task.CompletedTask;
    }

    private async Task OnToggleAsync(TValue value, bool selected)
    {
        var current = Value?.ToList() ?? new List<TValue>();
        if (selected)
        {
            if (!current.Any(v => EqualityComparer<TValue>.Default.Equals(v, value)))
                current.Add(value);
        }
        else
        {
            current.RemoveAll(v => EqualityComparer<TValue>.Default.Equals(v, value));
        }
        await SetValueAsync(current);
    }

    private async Task SelectAllAsync()
    {
        if (Items is null) return;
        await SetValueAsync(Items.Select(ValueSelector).ToList());
    }

    private Task ClearAsync() => SetValueAsync(new List<TValue>());
}
