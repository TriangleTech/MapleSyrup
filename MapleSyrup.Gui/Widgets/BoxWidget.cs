using System.Numerics;
using MapleSyrup.Gui.Containers;
using ZeroElectric.Vinculum;

namespace MapleSyrup.Gui.Widgets;

public class BoxWidget : Widget
{
    public Color BoxColor { get; set; } = Raylib.RED;
    public BoxWidget(string identifier, int width, int height) 
        : base(identifier, width, height)
    {
    }

    public override void Draw()
    {
        base.Draw();
        Raylib.DrawRectangleRec(Bounds, BoxColor);
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
    }
}