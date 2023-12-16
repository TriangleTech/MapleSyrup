using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.ECS.Components;
using MapleSyrup.Gameplay.World;

namespace MapleSyrup.ECS;

public class Entity : EventObject, IComparable<RenderLayer>, IEquatable<RenderLayer>
{
    protected Entity Parent;
    public readonly int Id;
    public readonly string Name;
    public readonly string Tag;
    public readonly List<Component> Components;
    public bool IsEnabled;
    public bool IsDestroyed;
    public RenderLayer Layer;
    
    public Entity(GameContext context, int id, string name, string tag)
        : base(context)
    {
        Id = id;
        Name = name;
        Tag = tag;
        Components = new List<Component>();
        IsEnabled = true;
        IsDestroyed = false;
    }
    
    public void AddComponent(Component component)
    {
        if (component == null)
            return;
        if (component.Parent != null)
            throw new Exception("Component already has a parent");
        component.Parent = this;
        Components.Add(component);
    }

    public T GetComponent<T>() where T : Component
    {
        return (T)Components.Find(x => x is T);
    }
    
    public bool HasComponent<T>() where T : Component
    {
        return Components.Exists(x => x is T);
    }
    
    public void RemoveComponent(Component component)
    {
        if (component == null)
            return;
        if (component.Parent != this)
            throw new Exception("Component does not belong to this entity");
        component.Parent = null;
        Components.Remove(component);
    }
    
    public void SetParent(Entity parent)
    {
        if (parent == null)
            return;
        if (parent == this)
            throw new Exception("Cannot set parent to self");
        if (Parent != null)
            throw new Exception("Entity already has a parent");
        Parent = parent;
    }

    public int CompareTo(RenderLayer other)
    {
        if (Layer == other)
            return 0;
        if (Layer < other)
            return -1;
        return 1;
    }

    public bool Equals(RenderLayer other)
    {
        return Layer == other;
    }
}