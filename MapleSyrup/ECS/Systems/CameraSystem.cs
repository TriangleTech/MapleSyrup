using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.ECS.Components;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MapleSyrup.ECS.Systems;

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
        var scene = Context.GetSubsystem<SceneSystem>().Current;
        var camera = scene.Entities[0].GetComponent<Camera>();
        var info = scene.Entities[0].GetComponent<WorldInfo>();
        camera.Bounds = new Rectangle(0, 0, 800, 600);
        camera.Viewport = new Viewport(camera.Bounds);
        camera.Transform = Matrix.CreateTranslation(new Vector3(-camera.Position.X, -camera.Position.Y, 0)) *
                           Matrix.CreateRotationZ(camera.Rotation) *
                           Matrix.CreateScale(new Vector3(camera.Zoom, camera.Zoom, 1)) *
                           Matrix.CreateTranslation(new Vector3(camera.Origin.X, camera.Origin.Y, 0));
    }
}