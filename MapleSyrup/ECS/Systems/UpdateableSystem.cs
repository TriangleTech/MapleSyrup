using MapleSyrup.Core;
using MapleSyrup.Core.Event;

namespace MapleSyrup.ECS.Systems;

public abstract class UpdateableSystem : EventObject
{
    protected UpdateableSystem(GameContext context) 
        : base(context)
    {
        SubscribeToEvent(EventType.OnSceneUpdate, new Subscriber() { EventType = EventType.OnSceneUpdate, Sender = this, Event = OnUpdate });
    }
    
    public virtual void OnUpdate(EventData eventData)
    {
        
    }
}