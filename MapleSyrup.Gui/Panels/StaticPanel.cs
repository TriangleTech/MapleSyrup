using ZeroElectric.Vinculum;

namespace MapleSyrup.Gui.Panels;

public class StaticPanel : PanelBase
{
    public bool HasHeader { get; init; }
    public Color HeaderColor { get; init; }
    public required Color BackgroundColor { get; init; }
    
    public StaticPanel(string name) 
        : base(name)
    {
        Movable = false;
    }

    public override void Draw()
    {
        Bounds = new Rectangle(Position.X, Position.Y, Width, Height);
        Raylib.DrawRectangleRounded(Bounds, 0.3f, 4, BackgroundColor);
        if (HasHeader)
            Raylib.DrawRectangleRounded(new Rectangle(Position.X, Position.Y, Width, 35), 0.1f, 4, HeaderColor);
        
        foreach (var (_, widget) in Widgets)
            widget.Draw();
    }
}