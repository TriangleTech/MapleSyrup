using MapleSyrup.ECS.Interfaces;

namespace MapleSyrup.Character.Components;

public class CharacterInfo : IComponent
{
    public int Owner { get; init; }
    public required int PlayerId { get; init; }
    public required string PlayerName { get; set; }
    public required int Health { get; set; }
    public required int Mana { get; set; }
    public required int MaxHealth { get; set; }
    public required int MaxMana { get; set; }
    public required int MaxSpeed { get; set; }
}