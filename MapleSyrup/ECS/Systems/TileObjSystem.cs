using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.ECS.Components;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace MapleSyrup.ECS.Systems;

public class TileObjSystem : DrawableSystem
{
    private SpriteBatch spriteBatch;
    
    public TileObjSystem(GameContext context) 
        : base(context)
    {
        spriteBatch = new SpriteBatch(Context.GraphicsDevice);
    }

    public override void OnRender(EventData eventData)
    {
        var scene = Context.GetSubsystem<SceneSystem>().Current;
        var entities = scene.Entities.OrderBy(x => x.Layer).ThenBy(x => x.ZIndex).ToList();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone);
        for (int i = 0; i < entities.Count; i++)
        {
            if (!entities[i].IsEnabled || !entities[i].HasComponent<MapItem>())
                continue;
            var item = entities[i].GetComponent<MapItem>();
            spriteBatch.Draw(item.Texture, item.Position, null, Color.White, 0f, item.Origin, 1f, SpriteEffects.None, 0f);
        }
        spriteBatch.End();
        entities.Clear();
        base.OnRender(eventData);
    }
}