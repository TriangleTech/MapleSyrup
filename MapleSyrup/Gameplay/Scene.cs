using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.ECS;
using MapleSyrup.ECS.Components;
using MapleSyrup.ECS.Systems;
using MapleSyrup.Gameplay.World;
using MapleSyrup.Resources;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace MapleSyrup.Gameplay;

public class Scene : EventObject
{
    public readonly List<Entity> Entities;
    public readonly List<DrawableSystem> DrawableSystems;
    public readonly List<UpdateableSystem> UpdateableSystems;
    private Queue<int> recycledIds;
    private int entityCount;
    private string worldId;
    
    public Scene(GameContext context) 
        : base(context)
    {
        Entities = new List<Entity>();
        DrawableSystems = new List<DrawableSystem>();
        UpdateableSystems = new List<UpdateableSystem>();
        recycledIds = new Queue<int>();
        entityCount = 0;
        worldId = string.Empty;
        
        
        RegisterEvent(EventType.OnSceneCreated);
        RegisterEvent(EventType.OnSceneChanged);
        RegisterEvent(EventType.OnSceneRender);
        RegisterEvent(EventType.OnSceneUpdate);
        RegisterEvent(EventType.OnSceneUnloaded);
        
        SubscribeToEvent(EventType.OnEngineRender, new Subscriber() { EventType = EventType.OnEngineRender, Sender = this, Event = OnRender });
        SubscribeToEvent(EventType.OnEngineUpdate, new Subscriber() { EventType = EventType.OnEngineUpdate, Sender = this, Event = OnUpdate });
    }
    
    public Entity CreateEntity(string name, string tag = "entity")
    {
        if (recycledIds.Count > 0)
        {
            var id = recycledIds.Dequeue();
            if (Entities[id] != null)
                throw new Exception($"Entity with id {id} is not null");
            var entity = new Entity(Context, id, name, tag);
            Entities[id] = entity;
            return entity;
        }
        
        var newId = entityCount++;
        var newEntity = new Entity(Context, newId, name, tag);
        Entities.Add(newEntity);
        return newEntity;
    }
    
    public void DestroyEntity(Entity entity)
    {
        if (entity == null)
            return;
        //Context.PublishEvent(EventType.OnEntityDestroyed, eventData);
        recycledIds.Enqueue(entity.Id);
        Entities.Remove(entity);
    }

    public void LoadScene(string id)
    {
        if (worldId != string.Empty)
        {
            // TODO: Clear scene and do transition
        }
        
        DrawableSystems.Add(new BackgroundSystem(Context));
        DrawableSystems.Add(new TileObjSystem(Context));
        
        worldId = id;
        var root = CreateEntity("root", "Scene");
        root.AddComponent(new WorldInfo());
        
        LoadWorldInfo();
        LoadBackground();
        LoadTiles();
        LoadObjects();
        Entities.Sort((a, b) => a.CompareTo(b.Layer));
        PublishEvent(EventType.OnSceneCreated, new EventData
        {
            ["Scene"] = this,
            ["WorldId"] = id
        });
    }

    private void LoadWorldInfo()
    {
        if (Entities[0].Tag != "Scene")
            throw new Exception("Why is the root entity not tagged as Scene?");
        
        var info = Entities[0].GetComponent<WorldInfo>();
        var resource = Context.GetSubsystem<ResourceSystem>();
        info.IsTown = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/town").data == 1;
        info.MobRate = (double)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/mobRate").data;
        info.Bmg = (string)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/bgm").data;
        info.ReturnMap = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/returnMap").data;
        info.HideMinimap = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/hideMinimap").data == 1;
        info.ForcedReturn = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/forcedReturn").data;
        info.MoveLimit = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/moveLimit").data;
        info.MapMark = (string)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/mapMark").data;
        info.FieldLimit = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/fieldLimit").data;
        if (resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/VRTop").resourceType != ResourceType.Unknown)
        {
            info.VrTop = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/VRTop").data;
            info.VrLeft = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/VRLeft").data;
            info.VrBottom = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/VRBottom").data;
            info.VrRight = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/VRRight").data;
        }
        // TODO: Generate World Bounds if VrTop, VrLeft, VrBottom, VrRight are not set
    }

    private void LoadBackground()
    {
        var resource = Context.GetSubsystem<ResourceSystem>();
        for (int i = 0; i < resource.GetNodeCount($"Map/Map/Map{worldId[0]}/{worldId}.img/back"); i++)
        {
            var no = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/back/{i}/no").data;
            var x = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/back/{i}/x").data;
            var y = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/back/{i}/y").data;
            var rx = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/back/{i}/rx").data;
            var ry = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/back/{i}/ry").data;
            var type = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/back/{i}/type").data;
            var cx = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/back/{i}/cx").data;
            var cy = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/back/{i}/cy").data;
            var bS = (string)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/back/{i}/bS").data;
            var a = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/back/{i}/a").data;
            var front = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/back/{i}/front").data;
            var f = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/back/{i}/f").data;
            var ani = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/back/{i}/ani").data;
            var origin = (Vector2)resource.GetItem($"Map/Back/{bS}.img/{ (ani == 0 ? "back" : "ani") }/{no}/origin").data;
            
            var background = CreateEntity($"background_{i}", "Background");
            background.Layer = RenderLayer.Background;
            background.AddComponent(new BackgroundItem()
            {
                Color = Color.White,
                Enabled = true,
                Position = new Vector2(x, y),
                Origin = origin,
                Texture = resource.GetItem($"Map/Back/{bS}.img/{ (ani == 0 ? "back" : "ani") }/{no}").data as Texture2D,
            });
        }
    }
    
    private void LoadTiles()
    {
        var layer = 0;
        var resource = Context.GetSubsystem<ResourceSystem>();
        var tileSet = (string)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/0/info/tS").data;

        do
        {
            for (int i = 0; i < resource.GetNodeCount($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/tile"); i++)
            {
                var x = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/tile/{i}/x").data;
                var y = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/tile/{i}/y").data;
                var u = (string)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/tile/{i}/u").data;
                var no = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/tile/{i}/no").data;
                var zM = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/tile/{i}/zM").data;
                var origin = (Vector2)resource.GetItem($"Map/Tile/{tileSet}.img/{u}/{no}/origin").data;
                var tile = CreateEntity($"tile_{i}", "Tile");
                tile.Layer = (RenderLayer)layer + 1;
                tile.AddComponent(new TileItem()
                {
                    Position = new Vector2(x, y),
                    Origin = origin,
                    Texture = resource.GetItem($"Map/Tile/{tileSet}.img/{u}/{no}").data as Texture2D,
                });
            }

            layer++;
        } while (layer < 8);
    }
    
    private void LoadObjects()
    {
        
    }
    
    private void OnUpdate(EventData eventData)
    {
        var data = new EventData
        {
            ["Entities"] = Entities,
            ["GameTime"] = eventData["GameTime"] as GameTime,
        };
        
        PublishEvent(EventType.OnSceneUpdate, data);
    }
    
    private void OnRender(EventData eventData)
    {
        PublishEvent(EventType.OnSceneRender);
    }
    
    public void Shutdown()
    {
        for (var index = 0; index < Entities.Count; index++)
        {
            var entity = Entities[index];
            DestroyEntity(entity);
        }

        Entities.Clear();
        DrawableSystems.Clear();
        UpdateableSystems.Clear();
        recycledIds.Clear();
        entityCount = 0;
        worldId = string.Empty;
        PublishEvent(EventType.OnSceneUnloaded);
    }
}