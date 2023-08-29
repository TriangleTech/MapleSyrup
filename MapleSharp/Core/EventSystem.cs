using MapleSharp.Core.Interface;

namespace MapleSharp.Core;

public class EventSystem : ISubsystem
{
    private Dictionary<string, Action> datalessEvents = new();
    private Dictionary<string, Action<object>> dataEvents = new();
    private Queue<string> datalessQueue = new();
    private Queue<(string, object)> dataQueue = new();
    
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
                datalessEvents[eventName].Invoke();
            }
        });
        
        Task.Run(() =>
        {
            if (dataQueue.Count > 0)
            {
                var (eventName, data) = dataQueue.Dequeue();
                dataEvents[eventName].Invoke(data);
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

    public void RegisterEvent(string eventName, Action callback)
    {
        if (datalessEvents.ContainsKey(eventName))
        {
            Console.WriteLine($"[EventSystem] Event {eventName} already exists.");
            return;
        }
        
        datalessEvents.Add(eventName, callback);
    }
    
    public void RegisterEvent(string eventName, Action<object> callback)
    {
        if (dataEvents.ContainsKey(eventName))
        {
            Console.WriteLine($"[EventSystem] Event {eventName} already exists.");
            return;
        }
        
        dataEvents.Add(eventName, callback);
    }

    public void QueueEvent(string eventName)
    {
        if (!datalessEvents.ContainsKey(eventName))
        {
            Console.WriteLine($"[EventSystem] Event {eventName} does not exist.");
            return;
        }
        
        datalessQueue.Enqueue(eventName);
    }
    
    public void QueueEvent(string eventName, object data)
    {
        if (!dataEvents.ContainsKey(eventName))
        {
            Console.WriteLine($"[EventSystem] Event {eventName} does not exist.");
            return;
        }
        
        dataQueue.Enqueue((eventName, data));
    }
}