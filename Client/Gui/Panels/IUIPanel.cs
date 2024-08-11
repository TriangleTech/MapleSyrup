using System.Numerics;
using Client.Gui.Components;
using Client.Gui.Enums;
using Raylib_CsLo;

namespace Client.Gui.Panels;

public interface IUIPanel
{
    public uint ID { get; init; }
    public string Name { get; set; }
    public bool Active { get; set; }
    public bool Visible { get; set; }
    public Vector2 Position { get; internal set; }
    public Vector2 ScreenOffset { get; internal set; }
    public Rectangle Bounds { get; internal set; }
    public Rectangle DstRectangle { get; internal set; }
    public GuiPriority Priority { get; }
    public List<IUIComponent> Nodes { get; }
    public string TexturePath { get; set; }
    public bool Add(IUIComponent node);
    public IUIComponent? GetNode(string node);
    public void Clear();
    public void Draw(float frameTime);
    public void Update(float frameTime);
}