using MapleSyrup.Event;

namespace MapleSyrup.Managers;

public class EventManager : IManager
{
    private ManagerLocator? _locator;
    private object addLock = new();
    private List<IEventListener> _eventListeners;
    private Queue<EventFlag> _eventQueue;

    public EventManager()
    {
        _eventListeners = new();
        _eventQueue = new();
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
        lock (addLock)
        {
            _eventQueue.Enqueue(flag);
        }
    }

    public void PollEvents()
    {
        Task.Run(() =>
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
        });
    }

    public void Shutdown()
    {
        
    }
}