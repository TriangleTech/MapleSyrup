using MapleSyrup.Core;
using MapleSyrup.Nodes;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Gameplay.Map;

public class MapItem : Node2D
{
    public bool IsAnimated = false;
    public Texture2D Texture;
    public List<Texture2D> Frames;
    
    public MapItem(GameContext context)
        : base(context)
    {
        Frames = new();
    }
}