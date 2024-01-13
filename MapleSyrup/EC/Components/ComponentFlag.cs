namespace MapleSyrup.EC.Components;

[Flags]
public enum ComponentFlag
{
    // Generic
    Transform = 1 << 0,
    Sprite = 1 << 1,
    Parallax = 1 << 2,
    Animation = 1 << 3,

    // Player
    Camera = 1 << 4,
    BodyPart = 1 << 5,
    Equipment = 1 << 6,

    // Physics
    Foothold = 1 << 7,
    Gravity = 1 << 8,
}