using Client.Resources;

namespace Client.ECS.Systems;

public interface IUpdateSystem
{
    void Update(EntityFactory entityFactory, ResourceFactory resourceFactory, float timeDelta);
}