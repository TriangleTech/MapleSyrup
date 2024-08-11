namespace Client.Avatar;

public class AvatarLook
{
    public byte Gender { get; set; } = 0;
    public byte SkinId { get; set; } = 0;
    public int Face { get; set; } = 0;
    public int Hair { get; set; } = 30000;
    public Dictionary<PartType, int> Equipment { get; set; } = new();
}