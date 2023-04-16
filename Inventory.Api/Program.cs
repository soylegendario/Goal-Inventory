using Inventory.Api;
using Inventory.Application;
using Inventory.CrossCutting.Events;
using Inventory.CrossCutting.Exceptions;
using Inventory.Infrastructure;
using Inventory.Infrastructure.Events;
using Inventory.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = new ConfigurationBuilder()
    .AddJsonFile("AppSettings.json", optional: false).Build();

services
    .AddApiServices()
    .AddEventBus()
    .AddInfrastructureServices(configuration)
    .AddApplicationServices();

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
    var context = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
    context.Database.EnsureCreated();
    
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