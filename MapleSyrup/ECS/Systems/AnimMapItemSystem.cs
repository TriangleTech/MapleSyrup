using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.ECS.Components;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework;

namespace MapleSyrup.ECS.Systems;

public class AnimMapItemSystem : UpdateableSystem
{
    public AnimMapItemSystem(GameContext context) 
        : base(context)
    {
    }

    public override void OnUpdate(EventData eventData)
    {
        var gameTime = eventData["GameTime"] as GameTime;
        var scene = Context.GetSubsystem<SceneSystem>().Current;
        var entities = scene.Entities.OrderBy(x => x.Layer).ThenBy(x => x.ZIndex).ToList();
        for (int i = 0; i < entities.Count; i++)
        {
            if (!entities[i].IsEnabled || !entities[i].HasComponent<AnimatedMapItem>())
                continue;
            var item = entities[i].GetComponent<AnimatedMapItem>();
            Task.Run(() => UpdateAnimation(gameTime, item));
        }
        base.OnUpdate(eventData);
    }
    
    private void UpdateAnimation(GameTime gameTime, AnimatedMapItem item)
    {
        var time = Context.GetSubsystem<TimeSystem>();
        if (item.CurrentFrame >= item.Frames.Count - 1)
        {
            item.CurrentFrame = 0;
            item.CurrentDelay = item.Delay[0];
        }

        if (item.CurrentDelay <= 0)
        {
            item.CurrentFrame++;
            item.CurrentDelay = item.Delay[item.CurrentFrame];
        }
        else
        {
            item.CurrentDelay -= (int)time.DeltaTime;
        }
    }
}