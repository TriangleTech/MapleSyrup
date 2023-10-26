using MapleSyrup.Core;
using MapleSyrup.Graphics;
using MapleSyrup.Resources;
using MapleSyrup.Scene;
using MapleSyrup.Window;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace MapleSyrup.Nodes;

public class SpriteNode : TextureNode
{
    private Matrix4 transform = Matrix4.Identity;
    private Vector2 origin;
    private Vector3 color;
    private float alpha;
    private float rotation;
    private float[] vertices;
    private Shader shader;
    private int vao;
    private IWindow window;
    
    public Matrix4 Transform
    {
        get => transform;
        set => transform = value;
    }
    
    public Vector2 Origin
    {
        get => origin;
        set => origin = value;
    }
    
    public Vector3 Color
    {
        get => color;
        set => color = value;
    }
    
    public float Alpha
    {
        get => alpha;
        set => alpha = value;
    }
    
    public float Rotation
    {
        get => rotation;
        set => rotation = value;
    }
    
    public SpriteNode(Image textureImage)
        : base(textureImage)
    {
        Position = new Vector2(0f, 0f);
        Origin = Vector2.Zero;
        Alpha = 1.0f;
        Rotation = 0.0f;
        Color = new Vector3(1.0f, 1.0f, 1.0f);
        shader = Engine.Instance.GetSubsystem<ResourceSystem>().GetShader("sprite");
        Transform = Matrix4.Identity;
        window = Engine.Instance.Window;
        Init();
    }

    private void Init()
    {
        vertices = new[]
        {
            // Position     // Texture
            0.0f, 1.0f,     0.0f, 1.0f,
            1.0f, 0.0f,     1.0f, 0.0f,
            0.0f, 0.0f,     0.0f, 0.0f,
            
            0.0f, 1.0f,     0.0f, 1.0f,
            1.0f, 1.0f,     1.0f, 1.0f,
            1.0f, 0.0f,     1.0f, 0.0f
        };
        
        GL.GenVertexArrays(1, out vao);
        GL.GenBuffers(1, out int vbo);
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
        
        GL.BindVertexArray(vao);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }

    public override void Render()
    {
        Engine.GetSubsystem<ResourceSystem>().GetShader("sprite").SetInt("image", 0);
        Engine.GetSubsystem<ResourceSystem>().GetShader("sprite").Use();
        var posx =  1.0f - (Position.X + (Origin.X / Size.X)) / (window.Width / 2.0f);
        var posy = (Position.Y + (Origin.Y / Size.Y)) / (window.Height / 2.0f) - 1.0f;
        Transform = Matrix4.CreateTranslation(0, 0, 0.0f) *
                    Matrix4.CreateRotationZ(Rotation) *
                    Matrix4.CreateScale(Size.X, Size.Y, 1.0f);
        Engine.GetSubsystem<ResourceSystem>().GetShader("sprite").SetMatrix4("model", Transform);
        Engine.GetSubsystem<ResourceSystem>().GetShader("sprite").SetVector3("imageColor", Color);
        Engine.GetSubsystem<ResourceSystem>().GetShader("sprite").SetFloat("alpha", Alpha);
        Use();
        GL.BindVertexArray(vao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        GL.BindVertexArray(0);
        base.Render();
    }

    private Vector3 PositionToCoord(Vector2 vec)
    {
        //var camera = Root.FindNode<CameraNode>("Camera");
        //var mv = (camera.Projection * camera.View).Inverted();
        var x = ((vec.X) / (window.Width)) - 1.0f;
        var y = 1.0f - ((vec.Y) / (window.Height));
        var z = Z;
        
        return new Vector3(x, y, z);
    }
    
    private Vector3 OriginToCoord(Vector2 vec)
    {
        var x = 2.0f * ((vec.X + 0.5f) / (window.Width / 2f)) - 1.0f;
        var y = 1.0f - ((vec.Y + 0.5f) / (window.Height / 2f)) * 2.0f;
        var z = 0; 
        
        return new Vector3(x, y, z);
    }

    private Vector4 ScreenToCoord(Vector3 pos)
    {
        return new Vector4();
    }
}