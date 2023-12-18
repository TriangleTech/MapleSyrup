using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.ECS.Components;
using MapleSyrup.Subsystems;

namespace MapleSyrup.ECS.Systems;

public class AnimMapItemSystem : UpdateableSystem
{
    public AnimMapItemSystem(GameContext context) 
        : base(context)
    {
    }

    public override void OnUpdate(EventData eventData)
    {
        var scene = Context.GetSubsystem<SceneSystem>().Current;
        var entities = scene.Entities.OrderBy(x => x.Layer).ThenBy(x => x.ZIndex).ToList();
        for (int i = 0; i < entities.Count; i++)
        {
            if (!entities[i].IsEnabled || !entities[i].HasComponent<AnimatedMapItem>())
                continue;
            var item = entities[i].GetComponent<AnimatedMapItem>();
            Task.Run(() => UpdateAnimation(item));
        }
        base.OnUpdate(eventData);
    }
    
    private void UpdateAnimation(AnimatedMapItem item)
    {
        if (item.CurrentFrame >= item.Frames.Count - 1)
            item.CurrentFrame = 0;
        item.CurrentFrame++;
    }
}