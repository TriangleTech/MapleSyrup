using MapleSyrup.ECS.Interfaces;
using ZeroElectric.Vinculum;

namespace MapleSyrup.Tests.Components;

public class RedSquare : IComponent
{
    public int Owner { get; init; }
    public Rectangle Bounds { get; set; }
    public Color Color { get; } = Raylib.RED;
}