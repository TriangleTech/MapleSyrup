using MapleSyrup.EC;
using MapleSyrup.EC.Components;
using MapleSyrup.Managers;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Map;

public class Portal : IEntity
{
    public string Name { get; set; }
    public EntityFlag Flags { get; set; }
    public ComponentFlag CFlags { get; set; }
    public TransformComponent Transform { get; }
    public AnimationComponent Animation { get; }
    public RenderLayer Layer { get; set; }
    public Texture2D Texture { get; set; }
    public PortalType Type { get; set; }
    public int PortalId { get; set; }
    public int TargetMap { get; set; }
    public string TargetPortal { get; set; }
    public string Script { get; set; }
    private ManagerLocator _locator;

    public Portal(ref ManagerLocator locator)
    {
        _locator = locator;
        Flags = EntityFlag.Active | EntityFlag.Portal;
        CFlags = ComponentFlag.Transform | ComponentFlag.Animation;
        Transform = new(this);
        Animation = new AnimationComponent(this);
    }

    public bool Scripted => Script != string.Empty;
}