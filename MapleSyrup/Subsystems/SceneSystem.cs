using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.ECS;
using MapleSyrup.ECS.Components;
using MapleSyrup.ECS.Components.Map;
using MapleSyrup.Gameplay;
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
        scene.LoadScene(worldId);
        Current = scene;
        
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
    
    public List<Entity> GetEntitiesWithComponent<T>() where T : Component
    {
        return Current.Entities.OrderBy(x => x.Layer).ThenBy(x => x.ZIndex).Where(x => x.HasComponent<T>()).ToList();
    }
    
    public List<Entity> GetEntitiesWithComponents<T, TU>() where T : Component where TU : Component
    {
        return Current.Entities.OrderBy(x => x.Layer).ThenBy(x => x.ZIndex).Where(x => x.HasComponent<T>() && x.HasComponent<TU>()).ToList();
    }
    
    public Entity GetPortalByName(string name)
    {
        return Current.Entities.Find(x => x.HasComponent<PortalInfo>() && x.GetComponent<PortalInfo>().Name == name);
    }
    
    public Entity GetPortalById(int id)
    {
        return Current.Entities.Find(x => x.HasComponent<PortalInfo>() && x.GetComponent<PortalInfo>().PortalId == id);
    }
}