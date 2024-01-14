using MapleSyrup.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.EC.Components;

public class ParallaxComponent : IComponent
{
    private Matrix _parallax;
    
    public IEntity Parent { get; }
    public ComponentFlag Flag { get; }
    public int Rx, Ry;
    public BackgroundType Type;

    public ParallaxComponent(IEntity parent)
    {
        Flag = ComponentFlag.Parallax;
        Parent = parent;
        _parallax = Matrix.Identity;
    }
    
    public Matrix GetMatrix()
    {
        return _parallax;
    }

    public void UpdateMatrix()
    {
        _parallax = Matrix.Identity;
    }
}