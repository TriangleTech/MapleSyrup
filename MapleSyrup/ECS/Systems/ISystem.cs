using MapleSyrup.Factories;

namespace MapleSyrup.ECS.Systems;

public interface ISystem
{
    void Initialize(EntityFactory entityFactory);
    void Shutdown();
}