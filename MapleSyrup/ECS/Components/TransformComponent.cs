using Microsoft.Xna.Framework;

namespace MapleSyrup.ECS.Components;

public class TransformComponent
{
    public Vector2 Position = Vector2.Zero;
    public Vector2 Origin = Vector2.Zero;
    public int Z = 0;
}