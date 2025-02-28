using MapleSyrup.Resources;

namespace MapleSyrup.ECS.Interfaces;

public interface IUpdateSystem
{
    void Update(Entity entity, float timeDelta);
}