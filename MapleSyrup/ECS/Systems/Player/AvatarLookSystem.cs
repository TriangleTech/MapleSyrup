using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.Subsystems;

namespace MapleSyrup.ECS.Systems.Player;

public class AvatarLookSystem
{
    private readonly GameContext Context;

    public AvatarLookSystem(GameContext context)
    {
        Context = context;
        var events = Context.GetSubsystem<EventSystem>();
        events.Subscribe(this, EventType.OnSceneRender, OnRender);
        events.Subscribe(this, EventType.OnSceneUpdate, OnUpdate);
    }

    private void OnRender(EventData obj)
    {
        
    }

    private void OnUpdate(EventData obj)
    {
        
    }
}