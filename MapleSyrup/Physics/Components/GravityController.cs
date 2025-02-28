using MapleSyrup.ECS.Interfaces;

namespace MapleSyrup.Physics.Components;

public class GravityController : IComponent
{
    public int Owner { get; init; }
    public float Weight { get; init; }
    public bool IsGrounded { get; set; }
}