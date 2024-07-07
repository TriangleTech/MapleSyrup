using MapleSyrup.EC;
using MapleSyrup.Event;
using MapleSyrup.Map;

namespace MapleSyrup.Managers;

public class EntityManager : IManager
{
    private ManagerLocator _locator;
    
    public void Initialize(ManagerLocator locator)
    {
        _locator = locator;
    }

    /// <summary>
    /// Creates an entity and dispatches it to the scene.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Create<T>() where T : IEntity
    {
        var _event = _locator.GetManager<EventManager>();
        var entity = Activator.CreateInstance(typeof(T), _locator) as IEntity;
        _event.Dispatch(EventFlag.OnEntityCreated, entity);

        return (T)entity;
    }
    
    public T Create<T>(string name, string tag) where T : IEntity
    {
        var _event = _locator.GetManager<EventManager>();
        var entity = Activator.CreateInstance(typeof(T), _locator, name, tag) as IEntity;
        _event.Dispatch(EventFlag.OnEntityCreated, entity);
        return (T)entity;
    }

    public MapBackground CreateBackground()
    {
        var _event = _locator.GetManager<EventManager>();
        var entity = new MapBackground(_locator);
        _event.Dispatch(EventFlag.OnEntityCreated, entity);

        return entity;
    }

    /// <summary>
    /// Deactivates the entity and dispatches it to the scene for removal.
    /// </summary>
    /// <param name="entity"></param>
    public void Remove(IEntity entity)
    {
        var _event = _locator.GetManager<EventManager>();
        if (entity & EntityFlag.Active)
            entity.Flags &= ~EntityFlag.Active;
        _event.Dispatch(EventFlag.OnEntityRemoved, entity);
    }

    public void Shutdown()
    {
        
    }
}