namespace Inventory.CrossCutting.Events;

internal class EventBus : IEventBus
{
    
    private readonly Dictionary<Type, List<Delegate>> _subscribers;

    public EventBus()
    {
        _subscribers = new Dictionary<Type, List<Delegate>>();
    }

    public void Publish<T>(T message)
    {
        if (!_subscribers.TryGetValue(typeof(T), out var delegates)
            || delegates.Count == 0)
        {
            return;
        }

        var payload = new EventPayload<T>(message);
        foreach(var handler in delegates.Select(item => item as Action<EventPayload<T>>))
        {
            Task.Factory.StartNew(() => handler?.Invoke(payload));
        }    
    }
    
    public void Subscribe<T>(Action<EventPayload<T>> subscription)
    {
        var delegates = _subscribers.ContainsKey(typeof(T)) 
            ? _subscribers[typeof(T)] 
            : new List<Delegate>();
        if(!delegates.Contains(subscription))
        {
            delegates.Add(subscription);
        }
        _subscribers[typeof(T)] = delegates;    
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _subscribers.Clear();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}