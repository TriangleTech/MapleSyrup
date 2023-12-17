using MapleSyrup.Gameplay.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace MapleSyrup.ECS.Components;

public class BackgroundItem : Component
{
    public Texture2D Texture;
    public Vector2 Position;
    public Vector2 Origin;
    public float Scale;
    public float Rotation;
    public Color Color;
    public SpriteEffects Flipped;
    public BackgroundType Type;
    public Vector2 Shift;
    public Vector2 Speed;
    public int Cx;
    public int Cy;
    public int Rx;
    public int Ry;
    public int Alpha;
}