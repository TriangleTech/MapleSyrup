using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Graphics;

public class Animation
{
    public Dictionary<int, Texture2D> Frames = new();
    public Dictionary<int, int> Delay = new();
    public Dictionary<int, Vector2> Position = new();
    public Dictionary<int, Vector2> Origin = new();
    public int NextFrame = 0;
    public int NextDelay = 0;
}