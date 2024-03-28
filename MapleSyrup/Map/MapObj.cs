using MapleSyrup.EC;
using MapleSyrup.EC.Components;
using MapleSyrup.Managers;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Map;

public class MapObj : IEntity
{
    private ManagerLocator _locator;
    
    public string Name { get; set; }
    public EntityFlag Flags { get; set; }
    public ComponentFlag CFlags { get; set; }
    public RenderLayer Layer { get; set; } = RenderLayer.TileObj0;
    public TransformComponent Transform { get; }
    public AnimationComponent? Animation { get; set; }
    public Texture2D Texture { get; set; }
    
    public MapObj(ManagerLocator locator)
    {
        _locator = locator;
        Flags = EntityFlag.Active | EntityFlag.MapObject;
        CFlags = ComponentFlag.Transform;
        Transform = new(this);
    }

    public void CleanUp()
    {
        Texture.Dispose();
        if (Animation != null)
            Animation.Clear();
    }
}