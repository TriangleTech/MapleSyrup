using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace MapleSyrup.ECS.Components;

public class AnimatedMapItem : Component
{
    public List<Vector2> Positions = new List<Vector2>();
    public List<Vector2> Origins = new List<Vector2>();
    public int FrameCount = 0;
    public int CurrentFrame = 0;
    public Color Color = Color.White;
    public List<Texture2D> Frames = new List<Texture2D>();
    public List<int> Delay = new List<int>();
    public List<int> Alpha0 = new List<int>();
    public List<int> Alpha255 = new List<int>();
    public int CurrentDelay = 0;
    public bool AwaitingNextFrame = false;
}