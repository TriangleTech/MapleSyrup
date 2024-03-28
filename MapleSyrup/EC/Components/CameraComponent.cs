using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.EC.Components;

public class CameraComponent : IComponent
{
    public IEntity Parent { get; }
    public ComponentFlag Flag { get; }

    public Vector2 Position
    {
        get => _position;
        set => _position = value;
    }
    
    public Viewport Viewport { get; set; }

    private Matrix _matrix;
    private Vector2 _position;

    public CameraComponent(IEntity entity)
    {
        Parent = entity;
        Flag = ComponentFlag.Camera;
    }

    public Matrix GetMatrix() => _matrix;
    public void UpdateMatrix(IEntity target)
    {
        _position = Vector2.Lerp(_position, target.Transform.Position, 0.5f);
        _matrix = Matrix.CreateTranslation(new Vector3(-_position, 0f)) *
                  Matrix.CreateTranslation(new Vector3(Viewport.Width / 2f, Viewport.Height / 2f, 0f));
    }

    public void Clear()
    {
        
    }
}