using System.Numerics;
using MSClient.Actors;
using MSClient.Net;
using MSClient.NX;
using Raylib_CsLo;

namespace MSClient.Map;

public class Background : Actor
{
    private readonly int _cx, _cy, _rx, _ry;
    private readonly bool _animated;
    private readonly List<Texture> _frames;
    private readonly int _frameCount;
    private int _currentFrame;
    private int _delay = 150;
    
    public Background(ref NxNode node, Texture texture, Vector2 position, Vector2 origin, int cx, int cy, int rx, int ry, ActorLayer layer)
        : base(ref node)
    {
        _node = node;
        _frames = new List<Texture>(1) { texture };
        _frameCount = 1;
        _currentFrame = 0;
        _cx = cx;
        _cy = cy;
        _rx = rx;
        _ry = ry;
        _animated = false;
        _layer = layer;
        _zIndex = 0;
        _position = position;
        _origin = origin;
    }
    
    public Background(ref NxNode node, List<Texture> frames, Vector2 position, int cx, int cy, int rx, int ry, ActorLayer layer)
        : base(ref node)
    {
        _node = node;
        _frames = frames;
        _frameCount = _frames.Count - 1;
        _currentFrame = 0;
        _cx = cx;
        _cy = cy;
        _rx = rx;
        _ry = ry;
        _animated = true;
        _layer = layer;
        _zIndex = 0;
        _position = position;
        _origin = _node["0"].GetVector("origin");
    }
    
    public override void Clear()
    {
        foreach (var texture in _frames)
            Raylib.UnloadTexture(texture);
        _frames.Clear();
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
        
        if (_currentFrame >= _frameCount) {
            _currentFrame = 0;
            _delay = _node["0"].GetInt("delay");
            return;
        }

        if (_delay <= 0) {
            _currentFrame++;
            _delay = _node[$"{_currentFrame}"].GetInt("delay");
        } else {
            _delay -= (int)frameTime;
        }
    }

    public override void ProcessPacket(PacketResponse response)
    {
    }
}