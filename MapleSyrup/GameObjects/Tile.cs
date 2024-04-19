using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.GameObjects;

public class Tile : Actor
{
    private Texture2D _texture;

    public Texture2D Texture
    {
        get => _texture;
        set => _texture = value;
    }

    public override void Clear()
    {
        _texture.Dispose();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        lock (_threadLock)
        {
            spriteBatch.Draw(_texture, Position, null, Color.White, 0f, Origin, Vector2.One, SpriteEffects.None, 0f);
        }
    }

    public override void Update(GameTime gameTime)
    {
    }
}