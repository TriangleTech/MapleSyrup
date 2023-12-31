using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.Gameplay.World;
using MapleSyrup.Subsystems;

namespace MapleSyrup.ECS.Systems.Map;

public class CloudSystem
{
    private readonly GameContext Context;

    public CloudSystem(GameContext context)
    {
        Context = context;
        var events = context.GetSubsystem<EventSystem>();
        events.Subscribe(this, EventType.OnSceneRender, OnDraw);
        events.Subscribe(this, EventType.OnSceneUpdate, OnUpdate);
    }
    
    private void OnDraw(EventData eventData)
    {
        var scene = Context.GetSubsystem<SceneSystem>().Current;
        var entities = scene.Entities.OrderBy(x => x.Layer).ThenBy(x => x.ZIndex).ToList();

        for (int i = 0; i < entities.Count; i++)
        {
            if (!entities[i].IsEnabled)
                continue;

            if (!entities[i].HasComponent<Cloud>())
                return;
            var cloud = entities[i].GetComponent<Cloud>();
        }
    }

    private void OnUpdate(EventData eventData)
    {
        var scene = Context.GetSubsystem<SceneSystem>().Current;
        var entities = scene.Entities.OrderBy(x => x.Layer).ThenBy(x => x.ZIndex).ToList();

        for (int i = 0; i < entities.Count; i++)
        {
            if (!entities[i].IsEnabled)
                continue;

            if (!entities[i].HasComponent<Cloud>())
                return;

            var cloud = entities[i].GetComponent<Cloud>();
            cloud.Position.X += cloud.Speed * (float)eventData["DeltaTime"];
            if (cloud.Position.X >= cloud.PositionLimit)
                cloud.Position.X = cloud.StartingPosition;

        }
    }
}