using CoreBlaze.Components.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CoreBlaze.Components;

/// <summary>
/// DI extension methods that register CoreBlaze services with the host application.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers CoreBlaze services (theme, validation) as singletons.
    /// Call this from <c>Program.cs</c> of the host application:
    /// <code>builder.Services.AddCoreBlazeComponents();</code>
    /// </summary>
    public static IServiceCollection AddCoreBlazeComponents(this IServiceCollection services)
    {
        services.AddSingleton<ThemeService>();
        services.AddSingleton<ValidationService>();
        services.AddScoped<ToastService>();
        return services;
    }
}
