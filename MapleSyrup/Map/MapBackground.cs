using MapleSyrup.EC;
using MapleSyrup.EC.Components;
using MapleSyrup.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Map;

public class MapBackground : IEntity
{
    private ManagerLocator _locator;
    
    public string Name { get; set; }
    public EntityFlag Flags { get; set; }
    public ComponentFlag CFlags { get; set; }
    public TransformComponent Transform { get; }
    public ParallaxComponent Parallax { get; }
    public RenderLayer Layer { get; set; } = RenderLayer.Background;
    public Texture2D Texture { get; set; }

    public MapBackground(ManagerLocator locator)
    {
        _locator = locator;
        Flags = EntityFlag.Active | EntityFlag.Background;
        CFlags = ComponentFlag.Transform | ComponentFlag.Parallax;
        Transform = new(this);
        Parallax = new ParallaxComponent(this);
    }
}