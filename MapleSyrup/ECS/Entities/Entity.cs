using MapleSyrup.ECS.Components;
using MapleSyrup.ECS.Events;
using Microsoft.Xna.Framework;

namespace MapleSyrup.ECS.Entities;

public class Entity : IDisposable
{
    public readonly int Id = 0;
    public string Name = "";
    public string Tag = "";
    private TransformComponent transform;
    private List<IComponent> components;
    private List<Entity> children; // TODO: Handle this later
    private List<EventType> subscribedEvents;

    internal Entity(int newId)
    {
        Id = newId;
        Name = $"entity{Id}";
        Tag = "default";
        transform = new TransformComponent(this);
        components = new List<IComponent>() { transform };
        children = new();
        subscribedEvents = new();
    }

    public void AddComponent<T>()
    {
        
    }

    public void AddComponent<T>(T component)
    {
        throw new NotImplementedException();
    }

    public T GetComponent<T>()
    {
        throw new NotImplementedException();
    }

    public bool HasComponent<T>()
    {
        return components.Any(x => x is T);
    }
    
    public Entity SubscribeToEvent(EventType eventType)
    {
        if (subscribedEvents.Any(x => x == eventType))
            return this;
        subscribedEvents.Add(eventType);
        return this;
    }

    public void Dispose()
    {
        components.Clear();
        foreach (var entity in children)
            entity.Dispose();
        subscribedEvents.Clear();
    }
}