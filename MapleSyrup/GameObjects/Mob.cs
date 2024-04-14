using MapleSyrup.GameObjects.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.GameObjects;

public class Mob : Actor
{
    private readonly StateMachine _state;

    public StateMachine StateMachine => _state;

    public Mob()
    {
        _state = new(this);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var tex = _state.State.GetFrame();
        spriteBatch.Draw(tex, Vector2.Zero, Color.White);
    }

    public override void Update(GameTime gameTime)
    {
        _state.State.UpdateFrame();
    }

    public override void Clear()
    {
        _state.Clear();
    }
}