using FluentValidation;
using Inventory.Api.Authentication;
using Inventory.Api.HostedServices;
using Inventory.Application.Contracts;
using Inventory.Application.Dto;
using Inventory.Application.Mappers.Items;
using Inventory.Application.Services;
using Inventory.Application.Validators;
using Inventory.CrossCutting.Cqrs.Commands;
using Inventory.CrossCutting.Cqrs.Queries;
using Inventory.CrossCutting.Events;
using Inventory.CrossCutting.Exceptions;
using Inventory.Domain.Items;
using Inventory.Infrastructure.Commands;
using Inventory.Infrastructure.Events;
using Inventory.Infrastructure.Persistence;
using Inventory.Infrastructure.Queries;
using Inventory.Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Basic Authorization header using the Bearer scheme."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "basic"
                }
            },
            Array.Empty<string>()
        }
    });
});

services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
services.AddAuthorization();
services.AddHostedService<ExpiredItemsNotificatorHostedService>();

services.AddSingleton<IEventBus, EventBus>();

services.AddSingleton<InventoryInMemoryContext>();
services.AddTransient<IItemRepository, ItemInMemoryRepository>();

services.AddSingleton<IQueryDispatcher, QueryDispatcher>();
services.AddSingleton<ICommandDispatcher, CommandDispatcher>();

services.AddTransient<IQueryHandler<GetAllItemsQuery, IEnumerable<Item>>, GetAllItemsQueryHandler>();
services.AddTransient<IQueryHandler<GetItemsByExpirationDateQuery, IEnumerable<Item>>, GetItemsByExpirationDateQueryHandler>();
services.AddTransient<ICommandHandler<AddItemCommand>, AddItemCommandHandler>();
services.AddTransient<ICommandHandler<RemoveItemByNameCommand>, RemoveItemByNameCommandHandler>();

services.AddTransient<AbstractValidator<ItemDto>, ItemValidator>();

services.AddTransient<IItemMapper, ItemMapper>();
services.AddTransient<IItemReadService, ItemReadService>();
services.AddTransient<IItemWriteService, ItemWriteService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionHandler>();

// Subscribe to events
using (var scope = app.Services.CreateScope())
{
    var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
    eventBus.Subscribe<ItemRemovedEvent>(payload =>
    {
        Console.WriteLine("Item with name {0} has been removed at {1}", payload.Event.Name, payload.Timestamp);
    });
    eventBus.Subscribe<ItemExpiredEvent>(payload =>
    {
        Console.WriteLine("Item with name {0} has expired at {1}", payload.Event.Name, payload.Event.ExpirationDate);
    });
}

app.Run();