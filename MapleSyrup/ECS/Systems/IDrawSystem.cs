using MapleSyrup.Resources;

namespace MapleSyrup.ECS.Systems;

public interface IDrawSystem
{
    void Draw(EntityFactory entityFactory, ResourceFactory resourceFactory);
}