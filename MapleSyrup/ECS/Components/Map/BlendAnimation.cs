using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace MapleSyrup.ECS.Components.Map;

public class BlendAnimation : Component
{
    public List<Texture2D> Frames = new();
    public List<int> StartAlpha = new();
    public List<int> EndAlpha = new();
    public List<int> Delay = new();
    public Color Color = Color.White;
    public int CurrentFrame = 0;
    public int StartingAlpha = 0;
    public int EndingAlpha = 0;
    public int CurrentDelay = 0;
}