using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace MapleSyrup.ECS.Components.Map;

public class AnimatedMapItem : Component
{
    public List<Vector2> Positions = new();
    public List<Vector2> Origins = new();
    public List<Texture2D> Frames = new();
    public List<int> Delay = new();
    public int CurrentDelay = 0;
    public int CurrentFrame = 0;
}