namespace CoreBlaze.Components.Shared;

/// <summary>Visual animation style for <see cref="Spinner"/>.</summary>
public enum SpinnerVariant
{
    /// <summary>Classic single-ring rotation.</summary>
    Ring = 0,

    /// <summary>Concentric rings that expand outward and fade — radar / sonar effect.</summary>
    Pulse = 1,

    /// <summary>Concentric rings that breathe in and out with a staggered wave.</summary>
    Wave = 2,

    /// <summary>Single ring filled with a smooth conic gradient; supports custom <c>Colors</c>.</summary>
    Gradient = 3,
}
