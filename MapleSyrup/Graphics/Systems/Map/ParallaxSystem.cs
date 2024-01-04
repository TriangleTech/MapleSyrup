using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.ECS;
using MapleSyrup.Gameplay.Map;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MapleSyrup.Graphics.Systems.Map;

public class ParallaxSystem
{
    private readonly GameContext Context;
    private SpriteBatch spriteBatch;

    public ParallaxSystem(GameContext context)
    {
        Context = context;
        spriteBatch = new SpriteBatch(Context.GraphicsDevice);

        var events = Context.GetSubsystem<EventSystem>();
        events.Subscribe(this, "SCENE_RENDER", OnDraw);
        events.Subscribe(this, "SCENE_UPDATE", OnUpdate);
    }

    private void OnDraw(EventData eventData)
    {
        var scene = Context.GetSubsystem<SceneSystem>();
        var entities = scene.GetEntitiesByTag("Background");
        
        for (int i = 0; i < entities.Count; i++)
        {
            if (!entities[i].IsEnabled || !entities[i].HasComponent<ParallaxBackground>())
                continue;

            var background = entities[i].GetComponent<ParallaxBackground>();
            var transform = entities[i].GetComponent<Transform>();

            // As of right now there isn't a better way
            // TODO: Changes this? Or don't?
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap,
                DepthStencilState.Default, RasterizerState.CullNone, null, background.Parallax);

            spriteBatch.Draw(background.Texture, transform.Position, background.SourceRect == Rectangle.Empty ? null : background.SourceRect, background.Color,
                transform.Rotation, transform.Origin, transform.Scale, background.Flipped, 0f);

            spriteBatch.End();

        }

        entities.Clear();
    }

    private void OnUpdate(EventData eventData)
    {
        var scene = Context.GetSubsystem<SceneSystem>();
        var entities = scene.GetEntitiesByTag("Background");
        var camera = scene.Root.GetComponent<Camera>();

        for (int i = 0; i < entities.Count; i++)
        {
            if (!entities[i].IsEnabled || !entities[i].HasComponent<ParallaxBackground>())
                continue;
            var next = i;
            Task.Run(() => UpdateMatrix(entities[next]));
        }
    }

    private void UpdateMatrix(Entity entity)
    {
        var scene = Context.GetSubsystem<SceneSystem>();
        var camera = scene.Root.GetComponent<Camera>();
        var info = scene.Root.GetComponent<WorldInfo>();
        var transform = entity.GetComponent<Transform>();
        var background = entity.GetComponent<ParallaxBackground>();

        background.Parallax = Matrix.CreateTranslation(new Vector3(
            (camera.Position.X * (background.Rx * 0.008f) + camera.Viewport.Width / 2f),
            camera.Position.Y * (background.Ry * 0.008f) + (camera.Viewport.Height / 2f), 0));

        switch (background.Type)
        {
            case BackgroundType.Default:
                break;
            case BackgroundType.HorizontalTiling:
                background.SourceRect = new Rectangle(0, 0, info.Bounds.Width, background.Texture.Height);
                break;
            case BackgroundType.HorizontalScrolling:
                break;
            case BackgroundType.HorizontalScrollingHVTiling:
                break;
            case BackgroundType.VerticalTiling:
                background.SourceRect = new Rectangle(0, 0, background.Texture.Width, info.Bounds.Height);
                break;
            case BackgroundType.VerticalScrolling:
                break;
            case BackgroundType.VerticalScrollingHVTiling:
                break;
        }
    }
}