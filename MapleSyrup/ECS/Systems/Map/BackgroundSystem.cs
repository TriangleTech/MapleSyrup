using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.ECS.Components;
using MapleSyrup.ECS.Components.Map;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.ECS.Systems.Map;

public class BackgroundSystem
{
    private readonly GameContext Context;
    private SpriteBatch spriteBatch;

    public BackgroundSystem(GameContext context)
    {
        Context = context;
        spriteBatch = new SpriteBatch(Context.GraphicsDevice);
        
        var events = Context.GetSubsystem<EventSystem>();
        events.Subscribe(this, EventType.OnSceneRender, OnDraw);
        events.Subscribe(this, EventType.OnSceneUpdate, OnUpdate);
    }

    private void OnDraw(EventData eventData)
    {
        var scene = Context.GetSubsystem<SceneSystem>();
        var camera = scene.GetRoot().GetComponent<Camera>();
        var info = scene.GetRoot().GetComponent<WorldInfo>();
        var entities = scene.GetEntitiesByTag("Background");

        
        for (int i = 0; i < entities.Count; i++)
        {
            if (!entities[i].IsEnabled || !entities[i].HasComponent<BackgroundItem>())
                continue;
            
            var background = entities[i].GetComponent<BackgroundItem>();
            var transform = entities[i].GetComponent<Transform>();
            
            var posX = background.Rx * 0.01f;
            
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicWrap,
                DepthStencilState.Default, RasterizerState.CullNone, null, camera.GetViewMatrix(new Vector2(posX , 1)));
            
            spriteBatch.Draw(background.Texture, transform.Position, null, background.Color,
                transform.Rotation, transform.Origin, transform.Scale, background.Flipped, 0f);
            
            spriteBatch.End();
            
        }
        entities.Clear();
    }
    
    private void OnUpdate(EventData eventData)
    {
        var scene = Context.GetSubsystem<SceneSystem>();
        var entities = scene.GetEntitiesByTag("Background");
        
        for (int i = 0; i < entities.Count; i++)
        {
            if (!entities[i].IsEnabled || !entities[i].HasComponent<BackgroundItem>())
                continue;
            
            var background = entities[i].GetComponent<BackgroundItem>();
            var transform = entities[i].GetComponent<Transform>();
            Task.Run(() => UpdatePosition(ref background, ref transform));
        }
    }

    private void UpdatePosition(ref BackgroundItem background, ref Transform transform)
    {
        var scene = Context.GetSubsystem<SceneSystem>();
        var camera = scene.GetRoot().GetComponent<Camera>();
        var info = scene.GetRoot().GetComponent<WorldInfo>();
        var time = Context.GetSubsystem<TimeSystem>();
        
        if (transform.Position.Y >= info.Bounds.Top)
            transform.Position.Y = Math.Clamp(transform.Position.Y, info.Bounds.Top, info.Bounds.Bottom);
        else if (transform.Position.Y <= info.Bounds.Bottom)
            transform.Position.Y = Math.Clamp(transform.Position.Y, info.Bounds.Top, info.Bounds.Bottom);

        if (transform.Position.X >= info.Bounds.Left)
            transform.Position.X = Math.Clamp(transform.Position.X, info.Bounds.Left, info.Bounds.Right);
        else if (transform.Position.X <= info.Bounds.Right)
            transform.Position.X = Math.Clamp(transform.Position.X, info.Bounds.Left, info.Bounds.Right);
    }
}