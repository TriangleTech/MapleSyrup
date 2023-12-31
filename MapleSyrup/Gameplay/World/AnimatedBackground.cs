using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Gameplay.World;

public class AnimatedBackground : BackgroundItem
{
    public int FrameCount;
    public int CurrentFrame;
    public List<Texture2D> Frames;
}