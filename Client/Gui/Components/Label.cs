using System.Numerics;
using Client.Gui.Panels;
using Client.NX;
using Raylib_CsLo;

namespace Client.Gui.Components;

public class Label : IUIComponent
{
    public IUIPanel Parent { get; init; }
    public int ID { get; init; }
    public string Name { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 ScreenOffset { get; set; }
    public Rectangle Bounds { get; set; }
    public Rectangle DstRectangle { get; set; }
    public NxNode? Node { get; init; }
    public Action? Callback { get; init; }
    public bool Active { get; set; }
    public string Text { get; init; } = string.Empty;
    public int FontSize { get; init; } = 16;
    public Color Color { get; init; } = Raylib.BLACK;
    public string TexturePath { get; set; } = string.Empty;

    public void Draw(Vector2 parentPos, float frameTime)
    {
        ScreenOffset = new Vector2(parentPos.X + Position.X, parentPos.Y + Position.Y);
        Raylib.DrawText(Text, ScreenOffset.X, ScreenOffset.Y, FontSize, Color);
    }

    public void Update(Vector2 parentPos, float frameTime)
    {
    }
    
    public void Clear()
    {
        
    }
}