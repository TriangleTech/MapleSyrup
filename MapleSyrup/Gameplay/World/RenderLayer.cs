namespace MapleSyrup.Gameplay.World;

public enum RenderLayer
{
    Background = 1 << 0,
    TileObj0 = 1 << 1,
    TileObj1 = 1 << 2,
    TileObj2 = 1 << 3,
    TileObj3 = 1 << 4,
    TileObj4 = 1 << 5,
    TileObj5 = 1 << 6,
    TileObj6 = 1 << 7,
    TileObj7 = 1 << 8,
    Effects = 1 << 9,
    Foreground = 1 << 10,
    
    Mask = Background | TileObj0 | TileObj1 | TileObj2 | TileObj3 | TileObj4 | TileObj5 | TileObj6 | TileObj7 | Effects | Foreground
}