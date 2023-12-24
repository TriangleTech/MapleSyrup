using Microsoft.Xna.Framework;

namespace MapleSyrup.ECS.Components;

public class Portal : Component
{
    public string Name;
    public Vector2 Position;
    public int TargetMapId;
    public string TargetPortalName;
    public int PortalType;
    public int PortalId;
    public string ScriptName;
    
}