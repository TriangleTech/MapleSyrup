using MapleSyrup.ECS.Interfaces;
using ZeroElectric.Vinculum;

namespace MapleSyrup.Physics.Components;

public class BoxCollision : IComponent
{
    public int Owner { get; init; }
    public Rectangle Bounds { get; set; }
}