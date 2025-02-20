using ZeroElectric.Vinculum;

namespace MapleSyrup.Gui.Containers;

public class BoxContainer : Container
{
    public float Roundness { get; init; }
    
    public BoxContainer(string name, int width, int height) 
        : base(name, width, height)
    {
    }

    public override void Draw()
    {
        base.Draw();
        Raylib.DrawRectangleRounded(Bounds, Roundness, 1, new Color(BackgroundColor.r, BackgroundColor.g, BackgroundColor.b, Alpha));
        foreach (var (_, widget) in Widgets)
            widget.Draw();
    }
}