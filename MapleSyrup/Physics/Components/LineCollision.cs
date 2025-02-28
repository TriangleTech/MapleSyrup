using System.Numerics;
using MapleSyrup.ECS.Interfaces;
using ZeroElectric.Vinculum;

namespace MapleSyrup.Physics.Components;

public record LineCollision : IComponent
{
    public required int Owner { get; init; }
    public required int Layer { get; init; }
    public required Vector2 Start { get; init; }
    public required Vector2 End { get; init; }
    public required Rectangle Bounds { get; init; }
}