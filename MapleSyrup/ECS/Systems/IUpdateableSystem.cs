using MapleSyrup.ECS.Entities;
using MapleSyrup.Factories;

namespace MapleSyrup.ECS.Systems;

public interface IUpdateableSystem : ISystem
{
    void Update(EntityFactory entityFactory, float timeDelta);
}