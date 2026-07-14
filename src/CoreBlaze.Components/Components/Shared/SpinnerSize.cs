namespace CoreBlaze.Components.Shared;

/// <summary>Size preset for <see cref="Spinner"/>.</summary>
public enum SpinnerSize
{
    /// <summary>8 px — tiny indicator inside dense UI (chips, badges).</summary>
    XSmall = 0,
    /// <summary>16 px — inline use next to text or inside buttons.</summary>
    Small = 1,
    /// <summary>32 px — default standalone use.</summary>
    Medium = 2,
    /// <summary>48 px — card / section loading states.</summary>
    Large = 3,
    /// <summary>72 px — page-level loading.</summary>
    XLarge = 4,
    /// <summary>96 px — full-page hero loading states.</summary>
    XXLarge = 5,
}
