using System.Numerics;
using MapleSyrup.Gui.Widgets;
using ZeroElectric.Vinculum;

namespace MapleSyrup.Gui.Containers;

public abstract class Container
{
    public string Identifier { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }
    public Vector2 Position;
    public Rectangle Bounds { get; protected set; }
    public Color BackgroundColor = Raylib.BLACK;
    public int Alpha = 255;
    public bool IsVisible = true;
    public bool IsMoveable = true;
    protected Dictionary<string, Widget> Widgets;

    public Container(string name, int width, int height)
    {
        Identifier = name;
        Width = width;
        Height = height;
        Position = new Vector2(Raylib.GetScreenWidth() / 4f, Raylib.GetScreenHeight() / 4f);
        Bounds = new Rectangle(Position.X, Position.Y, width, height);
        Widgets = new Dictionary<string, Widget>();
    }

    public void AddWidget(Widget widget)
    {
        if (widget.Width > Width)
            widget.Width = Width;
        if (widget.Height > Height)
            widget.Height = Height;
        widget.Parent = this;
        Widgets.Add(widget.Identifier, widget);
    }

    public virtual void Draw()
    {
        if (!IsVisible || !IsMoveable) return;
    }

    public virtual void Update(float timeDelta)
    {
        if (!IsVisible || !IsMoveable) return;
        var mousePosition = Raylib.GetMousePosition();
        if (Raylib.CheckCollisionPointRec(mousePosition, Bounds))
        {
            if (Raylib.IsMouseButtonDown(Raylib.MOUSE_LEFT_BUTTON))
            {
                var mouseDelta = Raylib.GetMouseDelta();
                var x = Position.X + mouseDelta.X;
                var y = Position.Y + mouseDelta.Y;
                Position = new Vector2(x, y);
                Bounds = new Rectangle(Position.X, Position.Y, Width, Height);
            }
        }
        
        foreach (var (_, widget) in Widgets)
            widget.Update(timeDelta);
    }
}