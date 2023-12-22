using MapleSyrup.Core;
using MapleSyrup.Core.Event;

namespace MapleSyrup.Subsystems;

public class EventSystem : ISubsystem
{
    public GameContext Context { get; private set; }
    private readonly Dictionary<EventType, Dictionary<object, Action<EventData>>> subscribers = new();

    public void Initialize(GameContext context)
    {
        Context = context;
    }
    
    public void Subscribe(object sender, EventType eventType, Action<EventData> callback)
    {
        if (!subscribers.ContainsKey(eventType))
        {
            subscribers.Add(eventType, new());
        }
        
        if (!subscribers[eventType].ContainsKey(sender))
        {
            subscribers[eventType].Add(sender, callback);
        }
    }
    
    public void Unsubscribe(object sender, EventType eventType)
    {
        if (!subscribers.ContainsKey(eventType))
            return;
        if (!subscribers[eventType].ContainsKey(sender))
            return;
        subscribers[eventType].Remove(sender);
    }
    
    public void Publish(EventType eventType)
    {
        if (!subscribers.ContainsKey(eventType))
            return;
        foreach (var subscriber in subscribers[eventType])
        {
            subscriber.Value.Invoke(null);
        }
    }
    
    public void Publish(EventType eventType, EventData eventData)
    {
        if (!subscribers.ContainsKey(eventType))
            return;
        foreach (var subscriber in subscribers[eventType])
        {
            subscriber.Value.Invoke(eventData);
        }
    }
    
    public void UnsubscribeAll(object sender)
    {
        foreach (var eventType in subscribers.Keys)
        {
            if (subscribers[eventType].ContainsKey(sender))
            {
                subscribers[eventType].Remove(sender);
            }
        }
    }
    
    public void Shutdown()
    {
        foreach (var (eventType, _) in subscribers)
        {
            subscribers[eventType].Clear();
        }
        subscribers.Clear();
    }
}