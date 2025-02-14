using System.Numerics;

namespace MapleSyrup.ECS.Components.Common;

public class LineCollision : IComponent
{
    public required int Owner { get; init; }
    public Vector2 Start;
    public Vector2 End;
}