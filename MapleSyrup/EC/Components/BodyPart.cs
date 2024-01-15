using MapleSyrup.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.EC.Components;

public class BodyPart : IComponent
{
    public IEntity Parent { get; }
    public ComponentFlag Flag { get; }
    public Texture2D Texture { get; set; }
    public BodyType BodyType { get; }

    private Vector2 _origin;
    private Vector2 _invertedOrigin;
    private Matrix _matrix;
    private List<(int frame, Vector2 origin, Texture2D texture)> _frames;
    private Dictionary<(int frame, BodyMap map), Vector2> _maps;
    private List<int> _delays;
    private int _currentFrame = 0;
    private int _delay = 100;

    public BodyPart(IEntity parent, BodyType type)
    {
        _frames = new();
        _delays = new();
        _maps = new();
        Parent = parent;
        BodyType = type;
        Flag = ComponentFlag.BodyPart;
    }

    public void UpdateMatrix()
    {
        switch (BodyType)
        {
            case BodyType.Body:
                _matrix = Matrix.CreateTranslation(new Vector3(-Parent.Transform.Position, 0)) *
                          Matrix.CreateTranslation(new Vector3(-_origin, 0.0f)) *
                          Matrix.CreateRotationZ(0f) *
                          Matrix.CreateScale(1f, 1f, 0f) *
                          Matrix.CreateTranslation(new Vector3(_origin, 0.0f));
                _invertedOrigin = _origin;
                break;
            case BodyType.Arm:
                _matrix = Matrix.CreateTranslation(new Vector3(-Parent.Transform.Position, 0)) *
                          Matrix.CreateTranslation(new Vector3(-_origin, 0f)) *
                          Matrix.CreateScale(1f, 1f, 0f) *
                          Matrix.CreateRotationZ(0f) *
                          Matrix.CreateTranslation(new Vector3(_maps[(_currentFrame, BodyMap.Navel)], 0f)) *
                          Matrix.CreateTranslation(new Vector3(_origin, 0f));
                _invertedOrigin = Vector2.Transform(Vector2.Zero, _matrix);
                break;
            case BodyType.Head:
                _matrix = Matrix.CreateTranslation(new Vector3(-Parent.Transform.Position, 0)) *
                          Matrix.CreateTranslation(new Vector3(-_origin, 0f)) *
                          Matrix.CreateRotationZ(0f) *
                          Matrix.CreateTranslation(new Vector3(-_maps[(_currentFrame, BodyMap.NeckBody)], 0f)) *
                          Matrix.CreateScale(1f, 1f, 0f) *
                          Matrix.CreateTranslation(new Vector3(_origin, 0f));
                _invertedOrigin = Vector2.Transform(Vector2.Zero, _matrix);
                break;
        }
    }

    public Vector2 GetOrigin()
    {
        return _invertedOrigin;
    }
    
    public void AddFrame(int frame, Vector2 _origin, Texture2D texture)
    {
        _frames.Add((frame, _origin, texture));
    }

    public void AddBodyMap(int frame, BodyMap map, Vector2 pos)
    {
        _maps[(frame, map)] = pos;
    }

    public void AddDelay(int delay)
    {
        _delays.Add(delay);
    }

    public void NextFrame()
    {
        if (_currentFrame >= _frames.Count - 1)
        {
            _currentFrame = 0;
            _origin = _frames[0].origin;
            Texture = _frames[0].texture;
        }
        
        _currentFrame++;
        _origin = _frames[_currentFrame].origin;
        Texture = _frames[_currentFrame].texture;
    }

    public bool Advance(float timeDelta)
    {
        if (_delay <= 0)
        {
            _delay = _delays[_currentFrame];
            return true;
        }

        _delay -= (int)timeDelta;
        return false;
    }

    public void Clear()
    {
        Texture.Dispose();
        foreach (var (_, _, frame) in _frames)
            frame.Dispose();
    }
}