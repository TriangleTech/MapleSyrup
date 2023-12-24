using MapleSyrup.Gameplay.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MapleSyrup.ECS.Components;

public class BackgroundItem : Component
{
    public Texture2D Texture;
    public Color Color = Color.White;
    public SpriteEffects Flipped = SpriteEffects.None;
    public BackgroundType Type;
    public Vector2 Shift = Vector2.Zero;
    public Vector2 Speed = Vector2.Zero;
    public int Cx = 0;
    public int Cy = 0;
    public int Rx = 0;
    public int Ry = 0;
    public int Alpha = 255;
}