using MapleSyrup.Gameplay.World;
using Microsoft.Xna.Framework;

namespace MapleSyrup.ECS.Components;

public class BackgroundItem : Sprite
{
    public BackgroundType Type;
    public Vector2 Shift;
    public Vector2 Speed;
}