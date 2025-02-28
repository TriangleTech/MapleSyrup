using MapleSyrup.ECS;
using MapleSyrup.ECS.Components.Common;
using MapleSyrup.ECS.Interfaces;
using MapleSyrup.Physics.Components;
using MapleSyrup.Resources;
using MapleSyrup.Tests.Components;
using ZeroElectric.Vinculum;

namespace MapleSyrup.Tests.Systems;

public class RedSquareTest : IDrawSystem, IUpdateSystem
{
    public void Draw(Entity entity)
    {
        if (!EntityFactory.Shared.HasComponent<RedSquare>(entity.Id)) return;

        var redSquare = EntityFactory.Shared.GetComponent<RedSquare>(entity.Id);
        var collision = EntityFactory.Shared.GetComponent<BoxCollision>(entity.Id);

        Raylib.DrawRectangleRec(redSquare.Bounds, redSquare.Color);
        Raylib.DrawRectangleLinesEx(collision.Bounds, 3, Raylib.BLACK);
    }

    public void Update(Entity entity, float timeDelta)
    {
        if (!EntityFactory.Shared.HasComponent<RedSquare>(entity.Id)) return;

        var redSquare = EntityFactory.Shared.GetComponent<RedSquare>(entity.Id);
        var transform = EntityFactory.Shared.GetComponent<TransformComponent>(entity.Id);
        var box = EntityFactory.Shared.GetComponent<BoxCollision>(entity.Id);

        var temp = redSquare.Bounds;
        redSquare.Bounds = new Rectangle(transform.Position.X, transform.Position.Y, temp.Width, temp.Height);
        box.Bounds = redSquare.Bounds;
    }
}