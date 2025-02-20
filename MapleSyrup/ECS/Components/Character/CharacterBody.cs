namespace MapleSyrup.ECS.Components.Character;

public class CharacterBody : IComponent
{
    public int Owner { get; init; }
    public required string SkinId { get; set; }
    public required string StateName { get; set; }
    public required string DefaultStateName { get; set; } = "stand1";
}