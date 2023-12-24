using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MapleSyrup.ECS.Components;

public class Camera : Component
{
    public Vector2 Position = Vector2.Zero;
    public Vector2 Origin = Vector2.Zero;
    public Matrix Transform = Matrix.Identity;
    public float Rotation = 0f;
    public float Zoom = 1f;
    public float Speed = 5f;
    public Rectangle Bounds = Rectangle.Empty;
    public Viewport Viewport = new Viewport();
    public bool EnabledCulling = true;
}