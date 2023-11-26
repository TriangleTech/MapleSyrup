using MapleSyrup.ECS.Entities;

namespace MapleSyrup.ECS.Components;

public interface IComponent
{
    Entity Parent { get; }
}