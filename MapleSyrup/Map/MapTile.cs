using MapleSyrup.EC;
using MapleSyrup.EC.Components;
using MapleSyrup.Managers;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Map;

public class MapTile : IEntity
{
    private ManagerLocator _locator;
    
    public string Name { get; set; }
    public EntityFlag Flags { get; set; }
    public ComponentFlag CFlags { get; set; }
    public TransformComponent Transform { get; }
    public RenderLayer Layer { get; set; } = RenderLayer.TileObj0;
    public Texture2D Texture;
    
    public MapTile(ref ManagerLocator locator)
    {
        _locator = locator;
        Flags = EntityFlag.Active | EntityFlag.MapTile;
        CFlags = ComponentFlag.Transform;
        Transform = new(this);
    }
}