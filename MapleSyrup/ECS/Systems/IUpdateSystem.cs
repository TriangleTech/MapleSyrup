using MapleSyrup.Resources;

namespace MapleSyrup.ECS.Systems;

public interface IUpdateSystem
{
    void Update(EntityFactory entityFactory, ResourceFactory resourceFactory, float timeDelta);
}