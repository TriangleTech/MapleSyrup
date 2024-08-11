using System.Numerics;
using Client.Gui.Components;
using Client.Gui.Components.Buttons;
using Client.Gui.Enums;
using Raylib_CsLo;

namespace Client.Gui.Panels;

public class ButtonPanel : IUIPanel
{
    public uint ID { get; init; }
    public string Name { get; set; } = string.Empty;
    public bool Active { get; set; } = false;
    public bool Visible { get; set; } = false;
    public Vector2 Position { get; set; } = Vector2.Zero;
    public Vector2 ScreenOffset { get; set; }
    public Rectangle Bounds { get; set; } = new();
    public Rectangle DstRectangle { get; set; }
    public GuiPriority Priority { get; init; } = GuiPriority.Normal;
    public List<IUIComponent> Nodes { get; } = new();
    public GridLayout Layout { get; init; } = GridLayout.HorizontalRight;
    public Texture Texture { get; init; } = new();
    public int OffsetX { get; init; } = 0;
    public int OffsetY { get; init; } = 0;
    public string TexturePath { get; set; }
    
    /// <summary>
    /// Number of Rows
    /// </summary>
    public int RowCount { get; init; } = 1;
    
    // Number of columns
    public int ColumnCount { get; init; } = 1;

    public ButtonPanel(string texturePath)
    {
        TexturePath = texturePath;
    }
    
    public bool Add(IUIComponent node)
    {
        if (node is not TextureButton) return false;
        //if ((int)node.Bounds.width != (int)Nodes[0].Bounds.width
        //    || (int)node.Bounds.height != (int)Nodes[0].Bounds.height) 
        //    return false; // For buttons, they all must be the same width and height
        Nodes.Add(node);
        return true;
    }

    public IUIComponent? GetNode(string node)
    {
        return Nodes.Find(x => x.Name == node);
    }

    public void Clear()
    {
        foreach (var node in Nodes)
            node.Clear();
        Raylib.UnloadTexture(Texture);
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
        switch (Layout)
        {
            case GridLayout.HorizontalLeft:
                for (var i = 0; i < Nodes.Count; i++)
                {
                    Nodes[i].Draw(ScreenOffset - new Vector2((Nodes[i].Bounds.width + OffsetX) * i, 0), frameTime);
                }
                break;
            case GridLayout.HorizontalRight:
                for (var i = 0; i < Nodes.Count; i++)
                {
                    Nodes[i].Draw(ScreenOffset + new Vector2((Nodes[i].Bounds.width + OffsetX) * i, 0), frameTime);
                }
                break;
            case GridLayout.VerticalUp:
                for (var i = 0; i < Nodes.Count; i++)
                {
                    Nodes[i].Draw(ScreenOffset - new Vector2(0, (Nodes[i].Bounds.height + OffsetY) * i), frameTime);
                }
                break;
            case GridLayout.VerticalDown:
                for (var i = 0; i < Nodes.Count; i++)
                {
                    Nodes[i].Draw(ScreenOffset + new Vector2(0, (Nodes[i].Bounds.height + OffsetY) * i), frameTime);
                }
                break;
            case GridLayout.LeftStack:
            {
                var row = 0;
                var column = 0;
                foreach (var button in Nodes)
                {
                    if (column >= ColumnCount)
                    {
                        column = 0;
                        row++;
                    }

                    button.Draw(ScreenOffset - new Vector2(
                            (button.Bounds.width + OffsetX) * column,
                            (button.Bounds.height + OffsetY) * row),
                        frameTime);
                    column++;
                }
            }
                break;
            case GridLayout.RightStack:
            {
                var row = 0;
                var column = 0;
                foreach (var button in Nodes)
                {
                    if (column >= ColumnCount)
                    {
                        column = 0;
                        row++;
                    }

                    button.Draw(ScreenOffset + new Vector2(
                            (button.Bounds.width + OffsetX) * column,
                            (button.Bounds.height + OffsetY) * row),
                        frameTime);
                    column++;
                }
            }
                break;
        }
    }

    public void Update(float frameTime)
    {
        switch (Layout)
        {
            case GridLayout.HorizontalLeft:
                for (var i = 0; i < Nodes.Count; i++)
                {
                    Nodes[i].Update(ScreenOffset - new Vector2((Nodes[i].Bounds.width + OffsetX) * i, 0), frameTime);
                }
                break;
            case GridLayout.HorizontalRight:
                for (var i = 0; i < Nodes.Count; i++)
                {
                    Nodes[i].Update(ScreenOffset + new Vector2((Nodes[i].Bounds.width + OffsetX) * i, 0), frameTime);
                }
                break;
            case GridLayout.VerticalUp:
                for (var i = 0; i < Nodes.Count; i++)
                {
                    Nodes[i].Update(ScreenOffset - new Vector2(0, (Nodes[i].Bounds.height + OffsetY) * i), frameTime);
                }
                break;
            case GridLayout.VerticalDown:
                for (var i = 0; i < Nodes.Count; i++)
                {
                    Nodes[i].Update(ScreenOffset + new Vector2(0, (Nodes[i].Bounds.height + OffsetY) * i), frameTime);
                }
                break;
            case GridLayout.LeftStack:
            {
                var row = 0;
                var column = 0;
                foreach (var button in Nodes)
                {
                    if (column >= ColumnCount)
                    {
                        column = 0;
                        row++;
                    }

                    button.Update(ScreenOffset - new Vector2(
                            (button.Bounds.width + OffsetX) * column,
                            (button.Bounds.height + OffsetY) * row),
                        frameTime);
                    column++;
                }
            }
                break;
            case GridLayout.RightStack:
            {
                var row = 0;
                var column = 0;
                foreach (var button in Nodes)
                {
                    if (column >= ColumnCount)
                    {
                        column = 0;
                        row++;
                    }

                    button.Update(ScreenOffset + new Vector2(
                            (button.Bounds.width + OffsetX) * column,
                            (button.Bounds.height + OffsetY) * row),
                        frameTime);
                    column++;
                }
            }
                break;
        }
    }
}