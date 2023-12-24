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
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MapleSyrup.Gameplay;

public class Scene 
{
    private readonly GameContext Context;
    public readonly List<Entity> Entities;
    private readonly List<object> entitySystems;
    private int entityCount;
    private string worldId;

    public Scene(GameContext context)
    {
        Context = context;
        Entities = new List<Entity>();
        entitySystems = new();
        entityCount = 0;
        worldId = string.Empty;

        var events = Context.GetSubsystem<EventSystem>();
        events.Subscribe(this, EventType.OnRender, OnDraw);
        events.Subscribe(this, EventType.OnUpdate, OnUpdate);
    }

    public Entity CreateEntity(string name, string tag = "entity")
    {
        var newId = entityCount++;
        var newEntity = new Entity(newId, name, tag);
        newEntity.AddComponent(new Transform());
        Entities.Add(newEntity);
        return newEntity;
    }

    public void DestroyEntity(Entity entity)
    {
        if (entity == null)
            return;
        Entities.Remove(entity);
    }
    
    public void LoadScene(string id)
    {
        if (worldId != string.Empty)
        {
            // TODO: Clear scene and do transition
        }

        // The order these are added is the order they are updated and rendered
        entitySystems.Add(new BackgroundSystem(Context));
        entitySystems.Add(new CloudSystem(Context));
        entitySystems.Add(new TileObjSystem(Context));
        entitySystems.Add(new CameraSystem(Context));
        entitySystems.Add(new MovementSystem(Context));

        worldId = id;
        var root = CreateEntity("root", "Scene");
        root.AddComponent(new WorldInfo());
        root.AddComponent(new Camera());
        
        LoadWorldInfo();
        LoadBackground();
        LoadTiles();
        LoadObjects();

        var events = Context.GetSubsystem<EventSystem>();
        events.Publish(EventType.OnSceneCreated);
    }

    private void LoadWorldInfo()
    {
        if (Entities[0].Tag != "Scene")
            throw new Exception("Why is the root entity not tagged as Scene?");

        var info = Entities[0].GetComponent<WorldInfo>();
        var resource = Context.GetSubsystem<ResourceSystem>();
        if (resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/town").resourceType != ResourceType.Unknown)
            info.IsTown = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/town").data == 1;
        if (resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/swim").resourceType != ResourceType.Unknown)
            info.CanSwim = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/swim").data == 1;
        if (resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/fieldLimit").resourceType !=
            ResourceType.Unknown)
            info.FieldLimit = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/fieldLimit").data;
        if (resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/returnMap").resourceType !=
            ResourceType.Unknown)
            info.ReturnMap = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/returnMap").data;
        if (resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/forcedReturn").resourceType !=
            ResourceType.Unknown)
            info.ForcedReturn = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/forcedReturn").data;
        if (resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/mobRate").resourceType !=
            ResourceType.Unknown)
            info.MobRate = (double)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/mobRate").data;
        if (resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/bgm").resourceType != ResourceType.Unknown)
            info.Bmg = (string)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/bgm").data;
        if (resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/hideMinimap").resourceType !=
            ResourceType.Unknown)
            info.HideMinimap = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/hideMinimap").data ==
                               1;
        if (resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/moveLimit").resourceType !=
            ResourceType.Unknown)
            info.MoveLimit = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/moveLimit").data;
        if (resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/mapMark").resourceType !=
            ResourceType.Unknown)
            info.MapMark = (string)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/mapMark").data;
        if (resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/VRTop").resourceType != ResourceType.Unknown)
        {
            info.VrTop = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/VRTop").data;
            info.VrLeft = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/VRLeft").data;
            info.VrBottom = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/VRBottom").data;
            info.VrRight = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/info/VRRight").data;
            info.Bounds = new Rectangle(info.VrLeft, info.VrTop, info.VrRight - info.VrLeft,
                info.VrBottom - info.VrTop);
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

            if (ani == 0)
            {
                var origin = (Vector2)resource.GetItem($"Map/Back/{bS}.img/back/{no}/origin").data;
                var background = CreateEntity($"background_{i}", "Background");
                background.Layer = front == 1 ? RenderLayer.Foreground : RenderLayer.Background;
                var transform = background.GetComponent<Transform>();
                transform.Position = new Vector2(x, y);
                transform.Origin = origin;
                background.AddComponent(new BackgroundItem()
                {
                    Color = Color.White,
                    Enabled = true,
                    Texture = resource.GetItem($"Map/Back/{bS}.img/back/{no}").data as Texture2D,
                    Rx = rx,
                    Ry = ry,
                    Type = (BackgroundType)type,
                    Cx = cx,
                    Cy = cy,
                    Alpha = a,
                    Flipped = f == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                });
            }
            else
            {
                //var animationCount = resource.GetNodeCount($"Map/Back/{bS}.img/ani/{no}");
            }
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
                if (tileSet == string.Empty ||
                    resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/info/tS").resourceType !=
                    ResourceType.Unknown)
                    tileSet = (string)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/info/tS").data;

                var x = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/tile/{i}/x").data;
                var y = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/tile/{i}/y").data;
                var u = (string)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/tile/{i}/u").data;
                var no = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/tile/{i}/no").data;
                var z = (int)resource.GetItem($"Map/Tile/{tileSet}.img/{u}/{no}/z").data;
                var zM = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/tile/{i}/zM").data;
                var origin = (Vector2)resource.GetItem($"Map/Tile/{tileSet}.img/{u}/{no}/origin").data;
                var tile = CreateEntity($"tile_{i}", "MapItem");
                tile.Layer = (RenderLayer)layer + 1;
                tile.ZIndex = z + 10 * (3000 * (layer + 1) - zM) - 1073721834;
                
                var transform = tile.GetComponent<Transform>();
                transform.Position = new Vector2(x, y);
                transform.Origin = origin;
                tile.AddComponent(new MapItem()
                {
                    Texture = resource.GetItem($"Map/Tile/{tileSet}.img/{u}/{no}").data as Texture2D,
                });
            }

            layer++;
        } while (layer < 8);
    }

    private void LoadObjects()
    {
        var layer = 0;
        var resource = Context.GetSubsystem<ResourceSystem>();

        do
        {
            for (int i = 0; i < resource.GetNodeCount($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/obj"); i++)
            {
                var oS = (string)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/obj/{i}/oS").data;
                var l0 = (string)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/obj/{i}/l0").data;
                var l1 = (string)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/obj/{i}/l1").data;
                var l2 = (string)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/obj/{i}/l2").data;
                var x = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/obj/{i}/x").data;
                var y = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/obj/{i}/y").data;
                var z = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/obj/{i}/z").data;
                var f = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/obj/{i}/f").data;
                var zM = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/obj/{i}/zM").data;
                var nodeCount = resource.GetNodeCount($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}");

                var obj = CreateEntity($"obj_{i}", "MapItem");
                obj.Layer = (RenderLayer)layer + 1;
                obj.ZIndex = (int)(30000 * layer + z) - 1073739824;
                var transform = obj.GetComponent<Transform>();
                transform.Position = new Vector2(x, y);

                if (resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/obj/{i}/r").resourceType !=
                    ResourceType.Unknown)
                    obj.IsEnabled = true;

                if (nodeCount == 1)
                {
                    var origin = (Vector2)resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/0/origin").data;
                    transform.Origin = origin;
                    
                    obj.AddComponent(new MapItem()
                    {
                        Texture = resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/0").data as Texture2D,
                        Flipped = f == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    });
                }
                else
                {
                    // TODO: Handle seats later
                    LoadAnimatedObject(ref obj, nodeCount, oS, l0, l1, l2, x, y);
                }
            }

            layer++;
        } while (layer < 8);
    }

    private void LoadAnimatedObject(ref Entity obj, int nodeCount, string oS, string l0, string l1, string l2, int x, int y)
    {
        var resource = Context.GetSubsystem<ResourceSystem>();
        obj.AddComponent(new AnimatedMapItem());
        var animated = obj.GetComponent<AnimatedMapItem>();
        var transform = obj.GetComponent<Transform>();
        transform.Position = new Vector2(x, y);

        for (int j = 0; j < nodeCount; j++)
        {
            if (resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/seat").resourceType ==
                ResourceType.Directory)
            {
                DestroyEntity(obj);
                continue;
            }

            if (resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/{j}/blend").resourceType ==
                ResourceType.Integer)
            {
                DestroyEntity(obj);
                continue;
            }

            var origin = (Vector2)resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/{j}/origin").data;
            transform.Origin = origin;
            animated.Positions.Add(new Vector2(x, y));
            animated.Origins.Add(origin);
            animated.Frames.Add(resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/{j}").data as Texture2D);
            animated.Delay.Add((int)resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/{j}/delay")
                .data);
            if (resource.GetItem("Map/Obj/{oS}.img/{l0}/{l1}/{l2}/{j}/a0").resourceType !=
                ResourceType.Unknown)
                animated.StartAlpha.Add((int)resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/{j}/a0").data);
            if (resource.GetItem("Map/Obj/{oS}.img/{l0}/{l1}/{l2}/{j}/a1").resourceType !=
                ResourceType.Unknown)
                animated.EndAlpha.Add((int)resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/{j}/a1")
                    .data);
        }
    }

    private void LoadPortals()
    {
        var resource = Context.GetSubsystem<ResourceSystem>();
        var worldPath = $"Map/Map/Map{worldId[0]}/{worldId}.img";
        var portalCount = resource.GetNodeCount($"{worldPath}/portal");
        
        for (int i = 0; i < portalCount; i++)
        {
            var name = (string)resource.GetItem($"{worldPath}/portal/{i}/pn").data;
            var script = (string)resource.GetItem($"{worldPath}/portal/{i}/script").data;
            var x = (int)resource.GetItem($"{worldPath}/portal/{i}/x").data;
            var y = (int)resource.GetItem($"{worldPath}/portal/{i}/y").data;
            var targetMap = (int)resource.GetItem($"{worldPath}/portal/{i}/tm").data; // The map it leads to
            var targetPortal = (string)resource.GetItem($"{worldPath}/portal/{i}/tn").data; // The portal you end up on
            var portal = CreateEntity($"portal_{name}", "Portal");
            var transform = portal.GetComponent<Transform>();
            
            portal.Layer = RenderLayer.Foreground;
            transform.Position = new Vector2(x, y);
            portal.AddComponent(new Portal()
            {
                Name = name,
                ScriptName = script,
                TargetMapId = targetMap,
                TargetPortalName = targetPortal,
                PortalId = i
            });
        }
    }

    private void OnUpdate(EventData eventData)
    {
        var data = new EventData
        {
            ["GameTime"] = eventData["GameTime"] as GameTime,
        };

        var events = Context.GetSubsystem<EventSystem>();
        events.Publish(EventType.OnSceneUpdate, data);
    }

    private void OnDraw(EventData eventData)
    {
        var events = Context.GetSubsystem<EventSystem>();
        events.Publish(EventType.OnSceneRender);
    }

    public void Shutdown()
    {
        for (var index = 0; index < Entities.Count; index++)
        {
            var entity = Entities[index];
            DestroyEntity(entity);
        }

        Entities.Clear();
        entitySystems.Clear();
        entityCount = 0;
        worldId = string.Empty;

        var events = Context.GetSubsystem<EventSystem>();
        events.Publish(EventType.OnSceneUnloaded);
    }
}