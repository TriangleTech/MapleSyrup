using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.ECS.Components.Map;

public class AnimatedBackground : BackgroundItem
{
    public int FrameCount;
    public int CurrentFrame;
    public List<Texture2D> Frames;
}