using MapleSyrup.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Subsystem;

public class GraphicsSystem : ISubsystem
{
    public MapleContext Context { get; private set; }
    private SpriteBatch spriteBatch;

    public void Initialize(MapleContext context)
    {
        Context = context;
        spriteBatch = new(Context.GraphicsDevice);
    }
    
    public void Shutdown()
    {
        
    }
}