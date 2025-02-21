using ZeroElectric.Vinculum;

namespace MapleSyrup.Gui.Widgets;

public class Label : WidgetBase
{
    public required string Text { get; init; }
    public required float FontSize { get; init; }
    public required Color FontColor { get; init; }
    
    public Label(string name) 
        : base(name)
    {
        Movable = false;
    }

    public override void Draw()
    {
        Raylib.DrawText(Text, Bounds.X, Bounds.Y, FontSize, FontColor);
    }
}