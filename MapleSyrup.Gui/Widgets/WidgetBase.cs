using System.Numerics;
using MapleSyrup.Gui.Enums;
using MapleSyrup.Gui.Panels;
using ZeroElectric.Vinculum;

namespace MapleSyrup.Gui.Widgets;

public abstract class WidgetBase
{
    public PanelBase Parent { get; set; }
    public string Name { get; }
    public required int Width { get; init; }
    public required int Height { get; init; }
    public required Vector2 Position { get; set; }
    public bool Visible { get; set; } = true;
    public bool Movable { get; init; }
    public Rectangle Bounds { get; protected set; }
    public WidgetState State { get; set; } = WidgetState.Normal;

    public WidgetBase(string name)
    {
        Name = name;
    }
    
    public abstract void Draw();
    public virtual void Update(float deltaTime)
    {
        if (!Visible) 
            return;
        
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