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

        SubscribeToEvent(EventType.OnEngineRender,
            new Subscriber() { EventType = EventType.OnEngineRender, Sender = this, Event = OnRender });
        SubscribeToEvent(EventType.OnEngineUpdate,
            new Subscriber() { EventType = EventType.OnEngineUpdate, Sender = this, Event = OnUpdate });
    }

    public Entity CreateEntity(string name, string tag = "entity")
    {
        if (recycledIds.Count > 0)
        {
            var id = recycledIds.Dequeue();
            var entity = new Entity(Context, id, name, tag);
            Entities.Add(entity);
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

        // The order these are added is the order they are updated and rendered
        DrawableSystems.Add(new BackgroundSystem(Context));
        DrawableSystems.Add(new TileObjSystem(Context));
        UpdateableSystems.Add(new AnimMapItemSystem(Context));

        worldId = id;
        var root = CreateEntity("root", "Scene");
        root.AddComponent(new WorldInfo());

        LoadWorldInfo();
        LoadBackground();
        LoadTiles();
        LoadObjects();
        PublishEvent(EventType.OnSceneCreated);
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
            info.Bounds = new RectangleF(info.VrLeft, info.VrTop, info.VrRight - info.VrLeft,
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
                background.AddComponent(new BackgroundItem()
                {
                    Color = Color.White,
                    Enabled = true,
                    Position = new Vector2(x, y),
                    Origin = origin,
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
                var x = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/tile/{i}/x").data;
                var y = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/tile/{i}/y").data;
                var u = (string)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/tile/{i}/u").data;
                var no = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/tile/{i}/no").data;
                var z = (int)resource.GetItem($"Map/Tile/{tileSet}.img/{u}/{no}/z").data;
                var zM = (int)resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/tile/{i}/zM").data;
                var origin = (Vector2)resource.GetItem($"Map/Tile/{tileSet}.img/{u}/{no}/origin").data;
                var tile = CreateEntity($"tile_{i}", "Tile");
                tile.Layer = (RenderLayer)(1 << (layer + 1));
                tile.ZIndex = z + 10 * (3000 * layer - zM) - 0x3FFFB1EA; //1073721834;
                tile.AddComponent(new MapItem()
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

                var obj = CreateEntity($"obj_{i}", "Object");
                obj.Layer = (RenderLayer)layer + 1;
                obj.ZIndex = (int)(30000 * layer + z) - 0x3FFFF830; //1073739824;

                if (resource.GetItem($"Map/Map/Map{worldId[0]}/{worldId}.img/{layer}/obj/{i}/r").resourceType !=
                    ResourceType.Unknown)
                    obj.IsEnabled = false;

                if (nodeCount == 1)
                {
                    var origin = (Vector2)resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/0/origin").data;
                    obj.AddComponent(new MapItem()
                    {
                        Position = new Vector2(x, y),
                        Origin = origin,
                        Texture = resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/0").data as Texture2D,
                        Flipped = f == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    });
                }
                else
                {
                    // TODO: Handle seats later
                    
                    obj.AddComponent(new AnimatedMapItem());
                    
                    var animated = obj.GetComponent<AnimatedMapItem>();
                    //animated.Positions = new List<Vector2>();
                    //animated.Origins = new List<Vector2>();
                    //animated.Frames = new List<Texture2D>();
                    //animated.Delay = new List<int>();
                    //animated.Alpha0 = new List<int>();
                    //animated.Alpha255 = new List<int>();

                    for (int j = 0; j < nodeCount; j++)
                    {
                        if (resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/seat").resourceType ==
                            ResourceType.Directory)
                        {
                            DestroyEntity(obj);
                            continue;
                        }

                        if (resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/{j}/blend").resourceType == ResourceType.Integer)
                        {
                            DestroyEntity(obj);
                            continue;
                        }
                        
                        var origin = (Vector2)resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/{j}/origin").data;
                        animated.Positions.Add(new Vector2(x, y));
                        animated.Origins.Add(origin);
                        animated.Frames.Add(resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/{j}").data as Texture2D);
                        //if (resource.GetItem("Map/Obj/{oS}.img/{l0}/{l1}/{l2}/{j}/delay").resourceType !=
                            //ResourceType.Unknown)
                            animated.Delay.Add((int)resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/{j}/delay")
                                .data);
                        if (resource.GetItem("Map/Obj/{oS}.img/{l0}/{l1}/{l2}/{j}/a0").resourceType !=
                            ResourceType.Unknown)
                            animated.Alpha0.Add((int)resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/{j}/a0").data);
                        if (resource.GetItem("Map/Obj/{oS}.img/{l0}/{l1}/{l2}/{j}/a1").resourceType !=
                            ResourceType.Unknown)
                            animated.Alpha255.Add((int)resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/{j}/a1")
                                .data);
                    }
                }
            }

            layer++;
        } while (layer < 8);
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