using CoreBlaze.Components.Base;
using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Components.Inputs;

/// <summary>
/// A single-select dropdown generic over the item type and the bound value type.
/// Callers supply <see cref="ValueSelector"/> (how to get the value from an item)
/// and <see cref="TextSelector"/> (how to display it).
/// </summary>
public partial class Dropdown<TItem, TValue> : BaseInputComponent<TValue>
{
    /// <summary>The options to render.</summary>
    [Parameter, EditorRequired] public IEnumerable<TItem>? Items { get; set; }

    /// <summary>Function mapping an item to the bound value.</summary>
    [Parameter, EditorRequired] public Func<TItem, TValue?> ValueSelector { get; set; } = default!;

    /// <summary>Function mapping an item to its display text.</summary>
    [Parameter, EditorRequired] public Func<TItem, string> TextSelector { get; set; } = default!;

    private async Task OnSelectChangedAsync(ChangeEventArgs e)
    {
        var text = e.Value?.ToString();
        if (string.IsNullOrEmpty(text))
        {
            await SetValueAsync(default);
            return;
        }

        if (Items is null) return;
        foreach (var item in Items)
        {
            var v = ValueSelector(item);
            if (string.Equals(v?.ToString(), text, StringComparison.Ordinal))
            {
                await SetValueAsync(v);
                return;
            }
        }
    }
}
