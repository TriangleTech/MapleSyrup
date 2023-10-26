using MapleSyrup.Core.Interface;

namespace MapleSyrup.Core;

public class EventSystem : ISubsystem
{
    private readonly Dictionary<string, List<Action>> datalessEvents = new();
    private readonly Dictionary<string, List<Action<object>>> dataEvents = new();
    private readonly Queue<string> datalessQueue = new();
    private readonly Queue<(string, object)> dataQueue = new();
    
    public void Initialize()
    {
        
    }

    public void Update(float timeDelta)
    {
        Task.Run(() =>
        {
            if (datalessQueue.Count > 0)
            {
                var eventName = datalessQueue.Dequeue();
                foreach (var callback in datalessEvents[eventName])
                {
                    callback?.Invoke();
                }
            }
        });
        
        Task.Run(() =>
        {
            if (dataQueue.Count > 0)
            {
                var (eventName, data) = dataQueue.Dequeue();
                foreach (var callback in dataEvents[eventName])
                {
                    callback?.Invoke(data);
                }
            }
        });
    }

    public void Shutdown()
    {
        datalessEvents.Clear();
        dataEvents.Clear();
        datalessQueue.Clear();
        dataQueue.Clear();
    }

    public void ListenForEvent(string eventName, Action callback)
    {
        if (datalessEvents.TryGetValue(eventName, value: out var @event))
        {
            @event.Add(callback);
            return;
        }
        
        datalessEvents.Add(eventName, new List<Action>() { callback });
    }
    
    public void ListenForEvent(string eventName, Action<object> callback)
    {
        if (dataEvents.TryGetValue(eventName, out var @event))
        {
            @event.Add(callback);
            return;
        }
        
        dataEvents.Add(eventName, new List<Action<object>>() { callback });
    }

    public void TriggerEvent(string eventName)
    {
        if (!datalessEvents.ContainsKey(eventName))
        {
            Console.WriteLine($"[EventSystem] Event {eventName} does not exist.");
            return;
        }
        
        datalessQueue.Enqueue(eventName);
    }
    
    public void TriggerEvent(string eventName, object data)
    {
        if (!dataEvents.ContainsKey(eventName))
        {
            Console.WriteLine($"[EventSystem] Event {eventName} does not exist.");
            return;
        }
        
        dataQueue.Enqueue((eventName, data));
    }
}