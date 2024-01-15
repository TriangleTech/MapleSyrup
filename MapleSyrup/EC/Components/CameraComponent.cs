using Microsoft.Xna.Framework;

namespace MapleSyrup.EC.Components;

public class CameraComponent : IComponent
{
    public IEntity Parent { get; }
    public ComponentFlag Flag { get; }

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
        _matrix = Matrix.CreateTranslation(new Vector3(-target.Transform.Position.X,
            -target.Transform.Position.Y, 0f));
    }

    public void Clear()
    {
        
    }
}