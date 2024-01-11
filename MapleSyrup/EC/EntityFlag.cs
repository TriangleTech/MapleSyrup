namespace MapleSyrup.EC;

[Flags]
public enum EntityFlag
{
    Active = 1 << 0,
    Background = 1 << 1,
    MapObject = 1 << 2,
    Effect = 1 << 3,
    Foreground = 1 << 4,
    
    Player = 1 << 5,
    PlayerControlled = 1 << 6,
}