namespace CoreBlaze.Components.Inputs;

/// <summary>How <see cref="MultiSelectDropdown{TItem,TValue}"/> displays selected values in its trigger.</summary>
public enum MultiSelectDisplayMode
{
    /// <summary>Each selected value renders as a removable chip inside the trigger.</summary>
    Chips = 0,

    /// <summary>Selected values are joined into a single comma-separated text summary.</summary>
    Text = 1,
}
