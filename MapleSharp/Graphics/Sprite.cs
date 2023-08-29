

using MapleSharp.Core;
using MapleSharp.Resources;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace MapleSharp.Graphics;

public class Sprite : EngineObject, IDisposable
{
    private Texture texture;
    private Vector2 position;
    private Vector2 origin;
    private Vector2 size;
    private float rotation;
    private float alpha;
    private Vector3 color;
    private float[] vertices;
    private Shader shader;
    private int vao;
    private Matrix4 model = Matrix4.Identity;

    public Sprite(Texture textureImage)
        : base(Engine.Instance)
    {
        texture = textureImage;
        position = new Vector2(0f, 0f);
        origin = Vector2.Zero;
        alpha = 1.0f;
        size = new Vector2(texture.TextureSize.X, texture.TextureSize.Y);
        rotation = 0.0f;
        color = new Vector3(1.0f, 1.0f, 1.0f);
        shader = GetSubsystem<ResourceSystem>().GetShader("sprite");
        Init();
    }

    private void Init()
    {
        vertices = new[]
        {
            0.0f, 1.0f, 0.0f, 1.0f,
            1.0f, 0.0f, 1.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 0.0f,

            0.0f, 1.0f, 0.0f, 1.0f,
            1.0f, 1.0f, 1.0f, 1.0f,
            1.0f, 0.0f, 1.0f, 0.0f
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

    public void Draw()
    {
        GetSubsystem<ResourceSystem>().GetShader("sprite").Use();
        model = Matrix4.CreateTranslation(new Vector3(position.X, position.Y, 0.0f)) *
                Matrix4.CreateTranslation(0.5f * origin.X, 0.5f * origin.Y, 0.0f) *
                Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation)) *
                Matrix4.CreateTranslation(-0.5f * origin.X, -0.5f * origin.Y, 0.0f) *
                Matrix4.CreateScale(new Vector3(size));
        GetSubsystem<ResourceSystem>().GetShader("sprite").SetMatrix4("model", model);
        GetSubsystem<ResourceSystem>().GetShader("sprite").SetVector3("imageColor", color);
        GetSubsystem<ResourceSystem>().GetShader("sprite").SetFloat("alpha", alpha);
        texture.Use();
        GL.BindVertexArray(vao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        GL.BindVertexArray(0);
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}