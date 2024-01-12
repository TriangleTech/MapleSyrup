namespace MapleSyrup.EC.Components;

[Flags]
public enum ComponentFlag
{
    // Generic
    Transform = 1 << 0,
    Sprite = 1 << 1,
    Parallax = 1 << 2,
    
    // Player
    Camera = 1 << 3,
    BodyPart = 1 << 4,
    Equipment = 1 << 5,
    
    // Physics
    Foothold = 1 << 6,
    Gravity = 1 << 7,
}