using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.ECS.Components;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace MapleSyrup.ECS.Systems;

public class BackgroundSystem : DrawableSystem
{
    public BackgroundSystem(GameContext context) 
        : base(context)
    {
    }

    public override void OnRender(EventData eventData)
    {
        var scene = Context.GetSubsystem<SceneSystem>().Current;
        var graphics = Context.GetSubsystem<GraphicsSystem>();
        var spriteBatch = new SpriteBatch(Context.GraphicsDevice);
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone);
        for (int i = 0; i < scene.Entities.Count; i++)
        {
            if (!scene.Entities[i].IsEnabled || !scene.Entities[i].HasComponent<BackgroundItem>())
                continue;
            var background = scene.Entities[i].GetComponent<BackgroundItem>();
            spriteBatch.Draw(background.Texture, background.Position, Color.White);
        }
        spriteBatch.End();
        
        base.OnRender(eventData);
    }
}