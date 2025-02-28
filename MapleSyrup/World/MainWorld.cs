using System.Numerics;
using MapleSyrup.World.Systems;
using ZeroElectric.Vinculum;

namespace MapleSyrup.World;

public class MainWorld : WorldBase
{
    public MainWorld() 
    {
        Camera = new Camera2D()
        {
            offset = Vector2.Zero,
            target = Vector2.Zero,
            rotation = 0f,
            zoom = 1.0f
        };
    }
}