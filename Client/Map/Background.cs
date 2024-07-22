using System.Numerics;
using Client.Actors;
using Client.Net;
using Client.NX;
using Raylib_CsLo;

namespace Client.Map;

public class Background : IActor
{
    private readonly int _cx, _cy, _rx, _ry;
    private readonly bool _animated;
    private readonly List<Texture> _frames;
    private readonly int _frameCount;
    private int _currentFrame;
    private int _delay = 150;
    
    public uint ID { get; set; }
    public string Name { get; set; }
    public int Z { get; set; }
    public bool Visible { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Origin { get; set; }
    public ActorLayer Layer { get; set; }
    public ActorType ActorType { get; set; } = ActorType.Player;
    public Rectangle Bounds { get; set; }
    public NxNode Node { get; set; }
    
    public Background(NxNode node, Texture texture, Vector2 position, Vector2 origin, int cx, int cy, int rx, int ry, ActorLayer layer)
    {
        Node = node;
        _frames = new List<Texture>(1) { texture };
        _frameCount = 1;
        _currentFrame = 0;
        _cx = cx;
        _cy = cy;
        _rx = rx;
        _ry = ry;
        _animated = false;
        Layer = layer;
        Z = 0;
        Position = position;
        Origin = origin;
    }
    
    public Background(NxNode node, List<Texture> frames, Vector2 position, int cx, int cy, int rx, int ry, ActorLayer layer)
    {
        Node = node;
        _frames = frames;
        _frameCount = _frames.Count;
        _currentFrame = 0;
        _cx = cx;
        _cy = cy;
        _rx = rx;
        _ry = ry;
        _animated = _frameCount > 1;
        Layer = layer;
        Z = 0;
        Position = position;
        Origin = Node["0"].GetVector("origin");
    }
    
    public void Clear()
    {
        foreach (var texture in _frames)
            Raylib.UnloadTexture(texture);
        _frames.Clear();
    }

    public void Draw(float frameTime)
    {
        if (_frameCount > 1)
        {
            Origin = Node[$"{_currentFrame}"].GetVector("origin");
            Bounds = new Rectangle(Position.X - Origin.X, Position.Y - Origin.Y, _frames[_currentFrame].width, _frames[_currentFrame].height);
        }
        Raylib.DrawTextureEx(_frames[_currentFrame], Position - Origin, 0.0f, 1.0f, Raylib.WHITE);
        //Raylib.DrawRectangleLinesEx(_bounds, 2.0f, Raylib.RED);
    }

    public void Update(float frameTime)
    {
        if (!_animated)
            return;

        if (_delay <= 0) {
            _currentFrame++;
            if (_currentFrame >= _frameCount)
                _currentFrame = 0;
            _delay = Node[$"{_currentFrame}"].GetInt("delay");
        } else {
            _delay -= (int)frameTime;
        }
    }

    public void ProcessPacket(PacketResponse response)
    {
    }
}