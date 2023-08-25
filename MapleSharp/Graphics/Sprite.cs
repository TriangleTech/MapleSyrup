using MapleSharp.Events.Experimental;
using MapleSharp.Services;
using OpenTK.Mathematics;

namespace MapleSharp.Graphics;

public class Sprite : IDisposable
{
    private readonly IndexBuffer indexBuffer;
    private readonly Texture texture;
    
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public float Rotation { get; set; }
    public Color4 Color { get; set; } = Color4.White;

    public Sprite(Image image)
    {
        texture = new Texture(image);
        indexBuffer = new IndexBuffer();
        Position = new Vector2(0, 0);
        Size = new Vector2(image.Width, image.Height);
        Rotation = 0f;
        
    }
    
    public Sprite(Texture texture)
    {
        this.texture = texture;
        indexBuffer = new IndexBuffer();
    }

    public void Draw()
    {
        indexBuffer.Bind();
        texture.Use();
        indexBuffer.Draw();
    }
    
    public void Dispose()
    {
        texture.Dispose();
        indexBuffer.Dispose();
        GC.SuppressFinalize(this);
    }
}