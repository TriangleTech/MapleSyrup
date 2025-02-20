using System.Numerics;
using MapleSyrup.Gui.Containers;
using ZeroElectric.Vinculum;

namespace MapleSyrup.Gui.Widgets;

public abstract class Widget
{
    public Container Parent { get; set; }
    public string Identifier { get; init; }
    public int Width;
    public int Height;
    public Vector2 Position;
    public Rectangle Bounds { get; protected set; }
    public bool IsVisible = true;

    public Widget(string identifier, int width, int height)
    {
        Identifier = identifier;
        Width = width;
        Height = height;
        Bounds = new Rectangle(Position.X, Position.Y, width, height);
    }

    public virtual void Draw()
    {
        if (!IsVisible) return;
    }

    public virtual void Update(float deltaTime)
    {
        if (!IsVisible) return;
        var x = Parent.Position.X + Position.X;
        var y = Parent.Position.Y + Position.Y;
        if (x > Parent.Position.X + Parent.Width - Width)
            x = Parent.Position.X + Parent.Width - Width;
        else if (x < Parent.Position.X)
            x = Parent.Position.X;
        if (y > Parent.Position.Y + Parent.Height - Height)
            y = Parent.Position.Y + Parent.Height - Height;
        else if (y < Parent.Position.Y)
            y = Parent.Position.Y;
        
        Bounds = new Rectangle(x, y, Width, Height);
    }
}