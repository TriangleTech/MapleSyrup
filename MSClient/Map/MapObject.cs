using System.Numerics;
using MSClient.Actors;
using MSClient.Net;
using MSClient.NX;
using Raylib_CsLo;

namespace MSClient.Map;

public class MapObject : Actor
{
    private readonly List<Texture> _frames;
    private readonly int _frameCount;
    private readonly bool _animated;

    private int _currentFrame;
    private int _delay;

    public MapObject(ref NxNode node, Texture texture, Vector2 position, Vector2 origin, ActorLayer layer, int order)
        : base(node, ActorType.MapObject)
    {
        _node = node;
        _frames = new List<Texture>(1) { texture };
        _frameCount = 1;
        _currentFrame = 0;
        _animated = false;
        _position = position;
        _origin = origin;
        _layer = layer;
        _zIndex = order;
        _bounds = new Rectangle(Position.X, Position.Y, texture.width, texture.height);
    }

    public MapObject(ref NxNode node, List<Texture> textures, Vector2 position, ActorLayer layer, int order)
        : base(node, ActorType.MapObject)
    {
        _node = node;
        _currentFrame = 0;
        _frames = textures;
        _frameCount = _frames.Count;
        _animated = true;
        _position = position;
        _layer = layer;
        _zIndex = order;
        _origin = _node["0"].GetVector("origin");
        _delay = _node["0"].GetInt("delay");
        _bounds = new Rectangle(Position.X, Position.Y, _frames[0].width, _frames[0].height);
    }

    public override void Clear()
    {
        foreach (var frame in _frames)
            Raylib.UnloadTexture(frame);
    }

    public override void Draw(float frameTime)
    {
        if (_frameCount > 1)
        {
            _origin = _node[$"{_currentFrame}"].GetVector("origin");
            _bounds = new Rectangle(Position.X, Position.Y, _frames[_currentFrame].width, _frames[_currentFrame].height);
        }

        Raylib.DrawTextureEx(_frames[_currentFrame], Position, 0.0f, 1.0f, Raylib.WHITE);
        //Raylib.DrawRectangleLinesEx(_bounds, 2.0f, Raylib.RED);
    }

    public override void Update(float frameTime)
    {
        if (!_animated)
            return;

        if (_delay <= 0) {
            _currentFrame++;
            if (_currentFrame >= _frameCount)
                _currentFrame = 0;
            _delay = _node[$"{_currentFrame}"].GetInt("delay");
        } else {
            _delay -= (int)frameTime;
        }
    }

    public override void ProcessPacket(PacketResponse response)
    {
        
    }
}