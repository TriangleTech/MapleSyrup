namespace MapleSharp.Events.Experimental;

public class EventFactory
{
    private readonly Dictionary<EventType, IEventHandler> registeredEvents = new();

    public EventFactory()
    {
        
    }

    public void RegisterEvent(IEventHandler handler)
    {
        if (registeredEvents.ContainsKey(handler.Type))
            return;
        registeredEvents.Add(handler.Type, handler);
    }

    public void RegisterEvent(EventType type, IEventHandler handler)
    {
        if (registeredEvents.ContainsKey(type))
            return;
        registeredEvents.Add(type, handler);
    }
    
    public void UnregisterEvent(EventType type)
    {
        if (registeredEvents.ContainsKey(type))
            registeredEvents.Remove(type);
        
        throw new Exception("[UnregisterEvent] Event does not exist.");
    }
    
    public void InvokeEvent<T>(EventType type, T arg)
    {
        if (!registeredEvents.ContainsKey(type))
            throw new Exception("[InvokeEvent] Event does not exist.");
        var handler = registeredEvents[type];
        handler.OnDispatchData(arg);
    }
    
    public T InvokeEvent<T>(EventType type, object arg)
    {
        if (!registeredEvents.ContainsKey(type))
            throw new Exception("[InvokeEvent] Event does not exist.");
        var handler = registeredEvents[type];
        return (T)handler.OnRequestData(arg);
    }
}