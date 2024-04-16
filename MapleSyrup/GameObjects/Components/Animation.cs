using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.GameObjects.Components;

public class Animation
{
    private int _frame;
    private List<Texture2D> _frames;
    private int _delay;
    private int _currentDelay;

    public int Count => _frames.Count - 1;
    public int Frame => _frame;
    
    public Animation()
    {
        _frame = 0;
        _delay = 0;
        _currentDelay = 0;
        _frames = new();
    }

    public void AddFrame(int delay, Texture2D frame)
    {
        _delay = delay;
        _currentDelay = delay;
        _frames.Add(frame);
    }

    public bool UpdateFrame(GameTime gameTime)
    {
        if (!Advance(gameTime)) 
            return false;
        
        if (_frame >= _frames.Count - 1)
        {
            _frame = 0;
        }
        else
        {
            _frame++;
        }

        return true;
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

    public Texture2D GetFrame() => _frames[_frame];
    
    public void Clear()
    {
        foreach (var tex in _frames)
            tex.Dispose();
        _frames.Clear();
    }
}