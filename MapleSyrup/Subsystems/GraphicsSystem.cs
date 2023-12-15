using MapleSyrup.Core;
using MapleSyrup.Nodes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MapleSyrup.Subsystems;

public class GraphicsSystem : ISubsystem
{
    public GameContext Context { get; private set; }
    private SpriteBatch spriteBatch;

    public void Initialize(GameContext context)
    {
        Context = context;
        spriteBatch = new SpriteBatch(Context.GraphicsDevice);
    }

    public void BeginDrawing()
    {
        
    }
    
    public void Shutdown()
    {
        
    }
}