using System.Numerics;
using Client.Actors;
using Client.Managers;
using Client.Net;
using Client.NX;
using Raylib_CsLo;

namespace Client.Map;

public class Background : IActor
{
    private byte _currentFrame;
    private int _delay = 150;
    public int ID { get; init; }
    public string Name { get; set; } = string.Empty;
    public int Z { get; set; }
    public bool Visible { get; set; } = true;
    public Vector2 Position { get; set; }
    public Vector2 Origin { get; set; }
    public Vector2 ScreenOffset { get; set; }
    public ActorLayer Layer { get; set; }
    public ActorType ActorType { get; init; } = ActorType.Background;
    
    public Rectangle Bounds { get; set; }
    public Rectangle DstRectangle { get; set; }
    public int Cx { get; init; }
    public int Cy { get; init; }
    public int Rx { get; init; }
    public int Ry { get; init; }
    public int BackgroundType { get; init; }
    public List<Texture> Frames { get; init; } = new();
    public int FrameCount { get; init; }
    public bool Animated { get; init; }
    
    public NxNode Node { get; set; }
    public string TexturePath { get; set; } = String.Empty;
    public int Width { get; internal set; }
    public int Height { get; internal set; }
    
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
            frame.height * AppConfig.ScaleFactor);

        switch (BackgroundType)
        {
            case 0: // Normal.
            case 4: // Horizontal Scrolling (Only gets updated)
                Raylib.DrawTexturePro(frame, Bounds, DstRectangle, Vector2.Zero, 0f, Raylib.WHITE);
                break;
            case 1: // HorizontalTiled
                var world = ServiceLocator.Get<WorldManager>();
                DstRectangle = new Rectangle(
                    ScreenOffset.X,
                    ScreenOffset.Y,
                     world.WorldBounds.width + frame.width * AppConfig.ScaleFactor,
                    frame.height * AppConfig.ScaleFactor);
                Raylib.DrawTextureTiled(frame, Bounds, DstRectangle, Vector2.Zero, 0f, 1.0f, Raylib.WHITE);
                break;
        }
    }

    public void Update(float frameTime)
    {
        Animate(frameTime);
        ShiftBackground(frameTime);
    }

    private void Animate(float frameTime)
    {
        if (!Animated)
            return;

        if (_delay <= 0) {
            _currentFrame++;
            if (_currentFrame >= FrameCount)
                _currentFrame = 0;
            var frameNode = Node[$"{_currentFrame}"];
            _delay = frameNode.Has("delay") ? frameNode.GetInt("delay") : 150;
        } else {
            _delay -= (int)frameTime;
        }
    }

    private void ShiftBackground(float frameTime)
    {
        if (BackgroundType is 0 or 1) return;
        var world = ServiceLocator.Get<WorldManager>();
        var position = Position;
        var frame = Frames[_currentFrame];
        switch (BackgroundType)
        {
            case 4: // Horizontal Scrolling
            {
                if (position.X > Cx + frame.width)
                    position.X = -world.WorldBounds.width - frame.width;
                if (position.X < -Cx - frame.width)
                    position.X = world.WorldBounds.width + frame.width;
                position.X += (Rx * 5) * Raylib.GetFrameTime();
            }
                break;
        }
        Position = position;
    }
    
    public void Clear()
    {
        foreach (var texture in Frames)
            Raylib.UnloadTexture(texture);
        Frames.Clear();
    }
}