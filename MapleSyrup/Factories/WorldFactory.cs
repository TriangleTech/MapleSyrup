using MapleSyrup.Gameplay;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Factories;

public class WorldFactory : IFactory
{
    public MapleWorld Current { get; internal set; }
    
    public WorldFactory()
    {
        
    }
    
    public void Initialize()
    {
        
    }

    public void Shutdown()
    {
        
    }

    public MapleWorld CreateWorld(int worldId, GraphicsDevice graphicsDevice)
    {
        // TODO: Fade
        Current.Clear();
        Current = new MapleWorld(worldId, graphicsDevice);

        return Current;
    }
}