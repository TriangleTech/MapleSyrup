using System.Numerics;
using Client.Gui.Components;
using Client.Gui.Enums;
using Raylib_CsLo;

namespace Client.Gui.Panels;

public class StackPanel : IUIPanel
{
    public uint ID { get; init; }
    public string Name { get; set; } = string.Empty;
    public bool Active { get; set; }
    public bool Visible { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 ScreenOffset { get; set; }
    public Rectangle Bounds { get; set; }
    public Rectangle DstRectangle { get; set; }
    public GuiPriority Priority { get; init; }
    public List<IUIComponent> Nodes { get; } = new();
    public GridLayout Layout { get; init; } = GridLayout.VerticalDown;
    public Texture Texture { get; init; } = new();
    public string TexturePath { get; set; } = string.Empty;
    public float OffsetX { get; init; } = 0;
    public float OffsetY { get; init; } = 0;
    
    /// <summary>
    /// Number of Rows
    /// </summary>
    public int RowCount { get; init; } = 1;
    
    // Number of columns
    public int ColumnCount { get; init; } = 1;
    
    public bool Add(IUIComponent node)
    {
            Nodes.Add(node);
            return true;
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
    
    public void Clear()
    {
        foreach (var node in Nodes)
            node.Clear();
        Raylib.UnloadTexture(Texture);
    }
}