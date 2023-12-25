using Microsoft.Xna.Framework.Graphics;
using Color = SixLabors.ImageSharp.Color;

namespace MapleSyrup.ECS.Components.Map;

public class MapItem : Component
{
    public Texture2D Texture;
    public Color Color;
    public SpriteEffects Flipped;
}