using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.ECS;
using MapleSyrup.Gameplay.Map;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MapleSyrup.Graphics.Systems.Map;

public class MapSystem
{
    private readonly GameContext Context;
    private readonly SpriteBatch spriteBatch;
    
    public MapSystem(GameContext context)
    {
        Context = context;
        spriteBatch = new SpriteBatch(Context.GraphicsDevice);

        var events = Context.GetSubsystem<EventSystem>();
        events.Subscribe(this, "RENDER_BACKGROUND", OnRenderBackground);
        events.Subscribe(this, "RENDER_TILEOBJ", OnRenderTileObj);
        //events.Subscribe(this, "RENDER_FOREGROUND", OnRender);
        events.Subscribe(this, "UPDATE_BACKGROUND", OnUpdateBackground);
        events.Subscribe(this, "UPDATE_TILEOBJ", OnUpdateTileObj);
        //events.Subscribe(this, "UPDATE_FOREGROUND", OnRender);
    }

    private void OnRenderBackground(EventData eventData)
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

    private void OnRenderTileObj(EventData eventData)
    {
        var scene = Context.GetSubsystem<SceneSystem>();
        var entities = scene.GetEntitiesByTag("MapItem");
        var camera = scene.Root.GetComponent<Camera>();
        
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

    private void OnUpdateBackground(EventData eventData)
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

    private void OnUpdateTileObj(EventData eventData)
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