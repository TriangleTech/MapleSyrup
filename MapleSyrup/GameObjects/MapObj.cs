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
        
    }

    public override void Update(GameTime gameTime)
    {
        if (!Animated)
            return;
        _animation.UpdateFrame(gameTime);
    }
}