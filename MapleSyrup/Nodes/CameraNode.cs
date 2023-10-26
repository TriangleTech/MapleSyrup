using MapleSyrup.Core;
using MapleSyrup.Graphics;
using MapleSyrup.Resources;
using OpenTK.Mathematics;
using SDL2;

namespace MapleSyrup.Nodes;

public class CameraNode : Node
{
    private Shader shader;
    private Vector2 viewportSize;
    private Matrix4 projection;
    private Matrix4 view;
    private Vector3 cameraPosition;
    private Vector3 worldUp;
    private Vector3 cameraFront;
    private float zoom;
    
    public Matrix4 Projection
    {
        get => projection;
        set => projection = value;
    }
    
    public Matrix4 View
    {
        get => view;
        set => view = value;
    }
    
    public Vector3 CameraPosition
    {
        get => cameraPosition;
        set => cameraPosition = value;
    }
    
    public Vector3 WorldUp
    {
        get => worldUp;
        set => worldUp = value;
    }
    
    public Vector3 CameraFront
    {
        get => cameraFront;
        set => cameraFront = value;
    }
    
    public Vector2 ViewportSize
    {
        get => viewportSize;
        set => viewportSize = value;
    }
    
    public float Zoom
    {
        get => zoom;
        set => zoom = value;
    }
    
    public CameraNode()
    {
        Name = "Camera";
        shader = Engine.GetSubsystem<ResourceSystem>().GetShader("sprite");
        CameraPosition = new Vector3(0.0f, 0.0f, -3.0f);
        WorldUp = Vector3.UnitY;
        CameraFront = new Vector3(0.0f, 0.0f, -1.0f);
        ViewportSize = new Vector2(Engine.Window.Width, Engine.Window.Height);
        Zoom = 1.0f;
        Engine.GetSubsystem<EventSystem>().ListenForEvent("OnWindowResized", OnWindowResized);
        Engine.GetSubsystem<EventSystem>().ListenForEvent("OnKeyDown", OnKeyDown);
    }
    
    public override void Update(float timeDelta)
    {
        View = Matrix4.LookAt(CameraPosition, CameraPosition + CameraFront, WorldUp);
        Projection = Matrix4.CreateOrthographicOffCenter(0.0f, viewportSize.X, viewportSize.Y, 0.0f, -10.0f, 10.0f);
        shader.Use();
        shader.SetMatrix4("projection", Projection);
        shader.SetMatrix4("view", View);
        base.Update(timeDelta);
    }

    private void OnWindowResized(object data)
    {
        var size = (Vector2)data;
        viewportSize = size;
    }

    private void OnKeyDown(object key)
    {
        var keycode = (SDL.SDL_Keycode)key;
        var cameraSpeed = 15f;
        if (keycode == SDL.SDL_Keycode.SDLK_LEFT)
            cameraPosition.X -= cameraSpeed;
        if (keycode == SDL.SDL_Keycode.SDLK_RIGHT)
            cameraPosition.X += cameraSpeed;
        if (keycode == SDL.SDL_Keycode.SDLK_UP)
            cameraPosition.Y -= cameraSpeed;
        if (keycode == SDL.SDL_Keycode.SDLK_DOWN)
            cameraPosition.Y += cameraSpeed;
        
        Console.WriteLine($"Normal Position: { CameraPosition } Fixed to Coord: {PositionToCoord(CameraPosition)}");
    }
    
    private Vector3 PositionToCoord(Vector3 vec)
    {
        var x = (((vec.X) / (ViewportSize.X )));
        var y = ( ((vec.Y) / (ViewportSize.Y)));
        var z = 2.0f * (vec.Z - 0) / (1.0f - 0.0f) - 1.0f;
        
        return new Vector3(x, y, z);
    }
    
    public Matrix4 InverseProjection()
    {
        return Matrix4.Invert(Projection);
    }
    
    public Matrix4 InverseView()
    {
        return Matrix4.Invert(View);
    }
    
    public Vector4 ScreenSpaceToWorldSpace(Vector4 screenSpace)
    {
        var inverseProjection = InverseProjection();
        var inverseView = InverseView();
        var screenToWorld = inverseProjection * inverseView;
        return screenToWorld * screenSpace;
    }
}