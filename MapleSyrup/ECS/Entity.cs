using System.Runtime.CompilerServices;
using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.Gameplay.Map;
using MapleSyrup.Graphics;
using MapleSyrup.Subsystems;

namespace MapleSyrup.ECS;

public class Entity
{
    private readonly GameContext Context;
    private readonly List<object> Components;
    public readonly int Id;
    public readonly string Name;
    public readonly string Tag;
    public bool IsEnabled;
    public RenderLayer Layer;
    public int ZIndex;
    
    public Entity(GameContext context, int id, string name, string tag)
    {
        Id = id;
        Name = name;
        Tag = tag;
        Components = new();
        IsEnabled = true;
        Context = context;
    }
    
    public void AddComponent(object component)
    {
        Components.Add(component);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T? GetComponent<T>()
    {
        return (T?)Components.Find(x => x is T);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasComponent<T>()
    {
        return Components.Exists(x => x is T);
    }
    
    public void RemoveComponent(object component)
    {
        Components.Remove(component);
    }

    public void SetVisibility(bool enabled)
    {
        IsEnabled = enabled;
    }

    public void Destroy()
    {
        Components.Clear();
    }
}