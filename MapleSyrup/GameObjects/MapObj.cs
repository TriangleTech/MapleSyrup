using MapleSyrup.GameObjects.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.GameObjects;

public class MapObj : Actor
{
    private Animation _animation;

    public Animation Animation
    {
        get => _animation;
        set => _animation = value;
    }

    public bool Animated => Animation.Count > 1;
    
    public override void Clear()
    {
        _animation.Clear();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        lock (_threadLock)
        {
            if (Animated)
                Origin = Node[$"{_animation.Frame}"]["origin"].GetVector();
            spriteBatch.Draw(_animation.GetFrame(), Position, null, Color.White, 0f, Origin, Vector2.One,
                SpriteEffects.None, 0f);
        }
    }

    public override void Update(GameTime gameTime)
    {
        lock(_threadLock)
        {
            if (Animated)
                _animation.UpdateFrame(gameTime);
        }
    }
}