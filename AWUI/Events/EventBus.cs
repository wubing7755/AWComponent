using System.Collections.Concurrent;
using AWUI.Interfaces;

namespace AWUI.Events;

public class EventBus : IEventBus
{
    private readonly ConcurrentDictionary<Type, List<Delegate>> _handlers = new();

    /// <summary>
    /// 订阅事件 => 注册事件监听器
    /// </summary>
    /// <typeparam name="T">事件的类型</typeparam>
    /// <param name="handler">处理事件的回调</param>
    public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : class
    {
        var handlers = _handlers.GetOrAdd(typeof(TEvent), _ => new List<Delegate>());
        handlers.Add(handler);
    }

    /// <summary>
    /// 发布事件 => 发布某种类型的事件，触发所有已订阅者的回调
    /// </summary>
    /// <typeparam name="TEvent">事件对象的类型</typeparam>
    /// <param name="event">事件实例 => 只支持引用类型</param>
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

    /// <summary>
    /// 取消订阅 => 取消某个订阅者注册的所有事件处理器
    /// </summary>
    /// <param name="subscriber">类实例</param>
    public void UnsubscribeAll(object subscriber)
    {
        foreach (var handlerList in _handlers.Values)
        {
            handlerList.RemoveAll(h => h.Target == subscriber);
        }
    }

    /// <summary>
    /// 取消订阅 => 取消某个特定事件类型的订阅
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <param name="handler">处理事件的回调</param>
    public void Unsubscribe<TEvent>(Action<TEvent> handler)
    {
        if(_handlers.TryGetValue(typeof(TEvent), out var handlers))
        {
            handlers.Remove(handler);
        }
    }
}
