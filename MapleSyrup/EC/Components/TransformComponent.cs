using Microsoft.Xna.Framework;

namespace MapleSyrup.EC.Components;

public class TransformComponent : IComponent
{
    public IEntity Parent { get; }
    public ComponentFlag Flag { get; } = ComponentFlag.Transform;
    
    public Vector2 Position;
    public Vector2 Origin;
    public int zIndex;

    public TransformComponent(IEntity parent)
    {
        Parent = parent;
        Position = Vector2.Zero;
        Origin = Vector2.Zero;
        zIndex = 0;
    }
}