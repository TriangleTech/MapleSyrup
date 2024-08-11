namespace Client.Avatar;

public class AvatarStats
{
    public int Id { get; init; } = 0;
    public string Name { get; set; } = string.Empty;
    public byte Gender { get; set; } = 0;
    public byte SkinId { get; set; } = 0;
    public int Face { get; set; } = 0;
    public int Hair { get; set; } = 30000;
    public List<int> PetId { get; set; } = new(3);
    public short Level { get; set; } = 1;
    public short JobId { get; set; } = 0;
    public short Strength { get; set; } = 4;
    public short Dexterity { get; set; } = 4;
    public short Intelligence { get; set; } = 4;
    public short Luck { get; set; } = 4;
    public short Hp { get; set; } = 100;
    public short MaxHp { get; set; } = 100;
    public short Mp { get; set; } = 50;
    public short MaxMp { get; set; } = 50;
    public short RemainingAp { get; set; } = 0;
    public int Exp { get; set; } = 0;
    public short Fame { get; set; } = 0;
    public int GachaExp { get; set; } = 0;
    public int MapId { get; set; } = 10000;
    public byte InitialSpawnPoint { get; set; } = 0;
}