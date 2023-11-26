using MapleSyrup.ECS.Entities;
using MapleSyrup.Factories;

namespace MapleSyrup.ECS.Systems;

public interface IDrawableSystem : ISystem
{
    void Draw(EntityFactory entityFactory);
}