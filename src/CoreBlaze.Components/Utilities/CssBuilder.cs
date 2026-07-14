using System.Text;

namespace CoreBlaze.Components.Utilities;

/// <summary>
/// A small, allocation-friendly fluent builder for composing CSS class strings.
/// </summary>
/// <example>
/// <code>
/// var css = CssBuilder.Default("cb-input")
///     .AddClass("cb-input--disabled", Disabled)
///     .AddClass(UserClass)
///     .Build();
/// </code>
/// </example>
public struct CssBuilder
{
    private readonly StringBuilder _sb;

    private CssBuilder(string? initial)
    {
        _sb = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(initial))
            _sb.Append(initial);
    }

    /// <summary>Creates a new builder, optionally seeded with a base class name.</summary>
    public static CssBuilder Default(string? baseClass = null) => new(baseClass);

    /// <summary>Creates a new empty builder.</summary>
    public static CssBuilder Empty() => new(null);

    /// <summary>Adds the given class if it is not null/whitespace.</summary>
    public CssBuilder AddClass(string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            if (_sb.Length > 0) _sb.Append(' ');
            _sb.Append(value);
        }
        return this;
    }

    /// <summary>Adds the given class when the condition is true.</summary>
    public CssBuilder AddClass(string? value, bool when) => when ? AddClass(value) : this;

    /// <summary>Adds the given class when the delegate returns true.</summary>
    public CssBuilder AddClass(string? value, Func<bool> when) => AddClass(value, when());

    /// <summary>Builds and returns the composed class string.</summary>
    public readonly string Build() => _sb?.ToString() ?? string.Empty;

    /// <inheritdoc />
    public override readonly string ToString() => Build();
}
