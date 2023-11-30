using MapleSyrup.Core;
using MapleSyrup.Gameplay.Map;
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
    
    public void Shutdown()
    {
        
    }

    public void BeginDraw()
    {
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
    }

    public void Draw(Node2D node)
    {
        // TODO: Fix this later
        var sprite = node.GetComponent<Sprite>();
        spriteBatch.Draw(sprite.Texture, node.Position, null, Color.White, node.Rotation, 
            node.Origin, node.Scale, SpriteEffects.None, (int)node.Layer / 10f);
    }

    public void DrawMapItem(MapItem item)
    {
        spriteBatch.Draw(item.Texture, item.Position, null, Color.White, item.Rotation, 
            item.Origin, item.Scale, SpriteEffects.None, (int)item.Layer / 10f);
    }

    public void EndDraw()
    {
        spriteBatch.End();
    }
}