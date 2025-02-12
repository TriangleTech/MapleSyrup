namespace Client.ECS.Components.Character;

public class PlayerInfo
{
    public required int PlayerId { get; init; }
    public required string PlayerName { get; set; }
    public required int Health { get; set; }
    public required int Mana { get; set; }
    public required int MaxHealth { get; set; }
    public required int MaxMana { get; set; }
    public required int MaxSpeed { get; set; }
}