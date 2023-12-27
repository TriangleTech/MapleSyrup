using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace MapleSyrup.ECS.Components.Map;

public class BlendAnimation : Component
{
    public List<Texture2D> Frames = new();
    public List<byte> Alpha = new();
    public List<int> Delay = new();
    public Color Color = Color.White;
    public int CurrentFrame = 0;
    public byte StartingAlpha = 0;
    public byte EndingAlpha = 0;
    public int CurrentDelay = 0;
}