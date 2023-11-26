using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Gameplay.Map;

public struct MapItem
{
    public Vector2 Position;
    public Vector2 Origin;
    public int Z;
    public Texture2D Texture;
}