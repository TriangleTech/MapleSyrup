using MapleSyrup.Core.Event;

namespace MapleSyrup.Core;

/// <summary>
/// Primary interface of the MapleEngine.
/// </summary>
public interface IGameObject
{
    GameContext Context { get; }
    
    /// <summary>
    /// Subscribes to an event, when triggered it goes to the method attached.
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="eventHandler"></param>
    void SubscribeToEvent(EventType eventType, Action<object> eventHandler)
    {
        Context.RegisterEventHandler(eventType, eventHandler);
    }

    /// <summary>
    /// Triggers an event without data, typically used when working with UI.
    /// </summary>
    /// <param name="eventType">The event being triggered.</param>
    void SendEvent(EventType eventType)
    {
        Context.SendEvent(eventType);
    }

    /// <summary>
    /// Sends an event with a specific EventData.
    /// </summary>
    /// <param name="eventType">The event being triggered</param>
    /// <param name="eventData">The data structure being sent.</param>
    void SendEvent(EventType eventType, EventData eventData)
    {
        Context.SendEvent(eventType, eventData);
    }
}