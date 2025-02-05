using System.Numerics;

namespace Client.ECS.Components.Common;

public class Transform
    : IComponent
{
    public required int Owner { get; init; }
    public required Vector2 Position;
    public required Vector2 Origin;
    public float Scale = 1.0f;
    public float Rotation = 0.0f;
    public int Z = 0;
}