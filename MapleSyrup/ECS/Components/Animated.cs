using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.ECS.Components;

public class Animated : Sprite
{
    public int FrameCount;
    public int CurrentFrame;
    public List<Texture2D> Frames;
    public float Delay;
    public int LoopCount;
    public bool Loop;
    public bool Paused;
    public bool Finished;
}