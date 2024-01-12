using MapleSyrup.EC;
using MapleSyrup.Event;

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
        _event.Dispatch(EventFlag.OnEntityCreated, ref entity);

        return (T)entity;
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
        _event.Dispatch(EventFlag.OnEntityRemoved, ref entity);
    }

    public void Shutdown()
    {
        
    }
}