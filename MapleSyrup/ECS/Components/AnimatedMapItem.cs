using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace MapleSyrup.ECS.Components;

public class AnimatedMapItem : Component
{
    public List<Vector2> Positions;
    public List<Vector2> Origins;
    public int FrameCount;
    public int CurrentFrame;
    public Color Color;
    public List<Texture2D> Frames;
    public List<int> Delay;
    public List<int> Alpha0;
    public List<int> Alpha255;
    public int CurrentDelay;
}