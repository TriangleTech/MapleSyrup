using MapleSyrup.Core;
using MapleSyrup.Core.Event;

namespace MapleSyrup.ECS.Systems;

public class RenderSystem : DrawableSystem
{
    public RenderSystem(GameContext context) 
        : base(context)
    {
    }

    public override void OnRender(EventData eventData)
    {
        base.OnRender(eventData);
    }
}