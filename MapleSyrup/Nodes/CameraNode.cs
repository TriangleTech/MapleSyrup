using MapleSyrup.Graphics;
using MapleSyrup.Resources;
using OpenTK.Mathematics;
using SDL2;

namespace MapleSyrup.Nodes;

public class CameraNode : Node
{
    private Shader shader;
    private Vector2 viewportSize;
    
    public Vector2 ViewportSize
    {
        get => viewportSize;
        set => viewportSize = value;
    }
    
    public CameraNode()
    {
        Name = "Camera";
        shader = Engine.GetSubsystem<ResourceSystem>().GetShader("sprite");
        Position = new Vector2(0.0f, 0.0f);
        viewportSize = new Vector2(Engine.Window.Width, Engine.Window.Height);
    }
    
    public override void Update(float timeDelta)
    {
        var view = Matrix4.LookAt(new Vector3(Position.X, Position.Y, 1.0f), new Vector3(Position.X, Position.Y, -1.0f), Vector3.UnitY);
        var projection = Matrix4.CreateOrthographicOffCenter(0.0f, viewportSize.X, viewportSize.Y, 0.0f, -1.0f, 1.0f);
        shader.Use();
        shader.SetMatrix4("projection", projection);
        shader.SetMatrix4("view", view);
        base.Update(timeDelta);
    }
}