namespace Inventory.Infrastructure.Helpers.Events;

public interface IEventBus : IDisposable
{
    void Publish<T>(T message);
    void Subscribe<T>(Action<EventPayload<T>> subscription);
}