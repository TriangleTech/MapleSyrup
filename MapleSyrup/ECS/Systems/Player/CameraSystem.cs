using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.ECS.Components;
using MapleSyrup.ECS.Components.Common;
using MapleSyrup.Gameplay.World;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MapleSyrup.ECS.Systems.Player;

public class CameraSystem 
{
    private readonly GameContext Context;
    
    public CameraSystem(GameContext context) 
    {
        Context = context;
        var events = Context.GetSubsystem<EventSystem>();
        events.Subscribe(this, EventType.OnSceneUpdate, OnUpdate);
    }

    private void OnUpdate(EventData eventData)
    {
        var scene = Context.GetSubsystem<SceneSystem>();
        var camera = scene.Current.Entities[0].GetComponent<Camera>();
        var info = scene.Current.Entities[0].GetComponent<WorldInfo>();
        var left = scene.FarLeft + 10f;
        var right = scene.FarRight - camera.Viewport.Width;
        var top = scene.FarTop - 100f;
        var bottom = scene.FarBottom - camera.Viewport.Height;
        
        if (camera.Position.X <= left)
            camera.Position.X = MathHelper.Clamp(camera.Position.X, left, left); 
        if (camera.Position.X >= right)
            camera.Position.X = MathHelper.Clamp(camera.Position.X, right, right);

        if (camera.Position.Y <= top)
            camera.Position.Y = MathHelper.Clamp(camera.Position.Y, top, top);
        if (camera.Position.Y >= bottom)
            camera.Position.Y = MathHelper.Clamp(camera.Position.Y, bottom, bottom);

        if (camera.EnabledCulling)
        {
            var entities = scene.Current.Entities.Where(x => !x.HasComponent<BackgroundItem>()).ToList();
            for (int i = 0; i < entities.Count; i++)
            {
                switch (entities[i].GetComponent<Transform>().Position)
                {
                    case var enabled
                        when entities[i].GetComponent<Transform>().Position.X <
                             camera.Position.X - camera.Viewport.Width - 175f ||
                             entities[i].GetComponent<Transform>().Position.X >
                             camera.Position.X + camera.Viewport.Width + 175f:
                        entities[i].IsEnabled = false;
                        break;
                    case var enabled when entities[i].GetComponent<Transform>().Position.Y <
                                          camera.Position.Y - camera.Viewport.Height - 175f ||
                                          entities[i].GetComponent<Transform>().Position.Y >
                                          camera.Position.Y + camera.Viewport.Height + 175f:
                        entities[i].IsEnabled = false;
                        break;
                    default:
                        entities[i].IsEnabled = true;
                        break;

                }
                /*
                if (entities[i].GetComponent<Transform>().Position.X < camera.Position.X - camera.Viewport.Width - 175f)
                    entities[i].IsEnabled = false;
                else if (entities[i].GetComponent<Transform>().Position.X >
                         camera.Position.X + camera.Viewport.Width + 175f)
                    entities[i].IsEnabled = false;
                else if (entities[i].GetComponent<Transform>().Position.Y <
                         camera.Position.Y - camera.Viewport.Height - 175f)
                    entities[i].IsEnabled = false;
                else if (entities[i].GetComponent<Transform>().Position.Y >
                         camera.Position.Y + camera.Viewport.Height + 175f)
                    entities[i].IsEnabled = false;
                else
                    entities[i].IsEnabled = true;*/
            }
        }
    }
}