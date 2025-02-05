using System.Collections.Immutable;
using System.Numerics;
using Client.ECS.Components;
using Client.ECS.Components.Common;
using Client.ECS.Components.Map;
using CommunityToolkit.HighPerformance;
using ZeroElectric.Vinculum;
using Transform = Client.ECS.Components.Common.Transform;

namespace Client.ECS;

public class EntityFactory
{
    private readonly List<Entity> _entities = new(1024);
    private readonly Dictionary<int, List<IComponent>> _components = new(1024);
    private readonly Queue<int> _recycledIds = new(256);
    private bool _dirty;
    private int _entityCount;

    public static EntityFactory Shared { get; private set; }

    public EntityFactory()
    {
        Shared = this;
    }

    public Entity CreateEntity(string name = "Default", string tag = "Default")
    {
        var id = _recycledIds.Count > 0 ? _recycledIds.Dequeue() : _entityCount++;
        var entity = new Entity { Id = id, Name = name, Tag = tag, Visible = true };
        _components.Add(id, new List<IComponent>());
        _entities.Add(entity);
        AddComponent(new Transform { Owner = id, Position = Vector2.Zero, Origin = Vector2.One }); // every entity has a transform component
        _dirty = true;
        
        return entity;
    }

    public void DestroyEntity(int id)
    {
        var index = _entities.FindIndex(x => x.Id == id);
        if (index == -1) return;
        _entities[index].Visible = false;
        _entities.RemoveAt(index);
        _components[id].Clear();
        _components.Remove(id);
        _recycledIds.Enqueue(id);
    }
    
    public void AddComponent<T>(T component) where T : class, IComponent
    {
        if (_components[component.Owner].OfType<T>().Any())
            return;
        _components[component.Owner].Add(component);
    }
    
    public void RemoveComponent<T>(int id) where T : class, IComponent
    {
        throw new NotImplementedException();
    }

    public T GetComponent<T>(int id) where T : class, IComponent
    {
        foreach (var component in _components[id])
        {
            if (component is T t)
                return t;
        }
        
        throw new Exception($"Entity {id} does not have component {typeof(T).Name}");
    }

    public Span<int> GetAllWithComponent<T>() where T : class, IComponent
    {
        List<int> ids = new(512);
        foreach (var entity in _entities)
        {
            if (_components[entity.Id].FindIndex(x => x is T) == -1) // TODO: Keep for now, change later. Causes about 200MB of mem alloc.
                continue; 
            if (!entity.Visible)
                continue;
            ids.Add(entity.Id);
        }
        return ids.AsSpan();
    }

    public Span<int> GetAllWithTag(string tag)
    {
        List<int> ids = new(512);
        foreach (var entity in _entities)
        {
            if (entity.Tag != tag || !entity.Visible) // TODO: Keep for now, change later. Causes about 200MB of mem alloc.
                continue; 
            ids.Add(entity.Id);
        }

        return ids.AsSpan();
    }

    public void Sort()
    {
        if (!_dirty) return;
        
        _entities.Sort((a, b) =>
        {
            if (a.Layer != b.Layer)
                return a.Layer.CompareTo(b.Layer);

            var t1 = GetComponent<Transform>(a.Id);
            var t2 = GetComponent<Transform>(b.Id);
            return t1.Z.CompareTo(t2.Z);
        });
        
        _dirty = false;
    }
    
    public void DestroyAll()
    {
        _entities.Clear();
        _components.Clear();
        _recycledIds.Clear();
    }
    
    public void Shutdown()
    {
        DestroyAll();
        _entities.Clear();
        _components.Clear();
        _recycledIds.Clear();
    }
}