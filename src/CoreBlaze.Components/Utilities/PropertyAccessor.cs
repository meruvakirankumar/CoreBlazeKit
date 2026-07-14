using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace CoreBlaze.Components.Utilities;

/// <summary>
/// Compiled, cached property getter/setter delegates used by the DataGrid
/// for reflection-heavy operations (sorting, filtering, editing).
/// </summary>
public static class PropertyAccessor
{
    private sealed record Accessor(
        Func<object, object?> Getter,
        Action<object, object?>? Setter,
        Type PropertyType,
        PropertyInfo PropertyInfo);

    private static readonly ConcurrentDictionary<(Type Type, string Name), Accessor?> _cache = new();

    /// <summary>Returns the accessor for the given property, or <see langword="null"/> if it does not exist.</summary>
    public static (Func<object, object?> Getter, Action<object, object?>? Setter, Type PropertyType, PropertyInfo PropertyInfo)? TryFor(Type type, string propertyName)
    {
        var accessor = _cache.GetOrAdd((type, propertyName), key => Build(key.Type, key.Name));
        return accessor is null ? null : (accessor.Getter, accessor.Setter, accessor.PropertyType, accessor.PropertyInfo);
    }

    /// <summary>Returns the accessor for the given property, throwing if it does not exist.</summary>
    public static (Func<object, object?> Getter, Action<object, object?>? Setter, Type PropertyType, PropertyInfo PropertyInfo) For(Type type, string propertyName)
        => TryFor(type, propertyName) ?? throw new InvalidOperationException($"Property '{propertyName}' was not found on type '{type.FullName}'.");

    /// <summary>Reads the value of the named property from the given instance.</summary>
    public static object? GetValue(object instance, string propertyName)
        => For(instance.GetType(), propertyName).Getter(instance);

    /// <summary>Writes the value of the named property on the given instance.</summary>
    public static void SetValue(object instance, string propertyName, object? value)
    {
        var accessor = For(instance.GetType(), propertyName);
        if (accessor.Setter is null)
            throw new InvalidOperationException($"Property '{propertyName}' on '{instance.GetType().FullName}' has no public setter.");
        accessor.Setter(instance, value);
    }

    private static Accessor? Build(Type type, string propertyName)
    {
        var prop = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        if (prop is null) return null;

        var target = Expression.Parameter(typeof(object), "target");
        var value = Expression.Parameter(typeof(object), "value");
        var castTarget = Expression.Convert(target, type);

        var getterBody = Expression.Convert(Expression.Property(castTarget, prop), typeof(object));
        var getter = Expression.Lambda<Func<object, object?>>(getterBody, target).Compile();

        Action<object, object?>? setter = null;
        if (prop.CanWrite && prop.SetMethod is { IsPublic: true })
        {
            var castValue = Expression.Convert(value, prop.PropertyType);
            var setterBody = Expression.Assign(Expression.Property(castTarget, prop), castValue);
            setter = Expression.Lambda<Action<object, object?>>(setterBody, target, value).Compile();
        }

        return new Accessor(getter, setter, prop.PropertyType, prop);
    }
}
