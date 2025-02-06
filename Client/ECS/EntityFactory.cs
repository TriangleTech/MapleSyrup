using System.Collections.Immutable;
using System.Numerics;
using Client.ECS.Components;
using Client.ECS.Components.Common;
using Client.ECS.Components.Map;
using CommunityToolkit.HighPerformance;
using ZeroElectric.Vinculum;
using Transform = Client.ECS.Components.Common.Transform;

namespace Client.ECS;

/// <summary>
/// The <see cref="EntityFactory"/> class manages the creation, destruction, and
/// enumeration of any <see cref="Entity"/> created through the game session.
/// </summary>
public class EntityFactory
{
    /// <summary>
    /// Contains the entities currently in the scene, regardless of visibility. TODO: Maybe separate them?
    /// </summary>
    private readonly List<Entity> _entities = new(1024);
    
    /// <summary>
    /// Contains the components of all the entity in the scene.
    /// </summary>
    private readonly Dictionary<int, List<IComponent>> _components = new(1024);
    
    /// <summary>
    /// Contains the IDs of any entities that have been destroyed, so they can be reused.
    /// </summary>
    private readonly Queue<int> _recycledIds = new(256);
    
    /// <summary>
    /// Checks whether an entity has been added to the scene in order to sort the entities
    /// by their layer and/or z-buffer.
    /// </summary>
    private bool _dirty;
    
    /// <summary>
    /// The number of entities in the scene.
    /// </summary>
    private int _entityCount;

    /// <summary>
    /// The instance of the created <see cref="EntityFactory"/>. All instances are created in the
    /// <see cref="Client.Windowing.GameWindow"/> class.
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
    /// <param name="name">Name of the entity.</param>
    /// <param name="tag">The related tag to find the entity.</param>
    /// <returns></returns>
    public Entity CreateEntity(string name = "Default", string tag = "Default")
    {
        var id = _recycledIds.Count > 0 ? _recycledIds.Dequeue() : _entityCount++;
        var entity = new Entity { Id = id, Name = name, Tag = tag, Visible = true };
        _components.Add(id, new List<IComponent>());
        _entities.Add(entity);
        _dirty = true;
        AddComponent(new Transform { Owner = id, Position = Vector2.Zero, Origin = Vector2.One }); // every entity has a transform component
        
        return entity;
    }

    /// <summary>
    /// Destroys an <see cref="Entity"/> based on the id.
    /// </summary>
    /// <param name="id">ID the <see cref="Entity"/></param>
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

    /// <summary>
    /// Returns the IDs of all Entities with a specified <see cref="IComponent"/>.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="IComponent"/></typeparam>
    /// <returns>An array of IDs of <c>visible</c> entities within the scene.</returns>
    public Span<int> GetAllWithComponent<T>() where T : class, IComponent
    {
        List<int> ids = new(512);
        foreach (var entity in _entities)
        {
            if (_components[entity.Id].FindIndex(x => x is T) == -1)
                continue; 
            if (!entity.Visible)
                continue;
            ids.Add(entity.Id);
        }
        return ids.AsSpan();
    }

    /// <summary>
    /// Returns the IDs of all entities in the scene with a specified <c>Tag</c>
    /// </summary>
    /// <param name="tag">The tag the entities reference.</param>
    /// <returns>An array containing the entities' ID.</returns>
    public Span<int> GetAllWithTag(string tag)
    {
        List<int> ids = new(512);
        foreach (var entity in _entities)
        {
            if (entity.Tag != tag || !entity.Visible)
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
    
    public void Shutdown()
    {
        _entities.Clear();
        _components.Clear();
        _recycledIds.Clear();
    }
}