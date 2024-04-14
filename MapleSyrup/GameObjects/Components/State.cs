using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.GameObjects.Components;

public class State
{
    private Dictionary<int, Texture2D> _frames;
    private int _frame;
    private int _frameCount;
    
    public State()
    {
        _frames = new();
    }

    public bool AddFrame(int frame, Texture2D texture)
    {
        return _frames.TryAdd(frame, texture);
    }

    public Texture2D GetFrame()
    {
        return _frames[_frame];
    }

    public void UpdateFrame()
    {
        if (_frame >= _frames.Count - 1)
        {
            _frame = 0;
        }
        else
        {
            _frame++;
        }
    }

    public void Clear()
    {
        foreach (var (_, tex) in _frames)
            tex.Dispose();
        _frames.Clear();
    }
}