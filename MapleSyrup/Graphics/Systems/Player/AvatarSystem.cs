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
        var entities = scene.GetPlayerByName("TestPlayer");
        var camera = scene.Root.GetComponent<Camera>();

        //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap,
        //DepthStencilState.Default, RasterizerState.CullNone, null, camera.Transform);

        var look = entities.GetComponent<AvatarLook>();
        var transform = entities.GetComponent<Transform>();

        var bodyMatrix = Matrix.CreateTranslation(-camera.Position.X, -camera.Position.Y, 0f)
                         * Matrix.CreateTranslation(-look.Origin["body"].X, -look.Origin["body"].Y, 0f)
                         * Matrix.CreateScale(new Vector3(1f, 1f, 0f))
                         * Matrix.CreateRotationZ(0f)
                         * Matrix.CreateTranslation(look.Origin["body"].X, look.Origin["body"].Y, 0f);

        // Small explanation: this takes the origin, moves it, then moves it by the "map", then places it back to normal.
        // rotation and scale is for later on.
        var armMatrix = Matrix.CreateTranslation(new Vector3(-camera.Position.X, -camera.Position.Y, 0f))
                        * Matrix.CreateTranslation(-look.Origin["arm"].X, -look.Origin["arm"].Y, 0f)
                        * Matrix.CreateScale(new Vector3(1f, 1f, 0f))
                        * Matrix.CreateRotationZ(0f)
                        * Matrix.CreateTranslation(-look.Map["arm_navel"].X, -look.Map["arm_navel"].Y, 0f)
                        * Matrix.CreateTranslation(look.Origin["arm"].X, look.Origin["arm"].Y, 0f);

        // Looks strange but we'll see, unlike the arm, the head has to be moved positively.
        var headMatrix = Matrix.CreateTranslation(new Vector3(-camera.Position.X, -camera.Position.Y, 0f))
                         * Matrix.CreateTranslation(-look.Origin["head"].X, -look.Origin["head"].Y, 0f)
                         * Matrix.CreateScale(new Vector3(1f, 1f, 0f))
                         * Matrix.CreateRotationZ(0f)
                         * Matrix.CreateTranslation(look.Map["head_neck"].X, look.Map["head_neck"].Y, 0f)
                         * Matrix.CreateTranslation(look.Origin["head"].X, look.Origin["head"].Y, 0f);

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap,
            DepthStencilState.Default, RasterizerState.CullNone, null, bodyMatrix);
        spriteBatch.Draw(look.Layers["body"], transform.Position, null, Color.White, 0f,
            Vector2.Zero, 1f, SpriteEffects.None, 0f);
        spriteBatch.End();

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap,
            DepthStencilState.Default, RasterizerState.CullNone, null, armMatrix);
        spriteBatch.Draw(look.Layers["arm"], transform.Position, null, Color.White, 0f,
            Vector2.Zero, 1f, SpriteEffects.None, 0f);
        spriteBatch.End();

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap,
            DepthStencilState.Default, RasterizerState.CullNone, null, headMatrix);
        spriteBatch.Draw(look.Layers["head"], transform.Position, null, Color.White, 0f,
            Vector2.Zero, 1f, SpriteEffects.None, 0f);
        spriteBatch.End();
    }

    private void OnUpdate(EventData eventData)
    {

    }
}