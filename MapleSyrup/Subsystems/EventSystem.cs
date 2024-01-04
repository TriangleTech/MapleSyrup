using MapleSyrup.Core;
using MapleSyrup.Core.Event;

namespace MapleSyrup.Subsystems;

public class EventSystem : ISubsystem
{
    public GameContext Context { get; private set; }
    private readonly Dictionary<string, Dictionary<object, Action<EventData>>> subscribers = new();

    public void Initialize(GameContext context)
    {
        Context = context;
    }
    
    public void Subscribe(object sender, string triggerEvent, Action<EventData> callback)
    {
        if (!subscribers.ContainsKey(triggerEvent))
        {
            subscribers.Add(triggerEvent, new());
        }
        
        if (!subscribers[triggerEvent].ContainsKey(sender))
        {
            subscribers[triggerEvent].Add(sender, callback);
        }
    }
    
    public void Unsubscribe(object sender, string triggerEvent)
    {
        if (!subscribers.ContainsKey(triggerEvent))
            return;
        if (!subscribers[triggerEvent].ContainsKey(sender))
            return;
        subscribers[triggerEvent].Remove(sender);
    }
    
    public void Publish(string triggerEvent)
    {
        if (!subscribers.ContainsKey(triggerEvent))
            return;
        foreach (var subscriber in subscribers[triggerEvent])
        {
            subscriber.Value.Invoke(new EventData());
        }
    }
    
    public void Publish(string triggerEvent, EventData eventData)
    {
        if (!subscribers.ContainsKey(triggerEvent))
            return;
        foreach (var subscriber in subscribers[triggerEvent])
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