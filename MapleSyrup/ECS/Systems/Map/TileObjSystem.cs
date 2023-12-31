using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.ECS.Components;
using MapleSyrup.ECS.Components.Common;
using MapleSyrup.Gameplay.World;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace MapleSyrup.ECS.Systems.Map;

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
        var info = scene.GetRoot().GetComponent<WorldInfo>();
        
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap,
            DepthStencilState.Default, RasterizerState.CullNone, null, camera.GetViewMatrix());
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
            else if (entities[i].HasComponent<BlendAnimation>())
            {
                var transform = entities[i].GetComponent<Transform>();
                var blend = entities[i].GetComponent<BlendAnimation>();
                var currentFrame = blend.Frames[blend.CurrentFrame];
                blend.Color.A = (byte)blend.StartingAlpha;

                spriteBatch.Draw(currentFrame, transform.Position, null, blend.Color, 0f,
                    transform.Origin, 1f,
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
            if (!entities[i].IsEnabled)
                continue;

            if (entities[i].HasComponent<AnimatedMapItem>())
            {
                var item = entities[i].GetComponent<AnimatedMapItem>();
                Task.Run(() => UpdateFrameAnimation(ref item));
            }
            else if (entities[i].HasComponent<BlendAnimation>())
            {
                var item = entities[i].GetComponent<BlendAnimation>();
                Task.Run(() => UpdateBlendAnimation(ref item));
            }
        }

        entities.Clear(); // TODO: This is a hack to prevent memory leak
    }

    private void UpdateFrameAnimation(ref AnimatedMapItem item)
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

    private void UpdateBlendAnimation(ref BlendAnimation blend)
    {
        var time = Context.GetSubsystem<TimeSystem>();
        if (blend.CurrentFrame >= blend.Frames.Count - 1)
        {
            blend.CurrentFrame = 0;
            blend.CurrentDelay = blend.Delay[0];
            blend.StartingAlpha = blend.Alpha[0];
            blend.EndingAlpha = blend.Alpha[1];
        }
        // A weird hack to prevent the alpha from going out of bounds
        // TODO: Find a better way to do this
        if (blend.StartingAlpha >= 255)
        {
            blend.StartingAlpha = 255;
            blend.EndingAlpha = 0;
        }
        else if (blend.StartingAlpha <= 0)
        {
            blend.StartingAlpha = 0;
            blend.EndingAlpha = 255;
        }

        if (blend.CurrentDelay <= 0 && blend.StartingAlpha == blend.EndingAlpha)
        {
            blend.CurrentFrame++;
            blend.CurrentDelay = blend.Delay[blend.CurrentFrame];
            blend.StartingAlpha = blend.Alpha[blend.CurrentFrame];
            blend.EndingAlpha = blend.StartingAlpha == 255 ? (byte)0 : (byte)255;
        }
        else
        {
            if (blend.StartingAlpha < blend.EndingAlpha)
            {
                blend.StartingAlpha++;
            }
            else if (blend.StartingAlpha > blend.EndingAlpha)
            {
                blend.StartingAlpha--;
            }
            
            blend.CurrentDelay -= (int)time.DeltaTime;
        }
    }
}