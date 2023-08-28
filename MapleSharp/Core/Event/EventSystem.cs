using MapleSharp.Core.Interface;

namespace MapleSharp.Core.Event;

public class EventSystem : ISubsystem
{
    private readonly Dictionary<string, EventSubscriber> subscribers = new();
    private readonly Queue<EventData> eventQueue = new();

    public EventSystem()
    {
        
    }
    
    public void Initialize()
    {
    }

    public void Update(float timeDelta)
    {
        Task.Factory.StartNew(() =>
        {
            if (eventQueue.Count > 0)
            {
                var data = eventQueue.Dequeue();

                if (data.Sender == null)
                {
                    foreach (var subscriber in subscribers.Where(x => x.Key == data.EventName))
                    {
                        subscriber.Value.Callback?.Invoke(data);
                        data.IsDirty = true;
                    }
                }
                else if (data.Sender != null)
                {
                    foreach (var subscriber in subscribers.Where(x => x.Key == data.EventName && x.Value.Sender == data.Sender))
                    {
                        subscriber.Value.Callback?.Invoke(data);
                    }
                }
                else
                {
                    Console.WriteLine($"[EventSystem] Attempted to send event {data.EventName} that does not exist.");
                }
            }
        });
    }

    public void Shutdown()
    {
    }
    
    public void SubscribeToEvent(string eventName, object receiver, Action<EventData> callback)
    {
        SubscribeToEvent(eventName, null, receiver, callback);
    }
    
    public void SubscribeToEvent(string eventName, object sender, object receiver, Action<EventData> callback)
    {
        if (subscribers.ContainsKey(eventName))
            throw new Exception("[EventSystem] Attempted to subscribe to an event that exist.");
        
        subscribers.Add(eventName, new EventSubscriber(eventName, sender, receiver, callback));
        Console.WriteLine($"[EventSystem] Subscribed to event {eventName}.");
    }
    
    public void SendEvent(EventData data)
    {
        if (subscribers.ContainsKey(data.EventName))
        {
            eventQueue.Enqueue(data);
        }
        else
        {
            Console.WriteLine($"[EventSystem] Attempted to send event {data.EventName} that does not exist.");
        }
    }
}