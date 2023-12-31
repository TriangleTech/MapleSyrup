using MapleSyrup.ECS.Components.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MapleSyrup.ECS.Components;

public class Camera
{
    public Vector2 Position = Vector2.Zero;
    public Vector2 Origin = Vector2.One;
    public Matrix Transform = Matrix.Identity;
    public float Rotation = 0f;
    public float Zoom = 1f;
    public float Speed = 5f;
    public Viewport Viewport = new Viewport();
    public bool EnabledCulling = true;
    
    public Camera(Viewport viewport)
    {
        Viewport = viewport;
        Origin = new Vector2(Viewport.Width / 2f, Viewport.Height / 2f);
    }
    
    public Matrix GetViewMatrix()
    {
        return Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
               Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
               Matrix.CreateRotationZ(Rotation) *
               Matrix.CreateScale(Zoom, Zoom, 1) *
               Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
    }
}