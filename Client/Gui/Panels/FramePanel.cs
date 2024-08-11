using System.Numerics;
using Client.Gui.Components;
using Client.Gui.Components.Buttons;
using Client.Gui.Enums;
using Client.Managers;
using Raylib_CsLo;

namespace Client.Gui.Panels;

public class FramePanel : IUIPanel
{
    public uint ID { get; init; }
    public string Name { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 ScreenOffset { get; set; }
    public Rectangle Bounds { get; set; }
    public Rectangle DstRectangle { get; set; }
    public GuiPriority Priority { get; init; } = GuiPriority.Above;
    public List<IUIComponent> Nodes { get; }
    public bool Active { get; set; }
    public bool Visible { get; set; }
    public Texture Texture { get; init; }
    public string TexturePath { get; set; }
    private object _threadLock;

    public FramePanel(string texturePath)
    {
        TexturePath = texturePath;
        _threadLock = new();
        Nodes = new();
    }

    public bool Add(IUIComponent node)
    {
        lock (_threadLock)
        {
            Nodes.Add(node);
            return true;
        }
    }

    public IUIComponent? GetNode(string name)
    {
        return Nodes.Find(x => x.Name == name);
    }

    public void Draw(float frameTime)
    {
        Bounds = new Rectangle(0, 0, Texture.width, Texture.height);
        ScreenOffset = new Vector2(Position.X * AppConfig.ScaleFactor + AppConfig.ScreenOffsetX,
            Position.Y * AppConfig.ScaleFactor + AppConfig.ScreenOffsetY);
        DstRectangle = new Rectangle(
            ScreenOffset.X, 
            ScreenOffset.Y, 
            Texture.width * AppConfig.ScaleFactor, 
            Texture.height * AppConfig.ScaleFactor); // TODO: create event system to do this there instead
        Raylib.DrawTexturePro(Texture, Bounds, DstRectangle, Vector2.Zero, 0f, Raylib.WHITE);
        foreach (var  node in Nodes)
            node.Draw(ScreenOffset, frameTime);
    }

    public void Update(float frameTime)
    {
        var input = ServiceLocator.Get<InputManager>();
        var ui = ServiceLocator.Get<UIManager>();
        
        ui.CheckPanelFocus(input, this, DstRectangle);
        foreach (var node in Nodes)
        {
            if (Active || node is TextureButton)
                ui.CheckNodeFocus(input, node, node.Bounds);
            
            if (node.Active || node is TextureButton)
                node.Update(ScreenOffset, frameTime);
        }
    }

    public void Clear()
    {
        foreach (var node in Nodes)
            node.Clear();
        Raylib.UnloadTexture(Texture);
    }
}