using MapleSharp.Core.Event;
using MapleSharp.Core.Interface;

namespace MapleSharp.Core;

public class EngineObject
{
    private readonly Engine engine;
    
    public EngineObject(Engine engine)
    {
        this.engine = engine;
    }
    
    public T GetSubsystem<T>()
    {
        return engine.GetSubsystem<T>();
    }
    
    protected void SubscribeToEvent(string eventName, Action<EventData> callback)
    {
        engine.GetSubsystem<EventSystem>().SubscribeToEvent(eventName, this, callback);
    }
    
    protected void SubscribeToEvent(string eventName, object sender, Action<EventData> callback)
    {
        engine.GetSubsystem<EventSystem>().SubscribeToEvent(eventName, sender, this, callback);
    }
    
    protected void SendEvent(EventData data)
    {
        engine.GetSubsystem<EventSystem>().SendEvent(data);
    }
}