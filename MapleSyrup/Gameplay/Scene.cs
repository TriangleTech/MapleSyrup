using System.Diagnostics;
using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.ECS;
using MapleSyrup.ECS.Components;
using MapleSyrup.ECS.Components.Map;
using MapleSyrup.ECS.Systems.Map;
using MapleSyrup.ECS.Systems.Player;
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
        entity.Components.Clear();
        Entities.Where(x => x.Id == entity.Id).ToList().ForEach(x => Entities.Remove(x));
    }

    public void LoadScene(string id)
    {
        if (worldId != string.Empty)
        {
            // TODO: Clear scene and do transition
        }

        worldId = id;
        var root = CreateEntity("root", "Scene");
        root.AddComponent(new WorldInfo());
        root.AddComponent(new Camera(Context.GraphicsDevice.Viewport));

        // The order these are added is the order they are updated and rendered
        entitySystems.Add(new BackgroundSystem(Context));
        entitySystems.Add(new CloudSystem(Context));
        entitySystems.Add(new TileObjSystem(Context));
        entitySystems.Add(new PortalSystem(Context));
        entitySystems.Add(new CameraSystem(Context));
        entitySystems.Add(new MovementSystem(Context));

        LoadBackground();
        LoadTiles();
        LoadObjects();
        LoadPortals();
        LoadWorldInfo();

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
        else
        {
            var scene = Context.GetSubsystem<SceneSystem>();
            var left = scene.FarLeft;
            var right = scene.FarRight;
            var top = scene.FarTop;
            var bottom = scene.FarBottom;

            info.Bounds = new Rectangle((int)left, (int)top, (int)(right - left), (int)(bottom - top));
        }
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

            if (bS == string.Empty)
                continue;

            if (ani == 0)
            {
                var origin = (Vector2)resource.GetItem($"Map/Back/{bS}.img/back/{no}/origin").data;
                //var background = CreateEntity($"background_{i}", "Background");
                //var transform = background.GetComponent<Transform>();

                switch ((BackgroundType)type)
                {
                    case BackgroundType.Default:
                        var background = CreateEntity($"background_{i}", "Background");
                        var transform = background.GetComponent<Transform>();
                        background.Layer = front == 1 ? RenderLayer.Foreground : RenderLayer.Background;
                        background.ZIndex = 0;
                        transform.Position = new Vector2(x, y);
                        transform.Origin = origin;
                        background.AddComponent(new BackgroundItem()
                        {
                            Color = Color.White,
                            Texture = resource.GetItem($"Map/Back/{bS}.img/back/{no}").data as Texture2D,
                            Rx = rx,
                            Ry = ry,
                            Type = (BackgroundType)type,
                            Cx = cx,
                            Cy = cy,
                            Alpha = a,
                            Flipped = f == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                        });
                        break;
                    case BackgroundType.HorizontalTiling:
                        break;
                    case BackgroundType.HorizontalScrolling:
                        var back = CreateEntity($"background_{i}", "Background");
                        var backT = back.GetComponent<Transform>();
                        back.Layer = front == 1 ? RenderLayer.Foreground : RenderLayer.Background;
                        back.ZIndex = 0;
                        backT.Position = new Vector2(x, y);
                        backT.Origin = origin;
                        back.AddComponent(new BackgroundItem()
                        {
                            Color = Color.White,
                            Texture = resource.GetItem($"Map/Back/{bS}.img/back/{no}").data as Texture2D,
                            Rx = rx,
                            Ry = ry,
                            Type = (BackgroundType)type,
                            Cx = cx,
                            Cy = cy,
                            Alpha = a,
                            Flipped = f == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                        });

                        for (int j = 1; j < 2; j++)
                        {
                            Entity scrolling = CreateEntity($"background_{i}_tiled", "Background");
                            var eTransform = scrolling.GetComponent<Transform>();
                            scrolling.AddComponent(new BackgroundItem());
                            scrolling.Layer = front == 1 ? RenderLayer.Foreground : RenderLayer.Background;
                            scrolling.ZIndex = 0;
                            scrolling.AddComponent(new BackgroundItem());
                            var backItem = scrolling.GetComponent<BackgroundItem>();
                            backItem.Color = Color.White;
                            backItem.Texture = resource.GetItem($"Map/Back/{bS}.img/back/{no}").data as Texture2D;
                            backItem.Rx = rx;
                            backItem.Ry = ry;
                            backItem.Type = (BackgroundType)type;
                            backItem.Cx = cx;
                            backItem.Cy = cy;
                            backItem.Alpha = a;
                            backItem.Flipped = f == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                            backItem.Offset = (back.GetComponent<BackgroundItem>().Texture.Width * j);

                            eTransform.Position = new Vector2(x + backItem.Offset, y);
                            eTransform.Origin = origin;

                            Debug.WriteLine("Tiled Entity Created");
                        }

                        break;
                    case BackgroundType.HorizontalScrollingHVTiling:
                        break;
                    case BackgroundType.VerticalTiling:
                        break;
                    case BackgroundType.VerticalScrolling:
                        break;
                    case BackgroundType.VerticalScrollingHVTiling:
                        break;
                }
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
                    // TODO: Handle Obstacles
                    if (resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/obstacle").resourceType !=
                        ResourceType.Unknown)
                        DestroyEntity(obj);
                    else
                    {
                        LoadAnimatedObject(ref obj, nodeCount, oS, l0, l1, l2, x, y);
                    }
                }
            }

            layer++;
        } while (layer < 8);
    }

    private void LoadAnimatedObject(ref Entity obj, int nodeCount, string oS, string l0, string l1, string l2, int x,
        int y)
    {
        var resource = Context.GetSubsystem<ResourceSystem>();
        var transform = obj.GetComponent<Transform>();
        transform.Position = new Vector2(x, y);

        // TODO: Handle seats
        if (resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/seat").resourceType ==
            ResourceType.Directory)
        {
            DestroyEntity(obj);
            Debug.WriteLine("Seat Detected, Skipping...");
            return;
        }

        // TODO: Handle blend 
        if (resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/blend").resourceType ==
            ResourceType.Integer)
        {
            DestroyEntity(obj);
            Debug.WriteLine("Blend Detected, Skipping...");
            return;
        }

        // Frame Animation
        if (resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/0/a0").resourceType ==
            ResourceType.Unknown)
        {
            obj.AddComponent(new AnimatedMapItem());
            var animated = obj.GetComponent<AnimatedMapItem>();

            for (int j = 0; j < nodeCount; j++)
            {
                var origin = (Vector2)resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/{j}/origin").data;
                transform.Origin = origin;
                animated.Positions.Add(new Vector2(x, y));
                animated.Origins.Add(origin);
                animated.Frames.Add(resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/{j}").data as Texture2D);
                if (resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/{j}/delay").resourceType !=
                    ResourceType.Unknown)
                    animated.Delay.Add((int)resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/{j}/delay")
                        .data);
                else
                {
                    animated.Delay.Add(100);
                    Debug.WriteLine("Delay not found, using default of 100");
                }
            }
        }

        // Blend Animation
        if (resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/0/a0").resourceType !=
            ResourceType.Unknown)
        {
            obj.AddComponent(new BlendAnimation());
            var origin = (Vector2)resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/0/origin").data;
            var blend = obj.GetComponent<BlendAnimation>();
            transform.Origin = origin;
            transform.Position = new Vector2(x, y);
            for (int j = 0; j < nodeCount; j++)
            {
                blend.Frames.Add((Texture2D)resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/{j}").data);
                blend.Alpha.Add((byte)(int)resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/{j}/a0").data);
                if (resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/{j}/a1").resourceType !=
                    ResourceType.Unknown)
                    blend.Alpha.Add((byte)(int)resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/{j}/a1").data);
                blend.Delay.Add((int)resource.GetItem($"Map/Obj/{oS}.img/{l0}/{l1}/{l2}/{j}/delay").data);
            }
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
            var portalType = (int)resource.GetItem($"{worldPath}/portal/{i}/pt").data;
            var script = (string)resource.GetItem($"{worldPath}/portal/{i}/script").data;
            var x = (int)resource.GetItem($"{worldPath}/portal/{i}/x").data;
            var y = (int)resource.GetItem($"{worldPath}/portal/{i}/y").data;
            var targetMap = (int)resource.GetItem($"{worldPath}/portal/{i}/tm").data; // The map it leads to
            var targetPortal = (string)resource.GetItem($"{worldPath}/portal/{i}/tn").data; // The portal you end up on
            var portal = CreateEntity($"portal_{name}", "Portal");
            var transform = portal.GetComponent<Transform>();
            portal.AddComponent(new PortalInfo()
            {
                Name = name,
                ScriptName = script,
                TargetMapId = targetMap,
                TargetPortalName = targetPortal,
                PortalId = i
            });

            switch (portalType)
            {
                case 2:
                case 4:
                case 7:
                    portal.GetComponent<PortalInfo>().PortalType = PortalType.Visible;
                    break;
                case 0:
                case 1:
                case 3:
                case 9:
                case 0x0C:
                case 0x0D:
                    portal.GetComponent<PortalInfo>().PortalType = PortalType.Hidden;
                    portal.IsEnabled = false;
                    break;
                case 6: // wtf is this??? In map 100000000 it spawns like 4 random portals
                case 10:
                case 0x0B:
                    portal.GetComponent<PortalInfo>().PortalType = PortalType.ScriptedHidden;
                    portal.IsEnabled = false;
                    break;
                default:
                    Console.WriteLine($"Unknown Portal Type Detected: {portalType}");
                    break;
            }

            portal.Layer = RenderLayer.Foreground;
            transform.Position = new Vector2(x, y);
            portal.AddComponent(new Portal());
            var animation = portal.GetComponent<Portal>();

            // TODO: When physics are complete, add portal hidden and portal scripted hidden
            switch (portal.GetComponent<PortalInfo>().PortalType)
            {
                case PortalType.Visible:
                    for (var j = 0; j < 8; j++)
                    {
                        animation.Frames.Add((Texture2D)resource.GetItem($"Map/MapHelper.img/portal/game/pv/{j}").data);
                        animation.Origins.Add(
                            (Vector2)resource.GetItem($"Map/MapHelper.img/portal/game/pv/{j}/origin").data);
                    }

                    portal.GetComponent<Portal>().IsHidden = false;
                    break;
                case PortalType.Hidden:
                    portal.GetComponent<Portal>().IsHidden = true;
                    DestroyEntity(portal);
                    break;
                case PortalType.ScriptedHidden:
                    portal.GetComponent<Portal>().IsHidden = true;
                    DestroyEntity(portal);
                    break;
                default: // This should never happen.
                    DestroyEntity(portal);
                    Debug.WriteLine("Something went wrong, portal entity destroyed.");
                    break;
            }
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