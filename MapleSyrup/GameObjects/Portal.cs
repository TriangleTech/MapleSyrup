using MapleSyrup.GameObjects.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.GameObjects;

public class Portal : Actor
{
    private Animation _animation;

    public Animation Animation
    {
        get => _animation;
        init => _animation = value;
    }

    public override void Clear()
    {
        _animation.Clear();
        Node.Dispose();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        lock (_threadLock)
        {
            Origin = Node[$"{_animation.Frame}"]["origin"].GetVector();
            spriteBatch.Draw(_animation.GetFrame(), Position, null, Color.White, 0f, Origin, Vector2.One,
                SpriteEffects.None, 0f);
        }
    }

    public override void Update(GameTime gameTime)
    {
        lock (_threadLock)
        {
            Origin = _animation.UpdateFrame(gameTime) ? Node[$"{_animation.Frame}"]["origin"].GetVector() : Origin;
        }
    }
}