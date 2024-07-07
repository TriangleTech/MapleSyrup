using MapleSyrup.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.EC.Components;

public class ParallaxComponent : IComponent
{
    private Matrix _parallax;
    
    public IEntity Parent { get; }
    public ComponentFlag Flag { get; }

    public Matrix Matrix
    {
        get => _parallax;
        set => _parallax = value;
    }
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

    public void UpdateMatrix(CameraComponent camera)
    {
        _parallax = Matrix.CreateTranslation(new Vector3(
            (camera.Position.X * (Rx * 0.008f) + camera.Viewport.Width / 2f),
            camera.Position.Y * (Ry * 0.008f) + (camera.Viewport.Height / 2f), 0));
    }

    public void Clear()
    {
        
    }
}