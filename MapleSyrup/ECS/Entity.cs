using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.ECS.Components;
using MapleSyrup.ECS.Components.Common;
using MapleSyrup.Gameplay.World;

namespace MapleSyrup.ECS;

public class Entity 
{
    protected Entity Parent;
    public readonly int Id;
    public readonly string Name;
    public readonly string Tag;
    public readonly List<object> Components;
    public bool IsEnabled;
    public bool IsDestroyed;
    public RenderLayer Layer;
    public int ZIndex;
    
    public Entity(int id, string name, string tag)
    {
        Id = id;
        Name = name;
        Tag = tag;
        Components = new();
        IsEnabled = true;
        IsDestroyed = false;
    }
    
    public void AddComponent(object component)
    {
        if (component == null)
            return;
        Components.Add(component);
    }

    public T GetComponent<T>()
    {
        return (T)Components.Find(x => x is T);
    }
    
    public bool HasComponent<T>()
    {
        return Components.Exists(x => x is T);
    }
    
    public void RemoveComponent(object component)
    {
        if (component == null)
            return;
        Components.Remove(component);
    }
}