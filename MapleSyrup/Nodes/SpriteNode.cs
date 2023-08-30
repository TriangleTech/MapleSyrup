using MapleSyrup.Core;
using MapleSyrup.Graphics;
using MapleSyrup.Resources;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace MapleSyrup.Nodes;

public class SpriteNode : TextureNode
{
    private Vector2 position;
    private Vector2 origin;
    private Vector2 size;
    private float rotation;
    private float alpha;
    private Vector3 color;
    private float[] vertices;
    private Shader shader;
    private int vao;
    
    public SpriteNode(Image textureImage)
        : base(textureImage)
    {
        position = new Vector2(0f, 0f);
        origin = Vector2.Zero;
        alpha = 1.0f;
        size = new Vector2(TextureSize.X, TextureSize.Y);
        rotation = 0.0f;
        color = new Vector3(1.0f, 1.0f, 1.0f);
        shader = Engine.Instance.GetSubsystem<ResourceSystem>().GetShader("sprite");
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

    public override void Render()
    {
        Engine.GetSubsystem<ResourceSystem>().GetShader("sprite").SetInt("image", 0);
        Engine.GetSubsystem<ResourceSystem>().GetShader("sprite").Use();
        Transform = Matrix4.CreateTranslation(new Vector3(position.X, position.Y, 0.0f)) *
                Matrix4.CreateTranslation(0.5f * origin.X, 0.5f * origin.Y, 0.0f) *
                Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation)) *
                Matrix4.CreateTranslation(-0.5f * origin.X, -0.5f * origin.Y, 0.0f) *
                Matrix4.CreateScale(new Vector3(size));
        Engine.GetSubsystem<ResourceSystem>().GetShader("sprite").SetMatrix4("model", Transform);
        Engine.GetSubsystem<ResourceSystem>().GetShader("sprite").SetVector3("imageColor", color);
        Engine.GetSubsystem<ResourceSystem>().GetShader("sprite").SetFloat("alpha", alpha);
        Use();
        GL.BindVertexArray(vao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        GL.BindVertexArray(0);
        base.Render();
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}