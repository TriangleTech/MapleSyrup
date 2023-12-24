using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace MapleSyrup.ECS.Components;

public class AnimatedMapItem : Component
{
    public List<Vector2> Positions = new();
    public List<Vector2> Origins = new();
    public int CurrentFrame = 0;
    public Color Color = Color.White;
    public List<Texture2D> Frames = new();
    public List<int> Delay = new();
    public List<int> StartAlpha = new();
    public List<int> EndAlpha = new();
    public int CurrentDelay = 0;
}