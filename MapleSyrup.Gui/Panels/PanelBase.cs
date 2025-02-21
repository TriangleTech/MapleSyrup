using System.Numerics;
using MapleSyrup.Gui.Enums;
using MapleSyrup.Gui.Widgets;
using ZeroElectric.Vinculum;

namespace MapleSyrup.Gui.Panels;

public abstract class PanelBase
{
    public string Name { get; }
    public required int Width { get; init; }
    public required int Height { get; init; }
    public bool Visible { get; set; }
    public bool Movable { get; init; }
    public Rectangle Bounds { get; protected set; }
    public Vector2 Position { get; set; }
    protected Dictionary<string, WidgetBase> Widgets;
    private string _activeWidget = string.Empty;

    public PanelBase(string name)
    {
        Name = name;
        Widgets = new Dictionary<string, WidgetBase>();
    }

    public void AddWidget(WidgetBase widget)
    {
        widget.Parent = this;
        Widgets.Add(widget.Name, widget);
    }

    public void RemoveWidget(WidgetBase widget)
    {
        if (!Widgets.ContainsKey(widget.Name))
            return;
        Widgets.Remove(widget.Name);
    }

    public void SetActive(WidgetBase widget)
    {
        if (widget.Name == string.Empty)
            return;
        _activeWidget = widget.Name;
    }

    public void RemoveActive(WidgetBase widget)
    {
        if (widget.Name != _activeWidget)
            return;
        _activeWidget = string.Empty;
    }

    public abstract void Draw();

    public virtual void Update(float timeDelta)
    {
        if (!Visible)
            return;
        if (Movable)
        {
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
        }

        foreach (var (_, widget) in Widgets)
            widget.Update(timeDelta);
    }
}