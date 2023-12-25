using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.ECS.Components;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework.Input;

namespace MapleSyrup.ECS.Systems.Player;

public class MovementSystem
{
    private readonly GameContext Context;
    public MovementSystem(GameContext context) 
    {
        Context = context;
        
        var events = Context.GetSubsystem<EventSystem>();
        events.Subscribe(this, EventType.OnSceneUpdate, OnUpdate);
    }

    private void OnUpdate(EventData eventData)
    {
        var keyboard = Keyboard.GetState();
        var scene = Context.GetSubsystem<SceneSystem>().Current;
        var camera = scene.Entities[0].GetComponent<Camera>();
        var time = Context.GetSubsystem<TimeSystem>();
        
        if (keyboard.IsKeyDown(Keys.W))
        {
            camera.Position.Y -= 1f * time.DeltaTime;
        }
        if (keyboard.IsKeyDown(Keys.S))
        {
            camera.Position.Y += 1f * time.DeltaTime;
        }
        if (keyboard.IsKeyDown(Keys.A))
        {
            camera.Position.X -= 1f * time.DeltaTime;
        }
        if (keyboard.IsKeyDown(Keys.D))
        {
            camera.Position.X += 1f * time.DeltaTime;
        }
    }
}