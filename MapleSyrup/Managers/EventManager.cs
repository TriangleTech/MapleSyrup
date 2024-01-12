using MapleSyrup.EC;
using MapleSyrup.Event;

namespace MapleSyrup.Managers;

/// <summary>
/// Handles the registration of EventListeners & dispatches events.
/// </summary>
public class EventManager : IManager
{
    private ManagerLocator? _locator;
    private object eventLock = new();
    private List<IEventListener> _eventListeners;
    private Queue<EventFlag> _eventQueue;
    private Queue<(EventFlag flag, IEntity entity)> _specificEvent;

    public EventManager()
    {
        _eventListeners = new();
        _eventQueue = new();
        _specificEvent = new();
    }
    
    public void Initialize(ManagerLocator locator)
    {
        _locator = locator;
    }

    public void Register(IEventListener listener)
    {
        if (_eventListeners.Contains(listener))
            return;
        _eventListeners.Add(listener);
    }

    public void Dispatch(EventFlag flag)
    {
        lock (eventLock)
        {
            _eventQueue.Enqueue(flag);
        }
    }

    public void Dispatch(EventFlag flag, ref IEntity entity)
    {
        lock (eventLock)
        {
            Console.WriteLine($"Dispatching Event: {flag.ToString()}");
            _specificEvent.Enqueue((flag, entity));
        }
    }

    public void PollEvents()
    {
        Task.Run(() =>
        {
            lock (eventLock)
            {
                if (_eventQueue.Count > 0)
                {
                    var flag = _eventQueue.Dequeue();
                    for (int i = 0; i < _eventListeners.Count; i++)
                    {
                        if (!(_eventListeners[i] & flag))
                            continue;
                        _eventListeners[i].ProcessEvent(flag);
                    }
                }

                if (_specificEvent.Count > 0)
                {
                    var _event = _specificEvent.Dequeue();
                    for (int i = 0; i < _eventListeners.Count; i++)
                    {
                        if (!(_eventListeners[i] & _event.flag))
                            continue;
                        _eventListeners[i].ProcessEvent(_event.flag, _event.entity);
                    }
                }
            }
        });
    }

    public void Shutdown()
    {
        
    }
}