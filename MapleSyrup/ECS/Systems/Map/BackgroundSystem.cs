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
        var entities = scene.GetEntitiesByTag("Background");


        for (int i = 0; i < entities.Count; i++)
        {
            if (!entities[i].IsEnabled || !entities[i].HasComponent<BackgroundItem>())
                continue;

            var background = entities[i].GetComponent<BackgroundItem>();
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
        var camera = scene.GetRoot().GetComponent<Camera>();

        for (int i = 0; i < entities.Count; i++)
        {
            if (!entities[i].IsEnabled || !entities[i].HasComponent<BackgroundItem>())
                continue;
            var next = i;
            Task.Run(() => UpdateMatrix(entities[next]));
        }
    }

    private void UpdateMatrix(Entity entity)
    {
        var scene = Context.GetSubsystem<SceneSystem>();
        var camera = scene.GetRoot().GetComponent<Camera>();
        var info = scene.GetRoot().GetComponent<WorldInfo>();
        var transform = entity.GetComponent<Transform>();
        var background = entity.GetComponent<BackgroundItem>();

        background.Parallax = Matrix.CreateTranslation(new Vector3(
            (camera.Position.X * (background.Rx * 0.008f) + camera.Viewport.Width / 2f),
            camera.Position.Y * (background.Ry * 0.01f) + (camera.Viewport.Height / 2f), 0));

        switch (background.Type)
        {
            case BackgroundType.Default:
                break;
            case BackgroundType.HorizontalTiling:
                background.SourceRect = new Rectangle(0, 0, info.Bounds.Width, background.Texture.Height);
                break;
            case BackgroundType.HorizontalScrolling:
                if (transform.Position.X < scene.FarLeft)
                    transform.Position.X = scene.FarRight;
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