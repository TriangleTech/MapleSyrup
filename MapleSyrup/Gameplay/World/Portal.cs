using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Gameplay.World;

public class Portal
{
    public List<Vector2> Origins = new();
    public List<Texture2D> Frames = new();
    public int CurrentFrame = 0;
    public int CurrentDelay = 100;
    public bool IsHidden = false;
}