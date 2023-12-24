using Microsoft.Xna.Framework;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MapleSyrup.ECS.Components;

public class Transform : Component
{
    public Vector2 Position = Vector2.Zero;
    public Vector2 Origin = Vector2.Zero;
    public Rectangle Source = Rectangle.Empty;
    public float Scale = 1f;
    public float Rotation = 0f;
}