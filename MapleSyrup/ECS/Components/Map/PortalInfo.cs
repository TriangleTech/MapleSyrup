using MapleSyrup.Gameplay.World;
using Microsoft.Xna.Framework;

namespace MapleSyrup.ECS.Components.Map;

public class PortalInfo : Component
{
    public string Name;
    public Vector2 Position;
    public int TargetMapId;
    public string TargetPortalName;
    public PortalType PortalType;
    public int PortalId;
    public string ScriptName;
    
}