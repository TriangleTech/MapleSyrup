using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.Gameplay.Map;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace MapleSyrup.Graphics.Systems.Map;

public class TileObjSystem
{
    private readonly GameContext Context;
    private SpriteBatch spriteBatch;

    public TileObjSystem(GameContext context)
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
        var entities = scene.GetEntitiesByTag("MapItem");
        var camera = scene.Root.GetComponent<Camera>();
        var info = scene.Root.GetComponent<WorldInfo>();
        
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap,
            DepthStencilState.Default, RasterizerState.CullNone, null, camera.GetViewMatrix());
        for (int i = 0; i < entities.Count; i++)
        {
            if (!entities[i].IsEnabled)
                continue;

            if (entities[i].HasComponent<Animation>())
            {
                var animItem = entities[i].GetComponent<Animation>();
                var transform = entities[i].GetComponent<Transform>();
                var currentFrame = animItem.NextFrame;

                transform.Position = animItem.Position[currentFrame];
                transform.Origin = animItem.Origin[currentFrame];

                spriteBatch.Draw(animItem.Frames[currentFrame], transform.Position, null, Color.White, 0f,
                    transform.Origin, 1f, SpriteEffects.None, 0f);
            }
            else if (entities[i].HasComponent<Sprite>())
            {
                var item = entities[i].GetComponent<Sprite>();
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
        entities.Clear();
    }

    private void OnUpdate(EventData eventData)
    {
        var scene = Context.GetSubsystem<SceneSystem>();
        var entities = scene.GetEntitiesByTag("MapItem");

        for (var i = 0; i < entities.Count; i++)
        {
            if (!entities[i].IsEnabled)
                continue;

            if (entities[i].HasComponent<Animation>())
            {
                var item = entities[i].GetComponent<Animation>();
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

    private void UpdateFrameAnimation(ref Animation item)
    {
        var time = Context.GetSubsystem<TimeSystem>();
        if (item.NextFrame >= item.Frames.Count - 1)
        {
            item.NextFrame = 0;
            item.NextDelay = item.Delay[0];
        }

        if (item.NextDelay <= 0)
        {
            item.NextFrame++;
            item.NextDelay = item.Delay[item.NextFrame];
        }
        else
        {
            item.NextDelay -= (int)time.DeltaTime;
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