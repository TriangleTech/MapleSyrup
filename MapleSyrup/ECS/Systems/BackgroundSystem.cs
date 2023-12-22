using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.ECS.Components;
using MapleSyrup.Gameplay.World;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MapleSyrup.ECS.Systems;

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
        var scene = Context.GetSubsystem<SceneSystem>().Current;
        var camera = scene.Entities[0].GetComponent<Camera>();
        var info = scene.Entities[0].GetComponent<WorldInfo>();

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap,
            DepthStencilState.Default, RasterizerState.CullNone, null, camera.Transform);
        for (int i = 0; i < scene.Entities.Count; i++)
        {
            if (!scene.Entities[i].IsEnabled || !scene.Entities[i].HasComponent<BackgroundItem>())
                continue;
            var background = scene.Entities[i].GetComponent<BackgroundItem>();
            spriteBatch.Draw(background.Texture, background.Position, background?.Source == Rectangle.Empty ? null : background.Source, background.Color,
                background.Rotation, background.Origin, background.Scale, background.Flipped, 0f);
        }

        spriteBatch.End();
    }
}