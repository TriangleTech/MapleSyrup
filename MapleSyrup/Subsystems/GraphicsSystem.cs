using MapleSyrup.Core;
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
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
    }

    public void Draw(Texture2D texture, Vector2 position)
    {
        spriteBatch.Draw(texture, position, Color.White);
    }
    
    public void EndDrawing()
    {
        spriteBatch.End();
    }
    
    public void Shutdown()
    {
        
    }
}