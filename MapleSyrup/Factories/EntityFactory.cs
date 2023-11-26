using System.Collections.Concurrent;
using MapleSyrup.ECS.Entities;

namespace MapleSyrup.Factories;

public class EntityFactory : IFactory, IDisposable
{
    public readonly List<Entity> WorldEntities;
    private int count;
    private Queue<int> recycledId;
    
    public EntityFactory()
    {
        WorldEntities = new();
        recycledId = new ();
    }

    public void Initialize()
    {
        
    }

    public void Shutdown()
    {
        
    }

    public Entity Create()
    {
        Entity entity = recycledId.Count > 0 ? new Entity(recycledId.Dequeue()) : new Entity(count);
        count++;
        WorldEntities.Add(entity);

        return WorldEntities[entity.Id];
    }

    public void OnDestroyed(Entity entity)
    {
        if (entity == null)
            return;
        
        WorldEntities.Remove(entity);
        recycledId.Enqueue(entity.Id);
    }

    public void Clear()
    {
        foreach (var entity in WorldEntities)
        {
            entity.Dispose();
        }
        WorldEntities.Clear();
    }

    public List<Entity> GetWithComponent<T>()
    {
        return WorldEntities.FindAll(entities => entities.HasComponent<T>());
    }

    public void Dispose()
    {
        
    }
}