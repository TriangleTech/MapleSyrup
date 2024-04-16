using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.GameObjects.Components;

public class State
{
    private List<Texture2D> _frames;
    private int _frame;
    private int _delay;
    private int _currentDelay;
    
    public State()
    {
        _frames = new();
    }

    public void AddFrame(int frame, int delay, Texture2D texture)
    {
        _delay = delay;
        _frames.Add(texture);
    }

    public Texture2D GetFrame()
    {
        return _frames[_frame];
    }

    public void UpdateFrame(GameTime gameTime)
    {
        if (!Advance(gameTime)) 
            return;
        
        if (_frame >= _frames.Count - 1)
        {
            _frame = 0;
        }
        else
        {
            _frame++;
        }
    }

    private bool Advance(GameTime gameTime)
    {
        if (_currentDelay <= 0)
        {
            _currentDelay = _delay;
            return true;
        }

        _currentDelay -= gameTime.ElapsedGameTime.Milliseconds;
        return false;
    }

    public void Clear()
    {
        foreach (var tex in _frames)
            tex.Dispose();
        _frames.Clear();
    }
}