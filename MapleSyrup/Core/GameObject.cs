using MapleSyrup.Core.Event;

namespace MapleSyrup.Core;

/// <summary>
/// Primary interface of the MapleEngine.
/// </summary>
public abstract class GameObject
{
    protected GameContext Context { get; }

    public GameObject(GameContext context)
    {
        Context = context;
    }
    
    public void SubscribeToEvent(EventType eventType, Subscriber subscriber)
    {
        Context.SubscribeToEvent(eventType, subscriber);
    }
    
    public void SubscribeToSpecificEvent(EventType eventType, object sender, object receiver, Action<EventData> eventHandler)
    {
        Context.SubscribeToSpecificEvent(eventType, sender, receiver, eventHandler);
    }
    
    public void UnsubscribeFromEvent(EventType eventType, Subscriber subscriber)
    {
        Context.UnsubscribeFromEvent(eventType, subscriber);
    }
    
    public void RegisterEvent(EventType eventType)
    {
        Context.RegisterEvent(eventType);
    }
    
    public void UnregisterEvent(EventType eventType)
    {
        Context.UnregisterEvent(eventType);
    }
    
    public void PublishEvent(EventType eventType, EventData eventData)
    {
        Context.PublishEvent(eventType, eventData);
    }
    
    public void PublishToOne(EventType eventType, EventData eventData)
    {
        Context.PublishToOne(eventType, eventData);
    }
}