using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework.Input;

namespace MapleSyrup.Graphics.Systems.Player;

public class PlayerMovement
{
    private readonly GameContext Context;

    public PlayerMovement(GameContext context)
    {
        Context = context;

        var events = Context.GetSubsystem<EventSystem>();
        events.Subscribe(this, "SCENE_UPDATE", OnUpdate);
    }

    private void OnUpdate(EventData eventData)
    {
        var scene = Context.GetSubsystem<SceneSystem>();
        var events = Context.GetSubsystem<EventSystem>();
        var time = Context.GetSubsystem<TimeSystem>();
        var player = scene.GetPlayerByName("TestPlayer");
        var transform = player.GetComponent<Transform>();
        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.Left))
        {
            transform.Position.X -= 0.5f * time.DeltaTime;
            events.Publish("PLAYER_ON_MOVE");
        }
        if (keyboard.IsKeyDown(Keys.Right))
        {
            transform.Position.X += 0.5f * time.DeltaTime;
            events.Publish("PLAYER_ON_MOVE");
        }
    }
}