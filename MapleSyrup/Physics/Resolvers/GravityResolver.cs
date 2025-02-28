using MapleSyrup.ECS;
using MapleSyrup.ECS.Components.Common;
using MapleSyrup.Physics.Components;

namespace MapleSyrup.Physics.Resolvers;

public class GravityResolver
{
    private const float Gravity = 9.81f;
    
    public void Update(float timeDelta)
    {
        var entities = EntityFactory.Shared.GetEntitiesWithComponents<GravityController>();
        foreach (var entity in entities)
        {
            var controller = EntityFactory.Shared.GetComponent<GravityController>(entity.Id);
            if (controller.Weight <= 0 || controller.IsGrounded) // TODO: For now I don't care about this, eventually we'll take it out.
                continue;
            
            var transform = EntityFactory.Shared.GetComponent<TransformComponent>(entity.Id);
            var fallRate = Gravity * 5f * (timeDelta / 1000f);
            
            transform.Position.Y += fallRate;
        }
    }
}