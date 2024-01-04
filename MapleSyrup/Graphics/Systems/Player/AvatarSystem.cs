using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.Gameplay.Player;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace MapleSyrup.Graphics.Systems.Player;

public class AvatarSystem
{
    private readonly GameContext Context;
    private readonly SpriteBatch spriteBatch;
    
    public AvatarSystem(GameContext context)
    {
        Context = context;
        spriteBatch = new SpriteBatch(Context.GraphicsDevice);

        var events = Context.GetSubsystem<EventSystem>();
        events.Subscribe(this, "PLAYER_ON_SPAWN", OnPlayerSpawn);
        events.Subscribe(this, "PLAYER_ON_CHANGE_MAP", OnPlayerChangeMap);
        events.Subscribe(this, "PLAYER_ON_DEATH", OnPlayerDeath);
        events.Subscribe(this, "PLAYER_UPDATE_LOOK", OnPlayerUpdateLook);
        events.Subscribe(this, "PLAYER_UPDATE_FACE", OnPlayerUpdateFace);
    }

    private void OnPlayerSpawn(EventData eventData)
    {
        var scene = Context.GetSubsystem<SceneSystem>();
        
        // We don't want to update the player without knowing it's data is registered first!
        var events = Context.GetSubsystem<EventSystem>();
        events.Subscribe(this, "SCENE_RENDER", OnRender);
        events.Subscribe(this, "SCENE_UPDATE", OnUpdate);
    }

    private void OnPlayerChangeMap(EventData eventData)
    {
        var events = Context.GetSubsystem<EventSystem>();
        events.Unsubscribe(this, "SCENE_RENDER");
        events.Unsubscribe(this, "SCENE_UPDATE");
    }

    private void OnPlayerDeath(EventData eventData)
    {
        var events = Context.GetSubsystem<EventSystem>();
        events.Unsubscribe(this, "SCENE_RENDER");
        events.Unsubscribe(this, "SCENE_UPDATE");
    }

    private void OnPlayerUpdateLook(EventData eventData)
    {
        throw new NotImplementedException();
    }

    private void OnPlayerUpdateFace(EventData eventData)
    {
        throw new NotImplementedException();
    }

    private void OnRender(EventData eventData)
    {
        var scene = Context.GetSubsystem<SceneSystem>();
        var entities = scene.GetEntitiesByTag("Player");
        var camera = scene.Root.GetComponent<Camera>();
        
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap,
            DepthStencilState.Default, RasterizerState.CullNone, null, camera.GetViewMatrix());

        for (int i = 0; i < entities.Count; i++)
        {
            if (!entities[i].IsEnabled)
                return;
            var look = entities[i].GetComponent<AvatarLook>();
            var transform = entities[i].GetComponent<Transform>();
            
            spriteBatch.Draw(look.Layers["body"], transform.Position + look.Position["body"], null, Color.White, 0f,
                Vector2.Zero, 1f, SpriteEffects.None, 0f);

            spriteBatch.Draw(look.Layers["arm"], look.Position["body"] + look.Position["arm"], null, Color.White, 0f,
                Vector2.Zero, 1f, SpriteEffects.None, 0f);
            
            spriteBatch.Draw(look.Layers["head"], look.Position["body"] + look.Position["head"], null, Color.White, 0f,
                Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
        
        spriteBatch.End();
    }

    private void OnUpdate(EventData eventData)
    {
        
    }
}