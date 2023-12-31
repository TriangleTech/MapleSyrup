using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.ECS;
using MapleSyrup.ECS.Components;
using MapleSyrup.ECS.Components.Common;
using MapleSyrup.Gameplay;
using MapleSyrup.Gameplay.Player;
using MapleSyrup.Gameplay.World;

namespace MapleSyrup.Subsystems;

public class SceneSystem : ISubsystem
{
    public GameContext Context { get; private set;}
    public Scene Current { get; private set; }
    
    public void Initialize(GameContext context)
    {
        Context = context;
    }

    public void Shutdown()
    {
        Current.Shutdown();
    }

    public void LoadScene(string worldId)
    {
        var scene = new Scene(Context);
        
        Current = scene;
        Current.LoadScene(worldId);
        
        var events = Context.GetSubsystem<EventSystem>();
        events.Publish(EventType.OnSceneCreated);
    }
    
    public void ChangeScene(string worldId)
    {
        var events = Context.GetSubsystem<EventSystem>();
        events.Publish(EventType.OnSceneChanged);
    }
    
    public void UnloadScene()
    {
        var events = Context.GetSubsystem<EventSystem>();
        events.Publish(EventType.OnSceneUnloaded);
    }
    
    #region Entities

    public Entity CreateEntity(string name, string tag)
    {
        var newId = Current.Entities.Count;
        var newEntity = new Entity(newId, name, tag);
        newEntity.AddComponent(new Transform());
        Current.Entities.Add(newEntity);

        return newEntity;
    }

    public Entity CreatePlayer(int id, string name, AvatarData data)
    {
        var player = new Entity(id, name, "Player");
        player.AddComponent(new Transform());
        player.AddComponent(data);
        player.AddComponent(new PlayerAppearance());
        Current.Entities.Add(player);

        var events = Context.GetSubsystem<EventSystem>();
        events.Publish(EventType.OnPlayerCreated);

        return player;
    }

    public void DestroyEntity(Entity entity)
    {
        entity.Components.Clear();
        Current.Entities.Remove(entity);
    }
    
    public Entity GetRoot()
    {
        return Current.Entities[0];
    }
    
    public List<Entity> GetEntities()
    {
        return Current.Entities.OrderBy(x => x.Layer).ThenBy(x => x.ZIndex).ToList();
    }
    
    public List<Entity> GetEntitiesByTag(string tag)
    {
        return Current.Entities.Where(x => x.Tag == tag).OrderBy(x => x.Layer).ThenBy(x => x.ZIndex).ToList();
    }
    
    public List<Entity> GetEntitiesByLayer(RenderLayer layer)
    {
        return Current.Entities.Where(x => x.Layer == layer).ToList();
    }
    
    public List<Entity> GetEntitiesByVisibility(bool visible)
    {
        return Current.Entities.OrderBy(x => x.Layer).ThenBy(x => x.ZIndex).Where(x => x.IsEnabled == visible).ToList();
    }
    
    public List<Entity> GetEntitiesWithComponent<T>() 
    {
        return Current.Entities.OrderBy(x => x.Layer).ThenBy(x => x.ZIndex).Where(x => x.HasComponent<T>()).ToList();
    }
    
    public List<Entity> GetEntitiesWithComponents<T, TU>()
    {
        return Current.Entities.OrderBy(x => x.Layer).ThenBy(x => x.ZIndex).Where(x => x.HasComponent<T>() && x.HasComponent<TU>()).ToList();
    }
    
    #endregion
    
    #region Boundary

    public float FarLeft => Current.Entities.Where(x=> !x.HasComponent<BackgroundItem>()).Min(x => x.GetComponent<Transform>().Position.X);
    public float FarRight => Current.Entities.Where(x=> !x.HasComponent<BackgroundItem>()).Max(x => x.GetComponent<Transform>().Position.X);
    public float FarTop => Current.Entities.Where(x=> !x.HasComponent<BackgroundItem>()).Min(x => x.GetComponent<Transform>().Position.Y);
    public float FarBottom => Current.Entities.Where(x=> !x.HasComponent<BackgroundItem>()).Max(x => x.GetComponent<Transform>().Position.Y);
    
    
    #endregion
    
    #region Portal
    
    public Entity GetPortalByName(string name)
    {
        return Current.Entities.Find(x => x.HasComponent<PortalInfo>() && x.GetComponent<PortalInfo>().Name == name);
    }
    
    public Entity GetPortalById(int id)
    {
        return Current.Entities.Find(x => x.HasComponent<PortalInfo>() && x.GetComponent<PortalInfo>().PortalId == id);
    }
    
    #endregion
    
    #region Player

    public Entity GetPlayerByName(string name)
    {
        return Current.Entities.Find(x => x.Name == name);
    }

    public Entity GetPlayerById(int id)
    {
        return Current.Entities.Find(x => x.Id == id);
    }
    
    #endregion
}