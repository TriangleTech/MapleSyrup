using System.Numerics;
using MapleSyrup.Common.Map;
using MapleSyrup.World.Systems;
using ZeroElectric.Vinculum;

namespace MapleSyrup.World;

public class LoginWorld : WorldBase
{
    public LoginWorld() 
    {
        Camera = new Camera2D()
        {
            offset = new Vector2(400, 300),
            rotation = 0f,
            target = Vector2.Zero,
            zoom = 1f
        };
    }
}