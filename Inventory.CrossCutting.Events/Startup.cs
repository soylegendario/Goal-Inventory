using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;

namespace Inventory.CrossCutting.Events;

public static class Startup
{
    public static IServiceCollection AddEventBus(this IServiceCollection services, string? connectionString)
    {
        // https://code-maze.com/rebus-dotnet/
        services.AddRebus(config => config
            .Logging(l => l.ColoredConsole())
            .Transport(t => t.UseRabbitMqAsOneWayClient(connectionString)));
        
        services.AddSingleton<IEventBus, EventBus>();
        return services;
    }
}