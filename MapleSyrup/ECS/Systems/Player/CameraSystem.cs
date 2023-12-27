using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.ECS.Components;
using MapleSyrup.ECS.Components.Map;
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
        var leftMost = scene.FarLeftX() + 10f;
        var rightMost = scene.FarRightX() - camera.Viewport.Width;
        var topMost = scene.FarTopY();
        var bottomMost = scene.FarBottomY() - camera.Viewport.Height;
        
        if (camera.Position.X <= leftMost)
            camera.Position.X = MathHelper.Clamp(camera.Position.X, leftMost, leftMost); 
        if (camera.Position.X >= rightMost)
            camera.Position.X = MathHelper.Clamp(camera.Position.X, rightMost, rightMost);

        if (camera.Position.Y <= topMost)
            camera.Position.Y = MathHelper.Clamp(camera.Position.Y, topMost, topMost);
        if (camera.Position.Y >= bottomMost)
            camera.Position.Y = MathHelper.Clamp(camera.Position.Y, bottomMost, bottomMost);

        if (camera.EnabledCulling)
        {
            var entities = scene.Current.Entities.Where(x => !x.HasComponent<BackgroundItem>()).ToList();
            for (int i = 0; i < entities.Count; i++)
            {
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
                    entities[i].IsEnabled = true;
            }
        }
    }
}