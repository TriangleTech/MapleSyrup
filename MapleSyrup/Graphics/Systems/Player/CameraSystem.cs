using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework;

namespace MapleSyrup.Graphics.Systems.Player;

public class CameraSystem
{
    private readonly GameContext Context;
    
    public CameraSystem(GameContext context)
    {
        Context = context;

        var events = context.GetSubsystem<EventSystem>();
        events.Subscribe(this, "PLAYER_ON_MOVE", OnPlayerMove);
        events.Subscribe(this, "SCENE_UPDATE", OnUpdate);
    }

    private void OnPlayerMove(EventData eventData)
    {
        var scene = Context.GetSubsystem<SceneSystem>();
        var camera = scene.Root.GetComponent<Camera>();
        var left = scene.FarLeft + 10f;
        var right = scene.FarRight - camera.Viewport.Width;
        var top = scene.FarTop - 100f;
        var bottom = scene.FarBottom - camera.Viewport.Height;
        var player = scene.GetPlayerByName("TestPlayer");
        var transform = player.GetComponent<Transform>();

        camera.Position.X = transform.Position.X - camera.Origin.X;
        
        if (camera.Position.X <= left)
            camera.Position.X = MathHelper.Clamp(camera.Position.X, left, 0);
        if (camera.Position.X >= right)
            camera.Position.X = MathHelper.Clamp(camera.Position.X, right, right);

        if (camera.Position.Y <= top)
            camera.Position.Y = MathHelper.Clamp(camera.Position.Y, top, top);
        if (camera.Position.Y >= bottom)
            camera.Position.Y = MathHelper.Clamp(camera.Position.Y, bottom, bottom);

    }

    private void OnUpdate(EventData eventData)
    {
        var scene = Context.GetSubsystem<SceneSystem>();
        var camera = scene.Root.GetComponent<Camera>();
        
        if (camera.EnableCulling)
        {
            var entities = scene.Current.Entities.Where(x => !x.HasComponent<ParallaxBackground>()).ToList();
            for (int i = 0; i < entities.Count; i++)
            {
                switch (entities[i].GetComponent<Transform>().Position)
                {
                    case var _
                        when entities[i].GetComponent<Transform>().Position.X <
                             camera.Position.X - camera.Viewport.Width - 175f ||
                             entities[i].GetComponent<Transform>().Position.X >
                             camera.Position.X + camera.Viewport.Width + 175f:
                        entities[i].SetVisibility(false);
                        break;
                    case var _ when entities[i].GetComponent<Transform>().Position.Y <
                                    camera.Position.Y - camera.Viewport.Height - 175f ||
                                    entities[i].GetComponent<Transform>().Position.Y >
                                    camera.Position.Y + camera.Viewport.Height + 175f:
                        entities[i].SetVisibility(false);
                        break;
                    default:
                        entities[i].SetVisibility(true);
                        break;
                }
            }
        }
        
        camera.UpdateMatrix();
    }
}