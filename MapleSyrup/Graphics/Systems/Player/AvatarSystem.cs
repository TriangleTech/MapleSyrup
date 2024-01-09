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
        events.Subscribe(this, "PLAYER_ON_SPAWN", OnSpawn);
        events.Subscribe(this, "PLAYER_ON_CHANGE_MAP", OnChangeMap);
        events.Subscribe(this, "PLAYER_ON_DEATH", OnDeath);
        events.Subscribe(this, "PLAYER_UPDATE_LOOK", OnUpdateLook);
        events.Subscribe(this, "PLAYER_UPDATE_FACE", OnUpdateFace);
    }

    private void OnSpawn(EventData eventData)
    {
        var scene = Context.GetSubsystem<SceneSystem>();

        // We don't want to update the player without knowing it's data is registered first!
        var events = Context.GetSubsystem<EventSystem>();
        events.Subscribe(this, "SCENE_RENDER", OnRender);
        events.Subscribe(this, "SCENE_UPDATE", OnUpdate);
    }

    private void OnChangeMap(EventData eventData)
    {
        var events = Context.GetSubsystem<EventSystem>();
        events.Unsubscribe(this, "SCENE_RENDER");
        events.Unsubscribe(this, "SCENE_UPDATE");
    }

    private void OnDeath(EventData eventData)
    {
        var events = Context.GetSubsystem<EventSystem>();
        events.Unsubscribe(this, "SCENE_RENDER");
        events.Unsubscribe(this, "SCENE_UPDATE");
    }

    private void OnUpdateLook(EventData eventData)
    {
        throw new NotImplementedException();
    }

    private void OnUpdateFace(EventData eventData)
    {
        throw new NotImplementedException();
    }

    private void OnUpdateState(EventData eventData)
    {
        
    }

    private void OnRender(EventData eventData)
    {
        var scene = Context.GetSubsystem<SceneSystem>();
        var entities = scene.GetPlayerByName("TestPlayer");
        var camera = scene.Root.GetComponent<Camera>();
        var look = entities.GetComponent<AvatarLook>();
        var transform = entities.GetComponent<Transform>();

        // Does this even need one?
        var bodyMatrix = Matrix.CreateTranslation(-camera.Position.X, -camera.Position.Y, 0f)
                         * Matrix.CreateTranslation(-look.Origin["stand1"]["body"].X, -look.Origin["stand1"]["body"].Y, 0f)
                         * Matrix.CreateScale(new Vector3(1f, 1f, 0f))
                         * Matrix.CreateRotationZ(0f)
                         * Matrix.CreateTranslation(look.Origin["stand1"]["body"].X, look.Origin["stand1"]["body"].Y, 0f);

        // Small explanation: this takes the origin, moves it, then moves it by the "map", then places it back to normal.
        // rotation and scale is for later on.
        var armMatrix = Matrix.CreateTranslation(new Vector3(-camera.Position.X, -camera.Position.Y, 0f))
                        * Matrix.CreateTranslation(-look.Origin["stand1"]["arm"].X, -look.Origin["stand1"]["arm"].Y, 0f)
                        * Matrix.CreateScale(new Vector3(1f, 1f, 0f))
                        * Matrix.CreateRotationZ(0f)
                        * Matrix.CreateTranslation(-look.Map["stand1"]["arm_navel"].X, -look.Map["stand1"]["arm_navel"].Y, 0f)
                        * Matrix.CreateTranslation(look.Origin["stand1"]["arm"].X, look.Origin["stand1"]["arm"].Y, 0f);

        // Looks strange but we'll see, unlike the arm, the head has to be moved positively.
        var headMatrix = Matrix.CreateTranslation(new Vector3(-camera.Position.X, -camera.Position.Y, 0f))
                         * Matrix.CreateTranslation(-look.Origin["stand1"]["head"].X, -look.Origin["stand1"]["head"].Y, 0f)
                         * Matrix.CreateScale(new Vector3(1f, 1f, 0f))
                         * Matrix.CreateRotationZ(0f)
                         * Matrix.CreateTranslation(look.Map["stand1"]["head_neck"].X, look.Map["stand1"]["head_neck"].Y, 0f)
                         * Matrix.CreateTranslation(look.Origin["stand1"]["head"].X, look.Origin["stand1"]["head"].Y, 0f);

        // Draw Body
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap,
            DepthStencilState.Default, RasterizerState.CullNone, null, bodyMatrix);
        spriteBatch.Draw(look.Layers["stand1"]["body"], transform.Position, null, Color.White, 0f,
            Vector2.Zero, 1f, SpriteEffects.None, 0f);
        spriteBatch.End();
        
        // Draw Arm
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap,
            DepthStencilState.Default, RasterizerState.CullNone, null, armMatrix);
        spriteBatch.Draw(look.Layers["stand1"]["arm"], transform.Position, null, Color.White, 0f,
            Vector2.Zero, 1f, SpriteEffects.None, 0f);
        spriteBatch.End();

        // Draw Head
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap,
            DepthStencilState.Default, RasterizerState.CullNone, null, headMatrix);
        spriteBatch.Draw(look.Layers["stand1"]["head"], transform.Position, null, Color.White, 0f,
            Vector2.Zero, 1f, SpriteEffects.None, 0f);
        spriteBatch.End();
    }

    private void OnUpdate(EventData eventData)
    {

    }
}