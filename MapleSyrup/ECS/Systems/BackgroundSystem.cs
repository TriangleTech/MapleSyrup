using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.ECS.Components;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace MapleSyrup.ECS.Systems;

public class BackgroundSystem : DrawableSystem
{
    private SpriteBatch spriteBatch;
    
    public BackgroundSystem(GameContext context) 
        : base(context)
    {
        spriteBatch = new SpriteBatch(Context.GraphicsDevice);
    }

    protected override void OnRender(EventData eventData)
    {
        var scene = Context.GetSubsystem<SceneSystem>().Current;
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone);
        for (int i = 0; i < scene.Entities.Count; i++)
        {
            if (!scene.Entities[i].IsEnabled || !scene.Entities[i].HasComponent<BackgroundItem>())
                continue;
            var background = scene.Entities[i].GetComponent<BackgroundItem>();
            spriteBatch.Draw(background.Texture, background.Position, null, Color.White, 0f, background.Origin, 1f, SpriteEffects.None, 0f);
        }
        spriteBatch.End();
        
        base.OnRender(eventData);
    }
}