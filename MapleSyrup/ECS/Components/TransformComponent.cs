using MapleSyrup.ECS.Entities;
using Microsoft.Xna.Framework;

namespace MapleSyrup.ECS.Components;

public class TransformComponent : IComponent
{
    public Entity Parent { get; }
    public Vector2 Position;
    public Vector2 Origin;
    public int Z;

    public TransformComponent(Entity entity)
    {
        Parent = entity;
        Position = Vector2.Zero;
        Origin = Vector2.Zero;
        Z = 0;
    }
}