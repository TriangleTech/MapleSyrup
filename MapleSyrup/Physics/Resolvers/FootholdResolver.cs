using MapleSyrup.ECS;
using MapleSyrup.Physics.Components;
using ZeroElectric.Vinculum;

namespace MapleSyrup.Physics.Resolvers;

public class FootholdResolver
{
    public void Update(float timeDelta)
    {
        var footholds = EntityFactory.Shared.GetEntitiesWithComponents<LineCollision>();
        var boxes = EntityFactory.Shared.GetEntitiesWithComponents<BoxCollision>();
        foreach (var foothold in footholds)
        {
            foreach (var box in boxes)
            {
                var controller = EntityFactory.Shared.GetComponent<GravityController>(box.Id);
                var lineCollision = EntityFactory.Shared.GetComponent<LineCollision>(foothold.Id);
                var boxCollision = EntityFactory.Shared.GetComponent<BoxCollision>(box.Id);
                
                if (Raylib.CheckCollisionRecs(boxCollision.Bounds, lineCollision.Bounds) && !controller.IsGrounded)
                {
                   // var transform = entityFactory.GetComponent<TransformComponent>(box);
                   EntityFactory.Shared.ChangeLayer(lineCollision.Layer, box);
                    controller.IsGrounded = true;
                }
            }
        }
    }
}