using System.Numerics;
using Client.Actors;
using Client.Net;
using Client.NX;
using Raylib_CsLo;

namespace Client.Map;

public class MapObject : IActor
{
    private readonly List<Texture> _frames;
    private readonly int _frameCount;
    private readonly bool _animated, _blended;
    private bool _timedAnimation;
    private int _loopCount;
    private int _alpha, _lowLimit, _highLimit;
    private bool _alphaSwap = true;

    private int _currentFrame;
    private int _delay;
    
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

    public MapObject(NxNode node, Texture texture, Vector2 position, Vector2 origin, ActorLayer layer, int order)
    {
        Node = node;
        _frames = new List<Texture>(1) { texture };
        _frameCount = 1;
        _currentFrame = 0;
        _animated = false;
        Position = position;
        Origin = origin;
        Layer = layer;
        Z = order;
        Bounds = new Rectangle(Position.X, Position.Y, texture.width, texture.height);
        _alpha = 255;
        _blended = Node.Has("0") && Node["0"].Has("a0");
    }

    public MapObject(NxNode node, List<Texture> textures, Vector2 position, ActorLayer layer, int order)
    {
        Node = node;
        _currentFrame = 0;
        _frames = textures;
        _frameCount = _frames.Count - 1;
        _animated = _frameCount > 1;
        if (Node["0"].Has("a0"))
        {
            _blended = true;
            _lowLimit = Node["0"].GetInt("a0");
            _highLimit = Node["0"].GetInt("a1");
        }
        else
        {
            _blended = false;
            _lowLimit = 0;
            _highLimit = 0;
        }
        _timedAnimation = Node["0"].Has("repeat") && (_loopCount =Node["0"].GetInt("repeat")) > -1;
        Position = position;
        Layer = layer;
        Z = order;
        Origin = Node["0"].GetVector("origin");
        _delay = Node["0"].GetInt("delay");
        Bounds = new Rectangle(Position.X, Position.Y, _frames[0].width, _frames[0].height);
        _alpha = 255;
    }

    public void Clear()
    {
        foreach (var frame in _frames)
            Raylib.UnloadTexture(frame);
    }

    public void Draw(float frameTime)
    {
        if (_frameCount > 1)
        {
            Origin = Node[$"{_currentFrame}"].GetVector("origin");
            Bounds = new Rectangle(Position.X - Origin.X, Position.Y - Origin.Y, _frames[_currentFrame].width, _frames[_currentFrame].height);
        }

        Raylib.DrawTextureEx(_frames[_currentFrame], Position - Origin, 0.0f, 1.0f, new Color(245, 245, 245, _alpha));
    }

    public void Update(float frameTime)
    {
        LoopAnimation(frameTime);
        OneTimeAnimation(frameTime);
        BlendAnimation(frameTime);
    }

    private void LoopAnimation(float frameTime)
    {
        if (!_animated || _timedAnimation || _blended)
            return;

        if (_delay <= 0) {
            _currentFrame++;
            if (_currentFrame >= _frameCount)
                _currentFrame = 0;
            _delay = Node[$"{_currentFrame}"].Has("delay") ? Node[$"{_currentFrame}"].GetInt("delay") : 150;
        } else {
            _delay -= (int)frameTime;
        }
    }

    private void OneTimeAnimation(float frameTime)
    {
        if (!_timedAnimation)
            return;

        if (_delay <= 0) {
            _currentFrame++;
            if (_currentFrame >= _frameCount)
            {
                _currentFrame = 0;
                _loopCount--;
                _timedAnimation = _loopCount <= 0;
            }
            _delay = Node[$"{_currentFrame}"].GetInt("delay");
        } else {
            _delay -= (int)frameTime;
        }
    }

    private void BlendAnimation(float frameTime)
    {
        if (!_blended)
            return;
        if (_alphaSwap)
        {
            if (_delay <= 0) {
                _alpha -= (int)frameTime;                
                if (_alpha <= _lowLimit)
                {
                    _alphaSwap = false;
                    _alpha = _lowLimit;
                    _delay = Node[$"{_currentFrame}"].GetInt("delay");
                }
            } else {
                _delay -= (int)frameTime;
            }
        }
        else
        {
            if (_delay <= 0) {
                _alpha += (int)frameTime;                
                if (_alpha >= _highLimit)
                {
                    _alphaSwap = true;
                    _alpha = _highLimit;
                    _delay = Node[$"{_currentFrame}"].GetInt("delay");
                }
            } else {
                _delay -= (int)frameTime;
            }
        }
    }

    public void ProcessPacket(PacketResponse response)
    {
        
    }
}