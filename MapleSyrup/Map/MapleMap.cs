using MapleSyrup.EC;
using MapleSyrup.Event;
using MapleSyrup.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Map;

public class MapleMap : IEventListener
{
    public EventFlag Flags { get; }
    private readonly ManagerLocator _locator;
    
    public MapleMap(string mapId, ManagerLocator locator)
    {
        Flags = EventFlag.OnMapChanged;
        _locator = locator;
        _locator.GetManager<EventManager>().Register(this);
    }

    public void Load()
    {
        LoadInfo();
        LoadBackground();
        LoadTile();
        LoadObj();
    }

    public void Unload()
    {
        var events = _locator.GetManager<EventManager>();
        events.Dispatch(EventFlag.OnMapUnloaded);
    }
    
    public void ProcessEvent(EventFlag flag)
    {
        
    }

    public void ProcessEvent(EventFlag flag, IEntity entity)
    {
        
    }

    #region Load Functions 
    
    public void LoadInfo()
    {
        var resource = _locator.GetManager<ResourceManager>();
    }

    public void LoadBackground()
    {
        
    }

    public void LoadTile()
    {
        
    }

    public void LoadObj()
    {
        
    }
    
    #endregion

    #region Draw/Update Functions

    public void RenderBackground(SpriteBatch spriteBatch, Background entity)
    {
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap,
            DepthStencilState.Default, RasterizerState.CullNone, null, entity.Parallax.GetMatrix());
        spriteBatch.Draw(entity.Parallax.Texture, entity.Parallax.Position, null, Color.White,
            0f, entity.Parallax.Origin, 1f, SpriteEffects.None, 0f);
        spriteBatch.End();
    }

    public void UpdateBackground(Background entity)
    {
        entity.Parallax.UpdateMatrix();
    }
    
    #endregion
}