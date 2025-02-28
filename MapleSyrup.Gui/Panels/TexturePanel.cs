using ZeroElectric.Vinculum;

namespace MapleSyrup.Gui.Panels;

public class TexturePanel : PanelBase
{
    public required string ResourcePath { get; set; }
    public Texture Texture { get; set; }
    
    public TexturePanel(string name) 
        : base(name)
    {
      
    }

    public override void Draw()
    {
        if (!Visible)
            return;
        Raylib.DrawTexture(Texture, (int)Bounds.X, (int)Bounds.Y, Raylib.WHITE);
    }

    public override void Update(float timeDelta)
    {
        base.Update(timeDelta);
        
        Bounds = new Rectangle(Position.X, Position.Y, Texture.width, Texture.height);
    }
}