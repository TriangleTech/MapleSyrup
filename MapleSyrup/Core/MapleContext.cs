using MapleSyrup.Core.Event;
using MapleSyrup.Subsystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Core;

public class MapleContext
{
    private List<ISubsystem> subsystems;
    private Dictionary<EventType, List<Action<EventData>>> eventHandlers;
    
    public readonly GraphicsDevice GraphicsDevice;
    
    public MapleContext(Game game)
    {
        GraphicsDevice = game.GraphicsDevice;
        subsystems = new();
        eventHandlers = new();
    }

    public void AddSubsystem<T>() where T : ISubsystem
    {
        var system = Activator.CreateInstance<T>();
        system.Initialize(this);
        subsystems.Add(system);
    }

    public T GetSubsystem<T>()
    {
        return (T)subsystems.Find(x => x is T)! ?? throw new NullReferenceException();
    }

    public void RegisterEventHandler(EventType eventType, Action<EventData> eventHandler)
    {
        if (!eventHandlers.ContainsKey(eventType))
            eventHandlers.Add(eventType, new());
        eventHandlers[eventType].Add(eventHandler);
    }

    public void SendEvent(EventType eventType)
    {
        if (eventHandlers.TryGetValue(eventType, out var events))
            events.ForEach(handler => handler?.Invoke(null!));
    }

    public void SendEvent(EventType eventType, EventData eventData)
    {
        if (eventHandlers.TryGetValue(eventType, out var events))
            events.ForEach(handler => handler?.Invoke(eventData));
    }

    public void SendToOne(object sender, object receiver, EventType eventType)
    {
        throw new NotImplementedException();
    }
}