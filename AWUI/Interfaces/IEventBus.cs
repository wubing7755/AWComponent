namespace AWUI.Interfaces;

/// <summary>
/// Defines a contract for event-based communication between components.
/// </summary>
public interface IEventBus
{
    void Publish<TEvent>(TEvent @event) where TEvent : class;

    /// <summary>
    /// Registers a handler for events of type <typeparamref name="T"/>.
    /// </summary>
    void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : class;

    /// <summary>
    /// Unregisters all event handlers from the specified subscriber.
    /// </summary>
    /// <param name="subscriber">Subscriber to remove</param>
    void UnsubscribeAll(object subscriber);

    /// <summary>
    /// Unsubscribes a previously registered event handler for the specified event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event to unsubscribe from.</typeparam>
    /// <param name="handler">The event handler delegate to remove.</param>
    void Unsubscribe<TEvent>(Action<TEvent> handler);
}
