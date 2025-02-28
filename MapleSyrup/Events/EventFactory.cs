using System.Collections.Concurrent;
using MapleSyrup.ECS;
using MapleSyrup.Events.Enums;

namespace MapleSyrup.Events;

public class EventFactory
{
    private readonly ConcurrentQueue<EventMessage> _events;
    
    public static EventFactory Shared { get; private set; }

    public EventFactory()
    {
        _events = new ConcurrentQueue<EventMessage>();
        Shared = this;
    }
    
    public void BroadcastEvent(EventType eventType)
    {
        
    }

    public void BroadcastEvent(int senderId, EventType eventType)
    {
        
    }

    public void BroadcastToOne(int senderId, int receiverId, EventType eventType)
    {
        
    }

    public void ProcessEvents()
    {
        lock (_events)
        {
            while (_events.Count > 0)
            {
                _events.TryDequeue(out var eventMessage);
            }
        }
    }
}