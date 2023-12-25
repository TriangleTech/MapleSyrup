namespace MapleSyrup.ECS.Components;

public abstract class Component
{
    public Entity Parent = null;
    public bool IsEnabled = false;
}