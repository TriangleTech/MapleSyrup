using MapleSyrup.Core.Event;

namespace MapleSyrup.Core;

public interface IMapleObject
{
    MapleContext Context { get; }
    
    void SubscribeToEvent(EventType eventType, Action<object> eventHandler)
    {
        Context.RegisterEventHandler(eventType, eventHandler);
    }

    void SendEvent(EventType eventType)
    {
        Context.SendEvent(eventType);
    }

    void SendEvent(EventType eventType, EventData eventData)
    {
        Context.SendEvent(eventType, eventData);
    }
}