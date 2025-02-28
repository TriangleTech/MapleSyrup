using MapleSyrup.ECS.Interfaces;

namespace MapleSyrup.Character.Components;

public class CharacterController : IComponent
{
    public int Owner { get; init; }
    public bool MovingLeft { get; set; }
    public bool IsGrounded { get; set; }
}