using System.Numerics;
using Client.Gui.Panels;
using Client.NX;
using Raylib_CsLo;

namespace Client.Gui.Components;

public class Decal : IUIComponent
{
    public IUIPanel Parent { get; init; }
    public int ID { get; init; }
    public string Name { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 ScreenOffset { get; set; }
    public Rectangle Bounds { get; set; }
    public Rectangle DstRectangle { get; set; }
    public NxNode Node { get; init; }
    public bool Active { get; set; }
    public Texture Texture { get; init; }
    public string TexturePath { get; set; }
    
    public void Clear()
    {
        Raylib.UnloadTexture(Texture);
    }

    public void Draw(Vector2 parentPos, float frameTime)
    {
        Bounds = new Rectangle(0, 0, Texture.width, Texture.height);
        ScreenOffset = new Vector2(parentPos.X + Position.X,
            parentPos.Y + Position.Y);
        DstRectangle = new Rectangle(
            ScreenOffset.X, 
            ScreenOffset.Y, 
            Texture.width * AppConfig.ScaleFactor, 
            Texture.height * AppConfig.ScaleFactor); // TODO: create event system to do this there instead
        Raylib.DrawTexturePro(Texture, Bounds, DstRectangle, Vector2.Zero, 0f, Raylib.WHITE);
    }

    public void Update(Vector2 parentPos, float frameTime)
    {
    }
}