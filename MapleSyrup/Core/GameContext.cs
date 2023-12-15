using MapleSyrup.Core.Event;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Core;

/// <summary>
/// The spine of the entire engine, without this nothing will work.
/// </summary>
public class GameContext
{
    private List<ISubsystem> subsystems;
    public readonly GraphicsDevice GraphicsDevice;
    private readonly Dictionary<EventType, List<Subscriber>> subscribers;
    private readonly List<EventType> publishedEvents;
    
    public GameContext(Game game)
    {
        GraphicsDevice = game.GraphicsDevice;
        subsystems = new List<ISubsystem>();
        subscribers = new();
        publishedEvents = new();
    }
    
    public void AddSubsystem<T>() where T : ISubsystem, new()
    {
        var subsystem = new T();
        subsystem.Initialize(this);
        subsystems.Add(subsystem);
    }
    
    public void RemoveSubsystem<T>() where T : ISubsystem
    {
        var subsystem = subsystems.Find(system => system is T);
        subsystem.Shutdown();
        subsystems.Remove(subsystem);
    }
    
    public T GetSubsystem<T>() where T : ISubsystem
    {
        return (T) subsystems.Find(system => system is T);
    }
    
    public void SubscribeToEvent(EventType eventType, Subscriber subscriber)
    {
        if (!subscribers.ContainsKey(eventType))
        {
            subscribers.Add(eventType, new List<Subscriber>());
        }
        subscribers[eventType].Add(subscriber);
    }
    
    public void SubscribeToSpecificEvent(EventType eventType, object sender, object receiver, Action<EventData> eventHandler)
    {
        throw new NotImplementedException();
    }
    
    public void UnsubscribeFromEvent(EventType eventType, Subscriber subscriber)
    {
        if (!subscribers.ContainsKey(eventType))
        {
            return;
        }
        subscribers[eventType].Remove(subscriber);
    }
    
    public void RegisterEvent(EventType eventType)
    {
        if (publishedEvents.Contains(eventType))
            return;
        publishedEvents.Add(eventType);
    }
    
    public void UnregisterEvent(EventType eventType)
    {
        if (!publishedEvents.Contains(eventType))
            return;
        publishedEvents.Remove(eventType);
    }
    
    public void PublishEvent(EventType eventType)
    {
        if (!publishedEvents.Contains(eventType))
            return;
        if (!subscribers.ContainsKey(eventType))
            return;
        subscribers[eventType].ForEach(subscriber =>
        {
            subscriber.Event?.Invoke(null);
        });
    }
    
    public void PublishEvent(EventType eventType, EventData eventData)
    {
        if (!publishedEvents.Contains(eventType))
            return;
        if (!subscribers.ContainsKey(eventType))
            return;
        subscribers[eventType].ForEach(subscriber =>
        {
            subscriber.Event?.Invoke(eventData);
        });
    }
    
    public void PublishToOne(EventType eventType, EventData eventData)
    {
        throw new NotImplementedException();
    }
    
    public void PublishToAll(EventData eventData)
    {
        foreach (var pubEvent in publishedEvents)
        {
            if (!subscribers.ContainsKey(pubEvent))
                continue;
            subscribers[pubEvent].ForEach(subscriber =>
            {
                subscriber.Event?.Invoke(eventData);
            });
        }
    }
    
    public void Shutdown()
    {
        subsystems.ForEach(system => system.Shutdown());
        GC.Collect();
    }
}