using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MapleSyrup.Graphics.Systems.Player;

public class MovementSystem
{
    private readonly GameContext Context;
    public MovementSystem(GameContext context) 
    {
        Context = context;
        
        var events = Context.GetSubsystem<EventSystem>();
        events.Subscribe(this, "SCENE_UPDATE", OnUpdate);
    }

    private void OnUpdate(EventData eventData)
    {
        var keyboard = Keyboard.GetState();
        var scene = Context.GetSubsystem<SceneSystem>();
        var camera = scene.Root.GetComponent<Camera>();
        var time = Context.GetSubsystem<TimeSystem>();
        var player = scene.GetPlayerByName("TestPlayer");
        var playerTransform = player.GetComponent<Transform>();
        
        if (keyboard.IsKeyDown(Keys.W))
        {
            playerTransform.Position.Y -= 0.5f * time.DeltaTime;
        }
        if (keyboard.IsKeyDown(Keys.S))
        {
            playerTransform.Position.Y += 0.5f * time.DeltaTime;
        }
        if (keyboard.IsKeyDown(Keys.A))
        {
            playerTransform.Position.X -= 0.5f * time.DeltaTime;
        }
        if (keyboard.IsKeyDown(Keys.D))
        {
            playerTransform.Position.X += 0.5f * time.DeltaTime;
        }
        
        camera.Position = Vector2.Lerp(camera.Position, playerTransform.Position, 1.5f);

    }
}