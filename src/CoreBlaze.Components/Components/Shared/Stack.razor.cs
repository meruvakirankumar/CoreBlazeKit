using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Components.Shared;

/// <summary>
/// A flex container for stacking children vertically or horizontally with consistent spacing.
/// </summary>
public partial class Stack : ComponentBase
{
    /// <summary>Content.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Flex direction. Default: <c>Column</c>.</summary>
    [Parameter] public StackDirection Direction { get; set; } = StackDirection.Column;

    /// <summary>Gap between children (0–6 maps to CSS variables, default 3 ≈ 0.75 rem).</summary>
    [Parameter] public int Gap { get; set; } = 3;

    /// <summary>Flex align-items.</summary>
    [Parameter] public StackAlign Align { get; set; } = StackAlign.Stretch;

    /// <summary>Flex justify-content.</summary>
    [Parameter] public StackJustify Justify { get; set; } = StackJustify.Start;

    /// <summary>Allow children to wrap onto a new line.</summary>
    [Parameter] public bool Wrap { get; set; }

    /// <summary>Extra CSS classes on the root element.</summary>
    [Parameter] public string? CssClass { get; set; }

    private string _directionClass => Direction == StackDirection.Row ? "cb-stack--row" : "cb-stack--col";
    private string _gapClass => $"cb-stack--gap-{Math.Clamp(Gap, 0, 6)}";
    private string _alignClass => Align switch
    {
        StackAlign.Start   => "cb-stack--align-start",
        StackAlign.Center  => "cb-stack--align-center",
        StackAlign.End     => "cb-stack--align-end",
        StackAlign.Baseline => "cb-stack--align-baseline",
        _                  => string.Empty,
    };
    private string _justifyClass => Justify switch
    {
        StackJustify.Center       => "cb-stack--justify-center",
        StackJustify.End          => "cb-stack--justify-end",
        StackJustify.SpaceBetween => "cb-stack--justify-between",
        StackJustify.SpaceAround  => "cb-stack--justify-around",
        _                         => string.Empty,
    };
}

/// <summary>Flex direction for <see cref="Stack"/>.</summary>
public enum StackDirection { Column, Row }

/// <summary>Flex align-items value for <see cref="Stack"/>.</summary>
public enum StackAlign { Stretch, Start, Center, End, Baseline }

/// <summary>Flex justify-content value for <see cref="Stack"/>.</summary>
public enum StackJustify { Start, Center, End, SpaceBetween, SpaceAround }
