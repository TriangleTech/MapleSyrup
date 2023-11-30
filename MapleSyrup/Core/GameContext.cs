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
    private Dictionary<EventType, List<Action<EventData>>> eventHandlers;
    public readonly GraphicsDevice GraphicsDevice;
    
    public GameContext(Game game)
    {
        GraphicsDevice = game.GraphicsDevice;
        subsystems = new();
        eventHandlers = new();
    }

    /// <summary>
    /// Adds a subsystem and initializes it. Subsystems have a copy of the context as well.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void AddSubsystem<T>() where T : ISubsystem
    {
        var system = Activator.CreateInstance<T>();
        system.Initialize(this);
        subsystems.Add(system);
    }

    /// <summary>
    /// Gets a specific subsystem from the list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public T GetSubsystem<T>()
    {
        return (T)subsystems.Find(x => x is T);
    }

    /// <summary>
    /// Register an event and its handler (the method). 
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="eventHandler"></param>
    public void RegisterEventHandler(EventType eventType, Action<EventData> eventHandler)
    {
        if (!eventHandlers.ContainsKey(eventType))
            eventHandlers.Add(eventType, new());
        eventHandlers[eventType].Add(eventHandler);
    }

    /// <summary>
    /// Triggers an event and notifies all subscribers.
    /// </summary>
    /// <param name="eventType"></param>
    public void SendEvent(EventType eventType)
    {
        if (eventHandlers.TryGetValue(eventType, out var events))
            events.ForEach(handler => handler?.Invoke(null!));
    }

    /// <summary>
    /// Triggers an events, notifies and sends all subscribers the data.
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="eventData"></param>
    public void SendEvent(EventType eventType, EventData eventData)
    {
        if (eventHandlers.TryGetValue(eventType, out var events))
            events.ForEach(handler => handler?.Invoke(eventData));
    }

    public void SendToOne(object sender, object receiver, EventType eventType)
    {
        throw new NotImplementedException();
    }

    public void Shutdown()
    {
        subsystems.ForEach(system => system.Shutdown());
        eventHandlers.Clear();
        GC.Collect();
    }
}