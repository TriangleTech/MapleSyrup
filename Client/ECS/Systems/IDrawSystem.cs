using Client.Resources;

namespace Client.ECS.Systems;

public interface IDrawSystem
{
    void Draw(EntityFactory entityFactory, ResourceFactory resourceFactory);
}