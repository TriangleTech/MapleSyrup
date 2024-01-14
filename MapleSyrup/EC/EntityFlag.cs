namespace MapleSyrup.EC;

[Flags]
public enum EntityFlag
{
    Active = 1 << 0,
    Background = 1 << 1,
    MapTile = 1 << 2,
    MapObject = 1 << 3,
    AniObj = 1 << 4,
    Portal = 1 << 5,
}