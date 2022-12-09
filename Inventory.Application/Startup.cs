using FluentValidation;
using Inventory.Application.Contracts;
using Inventory.Application.Dto;
using Inventory.Application.Mappers.Items;
using Inventory.Application.Services;
using Inventory.Application.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Application;

public static class Startup
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IValidator<ItemDto>, ItemValidator>();
        services.AddTransient<IItemMapper, ItemMapper>();
        services.AddTransient<IItemReadService, ItemReadService>();
        services.AddTransient<IItemWriteService, ItemWriteService>();

        return services;
    }
}