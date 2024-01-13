namespace MapleSyrup.EC;

[Flags]
public enum EntityFlag
{
    Active = 1 << 0,
    Background = 1 << 1,
    MapTile = 1 << 2,
    MapObject = 1 << 3,
    AniObj = 1 << 4,
    Effect = 1 << 5,
    Foreground = 1 << 6,
    Player = 1 << 7,
    PlayerControlled = 1 << 8,
}