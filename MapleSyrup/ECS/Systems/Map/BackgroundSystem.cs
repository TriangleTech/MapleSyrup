using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.ECS.Components;
using MapleSyrup.ECS.Components.Map;
using MapleSyrup.Gameplay.World;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
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
    }

    private void OnDraw(EventData eventData)
    {
        var scene = Context.GetSubsystem<SceneSystem>();
        var camera = scene.GetRoot().GetComponent<Camera>();
        var info = scene.GetRoot().GetComponent<WorldInfo>();
        var entities = scene.GetEntitiesByTag("Background");

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicWrap,
            DepthStencilState.Default, RasterizerState.CullNone, null, camera.Transform);
        for (int i = 0; i < entities.Count; i++)
        {
            if (!entities[i].IsEnabled || !entities[i].HasComponent<BackgroundItem>())
                continue;
            
            var background = entities[i].GetComponent<BackgroundItem>();
            var transform = entities[i].GetComponent<Transform>();
            if (background.Type == BackgroundType.Default)
                transform.Scale = (float)background.Texture.Width / camera.Viewport.Width;
            
            spriteBatch.Draw(background.Texture, transform.Position, transform?.Source == Rectangle.Empty ? null : transform.Source, background.Color,
                transform.Rotation, transform.Origin, transform.Scale, background.Flipped, 0f);
        }

        spriteBatch.End();
    }
}