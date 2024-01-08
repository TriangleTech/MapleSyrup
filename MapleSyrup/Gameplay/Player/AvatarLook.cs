using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Gameplay.Player;

public class AvatarLook
{
    public readonly Dictionary<string, Texture2D> Layers = new();
    public readonly Dictionary<string, Vector2> Position = new();
    public readonly Dictionary<string, Vector2> Origin = new();
    public readonly Dictionary<string, Vector2> Map = new();
}