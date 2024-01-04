using System.Runtime.CompilerServices;
using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.ECS;
using MapleSyrup.Gameplay;
using MapleSyrup.Gameplay.Map;
using MapleSyrup.Gameplay.Player;
using MapleSyrup.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

    /// <summary>
    /// Loads a map using a specified ID.
    /// </summary>
    /// <param name="worldId">ID of the map.</param>
    /// <example>LoadScene("000010000")</example>
    public void LoadScene(string worldId)
    {
        var scene = new Scene(Context);
        
        Current = scene;
        Current.LoadScene(worldId);
        
        var events = Context.GetSubsystem<EventSystem>();
        events.Publish("SCENE_CREATED");
    }
    
    /// <summary>
    /// Not implemented yet
    /// </summary>
    /// <param name="worldId"></param>
    public void ChangeScene(string worldId)
    {
        var events = Context.GetSubsystem<EventSystem>();
        events.Publish("SCENE_CHANGED");
    }
    
    /// <summary>
    /// Not implemented yet
    /// </summary>
    public void UnloadScene()
    {
        var events = Context.GetSubsystem<EventSystem>();
        events.Publish("SCENE_UNLOADED");
    }
    
    #region Entities

    public Entity CreateEntity(string name, string tag)
    {
        var newId = Current.Entities.Count;
        var newEntity = new Entity(Context, newId, name, tag);
        newEntity.AddComponent(new Transform());
        Current.Entities.Add(newEntity);

        return newEntity;
    }

    public Entity CreatePlayer(int id, string name)
    {
        var player = new Entity(Context, id, name, "Player");
        Current.Entities.Add(player);

        var events = Context.GetSubsystem<EventSystem>();
        events.Publish("PLAYER_CREATED");

        return player;
    }

    public void DestroyEntity(Entity entity)
    {
        var events = Context.GetSubsystem<EventSystem>();
        var eventData = new EventData()
        {
            ["Entity"] = entity,
        };
        events.Publish("ENTITY_DESTROYED", eventData);
        
        entity.Destroy();
        Current.Entities.Remove(entity);
    }
    
    public Entity Root => Current.Entities[0];
    
    public List<Entity> GetEntities()
    {
        return Current.Entities.OrderBy(x => x.Layer).ThenBy(x => x.ZIndex).ToList();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    
    public float FarLeft => Current.Entities.Where(x=> !x.HasComponent<ParallaxBackground>() && x.HasComponent<Transform>()).Min(x => x.GetComponent<Transform>().Position.X);
    public float FarRight => Current.Entities.Where(x=> !x.HasComponent<ParallaxBackground>() && x.HasComponent<Transform>()).Max(x => x.GetComponent<Transform>().Position.X);
    public float FarTop => Current.Entities.Where(x=> !x.HasComponent<ParallaxBackground>() && x.HasComponent<Transform>()).Min(x => x.GetComponent<Transform>().Position.Y);
    public float FarBottom => Current.Entities.Where(x=> !x.HasComponent<ParallaxBackground>() && x.HasComponent<Transform>()).Max(x => x.GetComponent<Transform>().Position.Y);
    
    #endregion
    
    #region Portal
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Entity GetPortalByName(string name)
    {
        return Current.Entities.Find(x => x.HasComponent<PortalInfo>() && x.GetComponent<PortalInfo>().Name == name);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Entity GetPortalById(int id)
    {
        return Current.Entities.Find(x => x.HasComponent<PortalInfo>() && x.GetComponent<PortalInfo>().PortalId == id);
    }
    
    #endregion
    
    #region Player

    /// <summary>
    /// Gets player by username, this shouldn't cause any issues.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Entity GetPlayerByName(string name)
    {
        return Current.Entities.Find(x => x.Name == name);
    }

    /// <summary>
    /// Gets player by ID. Use the ID generated by the server.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Entity? GetPlayerById(int id)
    {
        return Current.Entities.Find(x => x.Id == id);
    }

    public Entity CreateTestPlayer()
    {
        var resource = Context.GetSubsystem<ResourceSystem>();
        var events = Context.GetSubsystem<EventSystem>();
        var testPlayer = new Entity(Context, 1000000, "TestPlayer", "Player");
        testPlayer.AddComponent(new Transform());
        testPlayer.AddComponent(new AvatarLook());

        _ = testPlayer.GetComponent<Transform>().Position = Vector2.Zero;
        
        var look = testPlayer.GetComponent<AvatarLook>();
        look.Layers["arm"] = resource.GetCharItem("00002000.img/stand1/0/arm").data as Texture2D;
        look.Layers["body"] = resource.GetCharItem("00002000.img/stand1/0/body").data as Texture2D;
        look.Layers["head"] = resource.GetCharItem("00012000.img/stand1/0/head").data as Texture2D;
        
        look.Position["body"] = Vector2.Zero;
        look.Position["arm"] = Vector2.Zero;
        look.Position["head"] = Vector2.Zero;

        //look.Origin["body"] = (Vector2)resource.GetCharItem("00002000.img/stand1/0/body/origin").data;
        //look.Origin["arm"] = (Vector2)resource.GetCharItem("00002000.img/stand1/0/arm/origin").data;
        //look.Origin["head"] = (Vector2)resource.GetCharItem("00012000.img/stand1/0/head/origin").data;
        
        Current.Entities.Add(testPlayer);
        events.Publish("PLAYER_ON_SPAWN");

        return testPlayer;
    }
    
    #endregion
}