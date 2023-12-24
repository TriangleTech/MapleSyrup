using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.ECS.Components;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework.Graphics;
using SDL2;
using Color = Microsoft.Xna.Framework.Color;

namespace MapleSyrup.ECS.Systems;

public class TileObjSystem
{
    private readonly GameContext Context;
    private SpriteBatch spriteBatch;

    public TileObjSystem(GameContext context)
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
        var entities = scene.GetEntitiesByTag("MapItem");
        var camera = scene.GetRoot().GetComponent<Camera>();

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap,
            DepthStencilState.Default, RasterizerState.CullNone, null, camera.Transform);
        for (int i = 0; i < entities.Count; i++)
        {
            if (!entities[i].IsEnabled)
                continue;

            if (entities[i].HasComponent<AnimatedMapItem>())
            {
                var animItem = entities[i].GetComponent<AnimatedMapItem>();
                var transform = entities[i].GetComponent<Transform>();
                var currentFrame = animItem.CurrentFrame;

                transform.Position = animItem.Positions[currentFrame];
                transform.Origin = animItem.Origins[currentFrame];

                spriteBatch.Draw(animItem.Frames[currentFrame], transform.Position, null, Color.White, 0f,
                    transform.Origin, 1f, SpriteEffects.None, 0f);
            }
            else if (entities[i].HasComponent<MapItem>())
            {
                var item = entities[i].GetComponent<MapItem>();
                var transform = entities[i].GetComponent<Transform>();
                spriteBatch.Draw(item.Texture, transform.Position, null, Color.White, 0f, transform.Origin, 1f,
                    SpriteEffects.None, 0f);
            }
        }

        spriteBatch.End();
        entities.Clear(); // TODO: This is a hack to prevent memory leak;
    }

    private void OnUpdate(EventData eventData)
    {
        var scene = Context.GetSubsystem<SceneSystem>();
        var entities = scene.GetEntitiesByTag("MapItem");
        
        for (var i = 0; i < entities.Count; i++)
        {
            if (!entities[i].IsEnabled && !entities[i].HasComponent<AnimatedMapItem>())
                continue;
            var item = entities[i].GetComponent<AnimatedMapItem>();
            Task.Run(() => UpdateAnimation(ref item));
        }
        
        entities.Clear(); // TODO: This is a hack to prevent memory leak
    }

    private void UpdateAnimation(ref AnimatedMapItem item)
    {
        var time = Context.GetSubsystem<TimeSystem>();
        if (item.CurrentFrame >= item.Frames.Count - 1)
        {
            item.CurrentFrame = 0;
            item.CurrentDelay = item.Delay[0];
        }

        if (item.CurrentDelay <= 0)
        {
            item.CurrentFrame++;
            item.CurrentDelay = item.Delay[item.CurrentFrame];
        }
        else
        {
            item.CurrentDelay -= (int)time.DeltaTime;
        }
    }
}