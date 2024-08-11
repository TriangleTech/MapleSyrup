using System.Numerics;
using Client.Actors;
using Client.Net;
using Client.NX;
using Raylib_CsLo;

namespace Client.Map;

public class MapObject : IActor
{
    private int _currentFrame = 0;
    private int _delay = 150;
    private bool _alphaSwap = true;
    public int ID { get; init; }

    public string Name { get; set; } = string.Empty;
    public int Z { get; set; }
    public bool Visible { get; set; } = true;
    public Vector2 Position { get; set; }
    public Vector2 Origin { get; set; }
    public Vector2 ScreenOffset { get; set; }
    public ActorLayer Layer { get; set; }
    public ActorType ActorType { get; init; } = ActorType.StaticMapObject;
    public Rectangle Bounds { get; set; }
    public Rectangle DstRectangle { get; set; }
    public List<Texture> Frames { get; internal set; }
    public int FrameCount { get; init; }
    public bool Animated { get; init; }
    public bool Blend { get; init; }
    public int LoopCount { get; set; }
    public int Alpha { get; internal set; } = 255;
    public int LowerAlpha { get; init; }
    public int UpperAlpha { get; init; }
    public NxNode Node { get; set; }
    public string TexturePath { get; init; } = string.Empty;
    //public Rectangle DesRectangle { get; private set; }

    
    public void Draw(float frameTime)
    {
        var frame = Frames[_currentFrame];
        if (FrameCount > 1) Origin = Node[$"{_currentFrame}"].GetVector("origin");
        Bounds = new Rectangle(0, 0, frame.width, frame.height);
        ScreenOffset = new Vector2((Position.X - Origin.X) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetX,
            (Position.Y - Origin.Y) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetY);
        DstRectangle = new Rectangle(
            ScreenOffset.X, 
            ScreenOffset.Y, 
            frame.width * AppConfig.ScaleFactor, 
            frame.height * AppConfig.ScaleFactor); // TODO: create event system to do this there instead

        if (Blend) Raylib.BeginBlendMode(BlendMode.BLEND_ALPHA);
        
        Raylib.DrawTexturePro(frame, Bounds, DstRectangle, Vector2.Zero, 0f, new Color(255, 255, 255, Alpha));

        //Raylib.DrawTextureEx(frame, Position - Origin, 0.0f, 1.0f, new Color(255, 255, 255, Alpha));
        if (Blend) Raylib.EndBlendMode();
    }

    public void Update(float frameTime)
    {
        LoopAnimation(frameTime);
        OneTimeAnimation(frameTime);
        BlendAnimation(frameTime);
    }

    private void LoopAnimation(float frameTime)
    {
        if (!Animated || Blend || LoopCount == -1)
            return;

        if (_delay <= 0) {
            _currentFrame++;
            if (_currentFrame >= FrameCount)
                _currentFrame = 0;
            _delay = Node[$"{_currentFrame}"].Has("delay") ? Node[$"{_currentFrame}"].GetInt("delay") : 150;
        } else {
            _delay -= (int)frameTime;
        }
    }

    private void OneTimeAnimation(float frameTime)
    {
        if (LoopCount <= 0)
            return;

        if (_delay <= 0) {
            _currentFrame++;
            if (_currentFrame >= FrameCount)
            {
                _currentFrame = 0;
                LoopCount--;
            }
            _delay = Node[$"{_currentFrame}"].GetInt("delay");
        } else {
            _delay -= (int)frameTime;
        }
    }

    private void BlendAnimation(float frameTime)
    {
        if (!Blend)
            return;
        if (_alphaSwap)
        {
            if (_delay <= 0) {
                Alpha -= (int)frameTime;                
                if (Alpha <= LowerAlpha)
                {
                    _alphaSwap = false;
                    Alpha = LowerAlpha;
                    _delay = Node[$"{_currentFrame}"].GetInt("delay");
                }
            } else {
                _delay -= (int)frameTime;
            }
        }
        else
        {
            if (_delay <= 0) {
                Alpha += (int)frameTime;                
                if (Alpha >= UpperAlpha)
                {
                    _alphaSwap = true;
                    Alpha = UpperAlpha;
                    _delay = Node[$"{_currentFrame}"].GetInt("delay");
                }
            } else {
                _delay -= (int)frameTime;
            }
        }
    }

    public void Clear()
    {
        foreach (var frame in Frames)
            Raylib.UnloadTexture(frame);
    }
}