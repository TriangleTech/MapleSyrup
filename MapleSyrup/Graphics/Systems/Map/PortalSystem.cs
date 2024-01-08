using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.ECS;
using MapleSyrup.Gameplay.Map;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace MapleSyrup.Graphics.Systems.Map;

public class PortalSystem
{
    private readonly GameContext Context;
    private readonly SpriteBatch spriteBatch;
    
    public PortalSystem(GameContext context)
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
        var camera = scene.Root.GetComponent<Camera>();
        var entities = scene.GetEntitiesByTag("Portal");
        
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap,
            DepthStencilState.Default, RasterizerState.CullNone, null, camera.Transform);

        for (var i = 0; i < entities.Count; i++)
        {
            if (!entities[i].IsEnabled || !entities[i].HasComponent<PortalInfo>() || !entities[i].HasComponent<Portal>())
                continue;
            var portal = entities[i].GetComponent<Portal>();
            var info = entities[i].GetComponent<PortalInfo>();
            var transform = entities[i].GetComponent<Transform>();
            
            // TODO: Handle hidden and scripted portals later
            if (portal.IsHidden)
                continue;
            
            spriteBatch.Draw(portal.Frames[portal.CurrentFrame], transform.Position, null, Color.White,
                0f, portal.Origins[portal.CurrentFrame], 1f, SpriteEffects.None, 0f);
        }
        
        spriteBatch.End();
    }

    private void OnUpdate(EventData eventData)
    {
        var scene = Context.GetSubsystem<SceneSystem>();
        var entities = scene.GetEntitiesByTag("Portal");

        for (var i = 0; i < entities.Count; i++)
        {
            if (!entities[i].IsEnabled || !entities[i].HasComponent<PortalInfo>() || !entities[i].HasComponent<Portal>())
                continue;
            var portal = entities[i].GetComponent<Portal>();
            
            // TODO: Handle hidden and scripted portals later
            if (portal.IsHidden)
                continue;
            
            var next = i;
            Task.Run(() => UpdateAnimation(entities[next]));
        }
    }

    private void UpdateAnimation(Entity entity)
    {
        var time = Context.GetSubsystem<TimeSystem>();
        var portal = entity.GetComponent<Portal>();
        
        if (portal.CurrentFrame >= portal.Frames.Count - 1)
        {
            portal.CurrentFrame = 0;
            portal.CurrentDelay = 100;
        }
        
        if (portal.CurrentDelay <= 0)
        {
            portal.CurrentFrame++;
            portal.CurrentDelay = 100;
        }
        else
        {
            portal.CurrentDelay -= (int)time.DeltaTime;
        }
    }
}