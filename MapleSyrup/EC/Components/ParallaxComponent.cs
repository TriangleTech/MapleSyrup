using MapleSyrup.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.EC.Components;

public class ParallaxComponent : IComponent
{
    public IEntity Parent { get; }
    public ComponentFlag Flag { get; }
    
    public Vector2 Position;
    public Vector2 Origin;
    public int Rx, Ry;
    public BackgroundType Type;
    public Matrix Parallax;
    public Texture2D? Texture;

    public ParallaxComponent(IEntity parent)
    {
        Parent = parent;
    }
    
    public Matrix GetMatrix()
    {
        return Parallax;
    }

    public void UpdateMatrix()
    {
        Parallax = Matrix.Identity;
    }
}