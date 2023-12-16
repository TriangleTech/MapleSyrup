namespace MapleSyrup.ECS.Components;

public abstract class Component
{
    public ComponentType Type = ComponentType.None;
    public Entity Parent = null;
    public bool Enabled = false;
}