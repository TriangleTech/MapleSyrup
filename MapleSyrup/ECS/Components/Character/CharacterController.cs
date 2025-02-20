namespace MapleSyrup.ECS.Components.Character;

public class CharacterController : IComponent
{
    public int Owner { get; init; }
    public bool MovingLeft { get; set; }
    public bool IsGrounded { get; set; }
}