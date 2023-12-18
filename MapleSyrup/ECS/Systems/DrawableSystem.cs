using MapleSyrup.Core;
using MapleSyrup.Core.Event;

namespace MapleSyrup.ECS.Systems;

public abstract class DrawableSystem : EventObject
{
    protected DrawableSystem(GameContext context) 
        : base(context)
    {
        SubscribeToEvent(EventType.OnSceneRender, new Subscriber() { EventType = EventType.OnSceneRender, Sender = this, Event = OnRender });
    }

    protected virtual void OnRender(EventData eventData)
    {
        
    }
}