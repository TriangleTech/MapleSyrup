using System.Collections;
using MapleSyrup.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Gameplay.Player;

public class AvatarLook
{
    public readonly VariantSet<Texture2D> Layers = new();
    public readonly VariantSet<Vector2> Position = new();
    public readonly VariantSet<Vector2> Origin = new();
    public readonly VariantSet<Vector2> Map = new();
}