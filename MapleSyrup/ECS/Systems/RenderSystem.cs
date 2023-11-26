using MapleSyrup.ECS.Components;
using MapleSyrup.Gameplay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace MapleSyrup.ECS.Systems;

public class RenderSystem : IDrawSystem
{
    private SpriteBatch batch;

    public void Initialize(GraphicsDevice graphicsDevice)
    {
        batch = new SpriteBatch(graphicsDevice);
    }

    public void Draw(MapleWorld world)
    {
        var entities = world.GetObjectsWithComponent<SpriteComponent>();
        
        batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
        foreach (var gameObject in entities)
        {
            var transform = gameObject.GetComponent<TransformComponent>();
            var sprite = gameObject.GetComponent<SpriteComponent>();
            batch.Draw(sprite.Texture, transform.Position, null, Color.White, 0f, 
                transform.Origin, Vector2.One, SpriteEffects.None, 0f);
        }
        batch.End();
    }

    public void Shutdown()
    {
        batch.Dispose();
    }
}