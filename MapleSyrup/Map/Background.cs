using MapleSyrup.EC;
using MapleSyrup.EC.Components;
using MapleSyrup.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Map;

public class Background : IEntity
{
    public string Name { get; set; }
    public EntityFlag Flags { get; set; }
    public ComponentFlag CFlags { get; }
    public TransformComponent Transform { get; }
    public ParallaxComponent Parallax { get; }
    public RenderLayer Layer { get; set; } = RenderLayer.Background;
    private ManagerLocator _locator;
    
    public Background(ref ManagerLocator locator)
    {
        _locator = locator;
        Flags = EntityFlag.Active | EntityFlag.Background;
        CFlags = ComponentFlag.Transform | ComponentFlag.Parallax;
        Transform = new(this);
        Parallax = new ParallaxComponent(this);
    }
}