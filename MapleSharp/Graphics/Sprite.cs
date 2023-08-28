

using OpenTK.Mathematics;

namespace MapleSharp.Graphics;

public class Sprite : IDisposable
{
    private readonly IndexBuffer indexBuffer;
    private readonly Texture texture;
    private Matrix4 transform;
    private Vector2 position, origin, size;
    private float rotation, zIndex;

    public Vector2 Position
    {
        get => position;
        set
        {
            position = value;
            UpdateMatrix();
        }
    }
    
    public Vector2 Origin
    {
        get => origin;
        set
        {
            origin = value;
            UpdateMatrix();
        }
    }
    
    public Vector2 Size
    {
        get => size;
        set
        {
            size = value;
            UpdateMatrix();
        }
    }
    
    public float Rotation
    {
        get => rotation;
        set
        {
            rotation = value;
            UpdateMatrix();
        }
    }
    
    public float ZIndex
    {
        get => zIndex;
        set
        {
            zIndex = value;
            UpdateMatrix();
        }
    }
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
        Position = new Vector2(0, 0);
        Rotation = 0f;
    }
    
    private void UpdateMatrix()
    {
        transform = Matrix4.CreateTranslation(Position.X, Position.Y, ZIndex) *
                    Matrix4.CreateRotationZ(Rotation) *
                    Matrix4.CreateTranslation(-Origin.X, -Origin.Y, 0f) *
                    Matrix4.CreateScale(Size.X, Size.Y, 1f);
    }

    public void Draw()
    {
        indexBuffer.Bind();
        texture.Use();
        //texture.GetShader().SetMatrix4("view", Matrix4.CreateTranslation(0f, 0f, -3.0f));
        //texture.GetShader().SetMatrix4("model", Matrix4.Zero);
        //var projection = Matrix4.CreateOrthographic(1280f, 768f, 0.1f, 100.0f);
        //texture.GetShader().SetMatrix4("projection", projection);
        texture.GetShader().Use();
        indexBuffer.Draw();
    }
    
    public void Dispose()
    {
        texture.Dispose();
        indexBuffer.Dispose();
        GC.SuppressFinalize(this);
    }
}