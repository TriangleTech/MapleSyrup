using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = SixLabors.ImageSharp.Color;

namespace MapleSyrup.ECS.Components;

public class MapItem : Component
{
    public Texture2D Texture;
    public Vector2 Position;
    public Vector2 Origin;
    public float Scale;
    public float Rotation;
    public Color Color;
    public SpriteEffects Flipped;
}