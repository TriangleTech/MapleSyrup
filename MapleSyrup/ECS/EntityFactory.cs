using System.Collections.Concurrent;
using System.Numerics;
using CommunityToolkit.HighPerformance;
using MapleSyrup.ECS.Components;
using MapleSyrup.ECS.Components.Common;
using MapleSyrup.ECS.Interfaces;
using MapleSyrup.Windowing;

namespace MapleSyrup.ECS;

/// <summary>
/// The <see cref="EntityFactory"/> class manages the creation, destruction, and
/// enumeration of any <see cref="Entity"/> created through the game session.
/// </summary>
public class EntityFactory
{
    /// <summary>
    /// Contains the entities currently in the scene, regardless of visibility.
    /// </summary>
    private readonly List<Entity> _entities = new(1024);
    private readonly ConcurrentQueue<Entity> _pendingAdd = new();
    private readonly ConcurrentQueue<Entity> _pendingRemove = new();
    
    /// <summary>
    /// Contains the components of all the entity in the scene.
    /// </summary>
    private readonly ConcurrentDictionary<int, List<IComponent>> _components = new(Environment.ProcessorCount, 1024);
    
    /// <summary>
    /// Contains the IDs of any entities that have been destroyed, so they can be reused.
    /// </summary>
    private readonly Queue<int> _recycledIds = new(256);
    
    /// <summary>
    /// Checks whether an entity has been added to the scene in order to sort the entities
    /// by their layer and/or z-buffer.
    /// </summary>
    private bool _needSort;
    
    /// <summary>
    /// The number of entities in the scene.
    /// </summary>
    private int _entityCount;
    
    public List<Entity> Entities => _entities;

    /// <summary>
    /// The instance of the created <see cref="EntityFactory"/>. All instances are created in the
    /// <see cref="GameWindow"/> class.
    /// </summary>
    public static EntityFactory Shared { get; private set; }

    /// <summary>
    /// Default Constructor
    /// </summary>
    public EntityFactory()
    {
        Shared = this;
    }

    /// <summary>
    /// Creates an <see cref="Entity"/> with a default name and tag.
    /// </summary>
    /// <param name="layer">Layer in which the entity resides.</param>
    /// <param name="name">Name of the entity.</param>
    /// <param name="tag">The related tag to find the entity.</param>
    /// <returns></returns>
    public Entity CreateEntity(int layer = 0, string name = "Default", string tag = "Default")
    {
        lock (_entities)
        {
            var id = _recycledIds.Count > 0 ? _recycledIds.Dequeue() : _entityCount;
            var entity = new Entity { Id = id, Layer = layer, Name = name, Tag = tag, Visible = true };
            if (!_components.TryAdd(id, new List<IComponent>()))
                throw new Exception("An entity with the same id already exists");
            _pendingAdd.Enqueue(entity);
            _entityCount++;
            
            AddComponent(new TransformComponent
            {
                Owner = id, 
                Position = Vector2.Zero, 
                Origin = Vector2.One
            }); // every entity has a transform component
            
            return entity;
        }
    }

    /// <summary>
    /// Destroys an <see cref="Entity"/> based on the id.
    /// </summary>
    /// <param name="id">ID the <see cref="Entity"/></param>
    public void DestroyEntity(Entity entity)
    {
        lock (_entities)
        {
            _pendingRemove.Enqueue(entity);
        }
    }

    public void ChangeLayer(int layer, Entity entity)
    {
        entity.Layer = layer;
        _needSort = true;
    }

    public void ChangeZBuffer(int entityId, int zBuffer)
    {
        var transform = GetComponent<TransformComponent>(entityId);
        transform.Z = zBuffer;
        _needSort = true;
    }
    
    /// <summary>
    /// Adds a component to the specified <see cref="Entity"/>. 
    /// </summary>
    /// <param name="component"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>If the <see cref="Entity"/> has the component, it'll do nothing.</returns>
    public void AddComponent<T>(T component) where T : class, IComponent
    {
        if (_components[component.Owner].OfType<T>().Any())
            return;
        _components[component.Owner].Add(component);
    }
    
    /// <summary>
    /// Removes a component from the specified <see cref="Entity"/>.
    /// </summary>
    /// <param name="id">ID of the <see cref="Entity"/></param>
    /// <typeparam name="T">The type of the <see cref="IComponent"/></typeparam>
    /// <exception cref="NotImplementedException"></exception>
    public void RemoveComponent<T>(int id) where T : class, IComponent
    {
        _components[id].RemoveAll(c => c is T);
    }

    /// <summary>
    /// Returns a <see cref="IComponent"/> for the specified <see cref="Entity"/>.
    /// </summary>
    /// <param name="id">ID of the <see cref="Entity"/>.</param>
    /// <typeparam name="T">The type of <see cref="IComponent"/>.</typeparam>
    /// <returns>The located <see cref="IComponent"/>.</returns>
    /// <exception cref="Exception">Throws when the <see cref="IComponent"/> doesn't exist.</exception>
    public T GetComponent<T>(int id) where T : class, IComponent
    {
        foreach (var component in _components[id])
        {
            if (component is T t)
                return t;
        }
        
        throw new Exception($"Entity {id} does not have component {typeof(T).Name}");
    }

    public bool HasComponent<T>(int id) where T : class, IComponent
    {
        foreach (var component in _components[id])
        {
            if (component is T t)
                return true;
        }

        return false;
    }
    
    public int GetEntityWithComponent<T1>() 
        where T1 : class, IComponent
    {
        var id = -999;
        foreach (var entity in _entities)
        {
            if (_components[entity.Id].FindIndex(x => x is T1) == -1)
                continue; 
            if (!entity.Visible)
                continue;
            id = entity.Id;
            break;
        }

        return id;
    }

    /// <summary>
    /// Returns the IDs of all Entities with a specified <see cref="IComponent"/>.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="IComponent"/></typeparam>
    /// <returns>An array of IDs of <c>visible</c> entities within the scene.</returns>
    public Span<Entity> GetEntitiesWithComponents<T>() 
        where T : class, IComponent
    {
        List<Entity> ids = new(512);
        foreach (var entity in _entities)
        {
            if (_components[entity.Id].FindIndex(x => x is T) == -1)
                continue; 
            if (!entity.Visible)
                continue;
            ids.Add(entity);
        }
        return ids.AsSpan();
    }

    public void ProcessPending()
    {
        lock (_entities)
        {
            while (_pendingAdd.TryDequeue(out var entity))
            {
                _entities.Add(entity);
                _needSort = true;
            }
            
            while (_pendingRemove.TryDequeue(out var entity))
            {
                var index = _entities.FindIndex(x => x.Id == entity.Id);
                if (index == -1) 
                    continue;
                entity.Visible = false;
                _recycledIds.Enqueue(entity.Id);
                _entities.RemoveAt(index);
                _needSort = true;
        
                if (!_components.TryRemove(entity.Id, out var components))
                    continue;
                components.Clear();
            }
        }
    }

    public void Sort()
    {
        if (!_needSort) 
            return;
        
        _entities.Sort((a, b) =>
        {
            if (a.Layer != b.Layer)
                return a.Layer.CompareTo(b.Layer);

            var t1 = GetComponent<TransformComponent>(a.Id);
            var t2 = GetComponent<TransformComponent>(b.Id);
            return t1.Z.CompareTo(t2.Z);
        });
        
        _needSort = false;
    }
    
    public void Shutdown()
    {
        _entities.Clear();
        _components.Clear();
        _recycledIds.Clear();
    }
}