using SharedLibrary.Components;
using System.Collections.Concurrent;

namespace SharedLibrary.Events;

public class EventBus : IEventBus
{
    private readonly ConcurrentDictionary<Type, List<Delegate>> _handlers = new();

    // 发布事件
    public void Publish<TEvent>(TEvent @event) where TEvent : class
    {
        if (_handlers.TryGetValue(typeof(TEvent), out var handlers))
        {
            foreach (var handler in handlers.Cast<Action<TEvent>>())
            {
                handler(@event);
            }
        }
    }

    // 订阅事件
    public void Subscribe<T>(Action<T> handler)
    {
        var handlers = _handlers.GetOrAdd(typeof(T), _ => new List<Delegate>());
        handlers.Add(handler);
    }

    // 取消订阅
    public void UnsubscribeAll(object subscriber)
    {
        foreach (var handlerList in _handlers.Values)
        {
            handlerList.RemoveAll(h => h.Target == subscriber);
        }
    }
}
