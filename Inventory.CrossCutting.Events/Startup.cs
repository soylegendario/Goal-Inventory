using Microsoft.Extensions.DependencyInjection;

namespace Inventory.CrossCutting.Events;

public static class Startup
{
    public static IServiceCollection AddEventBus(this IServiceCollection services)
    {
        services.AddSingleton<IEventBus, EventBus>();
        return services;
    }
}