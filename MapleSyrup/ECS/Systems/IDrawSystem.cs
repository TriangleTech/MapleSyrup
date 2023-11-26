using MapleSyrup.Gameplay;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.ECS.Systems;

public interface IDrawSystem
{
    void Initialize(GraphicsDevice graphicsDevice);
    void Shutdown();
    void Draw(MapleWorld world);
}