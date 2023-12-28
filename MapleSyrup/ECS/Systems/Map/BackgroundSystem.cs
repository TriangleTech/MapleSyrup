using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.ECS.Components;
using MapleSyrup.ECS.Components.Map;
using MapleSyrup.Gameplay.World;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

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
            
            var posX = background.Rx * 0.05f;
            var posY = background.Ry * 0.05f;
            
            switch (background.Type)
            {
                case BackgroundType.HorizontalTiled:
                    background.SourceRect = new Rectangle(0, 0, info.Bounds.Width, background.Texture.Height);
                    break;
            }

            Matrix matrix = Matrix.CreateTranslation(new Vector3(camera.Position.X * (background.Rx * 0.01f) + camera.Viewport.Width / 2f, 
                camera.Position.Y * (background.Ry * 0.01f) + (camera.Viewport.Height / 2f), 0));
            
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap,
                DepthStencilState.Default, RasterizerState.CullNone, null, matrix);
            
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
        var camera = scene.GetRoot().GetComponent<Camera>();
        
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
        var time = Context.GetSubsystem<TimeSystem>();
        
    }
}