using System.Numerics;
using MapleSyrup.Common.Map;
using MapleSyrup.ECS;
using MapleSyrup.ECS.Components.Common;
using MapleSyrup.ECS.Interfaces;
using MapleSyrup.NX;
using MapleSyrup.Physics.Components;
using MapleSyrup.Physics.Resolvers;
using MapleSyrup.Resources;
using MapleSyrup.Tests.Systems;
using MapleSyrup.World.Components;
using MapleSyrup.World.Systems;
using ZeroElectric.Vinculum;

namespace MapleSyrup.World;

public abstract class WorldBase
{
    protected readonly List<IDrawSystem> DrawSystems;
    protected readonly List<IUpdateSystem> UpdateSystems;
    private readonly FootholdResolver _footholdResolver;
    private readonly GravityResolver _gravityResolver;
    public Camera2D Camera;

    public WorldBase()
    {
        DrawSystems = new List<IDrawSystem>(5);
        UpdateSystems = new List<IUpdateSystem>(5);
        _footholdResolver = new FootholdResolver();
        _gravityResolver = new GravityResolver();
    }

    public virtual void InitSystems()
    {
        var background = new BackgroundAnimation();
        var mapObj = new MapObjAnimation();
        var footholdResolver = new FootholdResolver();
        var squareTest = new RedSquareTest();
        var gravityResolver = new GravityResolver();
        
        // Add any draw systems here
        DrawSystems.Add(background);
        DrawSystems.Add(mapObj);
        DrawSystems.Add(squareTest);
        
        // Add any update systems here
        UpdateSystems.Add(background);
        UpdateSystems.Add(mapObj);
        UpdateSystems.Add(squareTest);
    }

    public virtual void LoadContent(MapleMap map)
    {
        LoadBackground(map);
        LoadObjects(map);
        LoadTiles(map);
        LoadFootholds(map);
    }

    #region Load Background

    private void LoadBackground(MapleMap map)
    {
        try
        {
            foreach (var background in map.Backgrounds)
            {
                var entity = EntityFactory.Shared.CreateEntity(-1, background.NodePath, "Background");
                var transform = EntityFactory.Shared.GetComponent<TransformComponent>(entity.Id);
                transform.Position = new Vector2(background.X, background.Y);
                transform.Origin = Vector2.Zero;
                transform.Z = 0;

                var animated = background.NodePath.Contains("ani");
                if (animated)
                {
                    var animatedNode = ResourceFactory.Shared.GetNode("Map", background.NodePath)
                        ?? throw new NullReferenceException("Failed to find background animated node");
                    var nodes = animatedNode.GetChildren();
                    var frames = new List<string>();
                    var alpha = new Queue<int>();
                    var blend = false;
                    
                    foreach (var (_, animation) in nodes)
                    {
                        var children = animatedNode.GetChildren();
                        var origin = children.TryGetValue("origin", out _) ? children["origin"].GetVector() : Vector2.Zero;
                        var delay = children.TryGetValue("delay", out _) ? children["delay"].GetInt() : 150f;
                        blend = animation.HasNode("a0");
                        if (blend)
                        {
                            var a0 = children.TryGetValue("a0", out _) ? children["a0"].GetInt() : 0;
                            var a1 = children.TryGetValue("a1", out _) ? children["a1"].GetInt() : 255;
                            alpha.Enqueue(a0);
                            alpha.Enqueue(a1);
                        }
                        if (!ResourceFactory.Shared.HasResource(animation.NodePath) && animation.Type == NodeType.Bitmap)
                        {
                            ResourceFactory.Shared.LoadResource(new TextureResource("Map", animation.NodePath)
                            {
                                Origin = origin,
                                Delay = delay,
                            });
                        }
                        
                        if (animation.Type == NodeType.Bitmap) 
                            frames.Add(animation.NodePath);
                    }
                    
                    var backComp = new BackgroundObj()
                    {
                        Owner = entity.Id,
                        Textures = frames,
                        Type = background.BackgroundType,
                        Blend = blend,
                        Cx = background.Cx,
                        Cy = background.Cy,
                        Rx = background.Rx,
                        Ry = background.Ry,
                        Animated = !blend,
                    };
                    
                    if (blend)
                    {
                        backComp.Alpha0 = alpha.Dequeue();
                        backComp.Alpha1 = alpha.Dequeue();
                    }

                    EntityFactory.Shared.AddComponent(backComp);
                }
                else
                {
                    var backgroundItem = ResourceFactory.Shared.GetNode("Map", background.NodePath) ??
                                        throw new NullReferenceException("Failed to find background");
                    var origin =
                        ResourceFactory.Shared.GetChildNode("Map", backgroundItem, "origin")?.GetVector() ??
                        throw new NullReferenceException("Failed to find [origin] node");
                    transform.Origin = origin;
                    
                    if (!ResourceFactory.Shared.HasResource(backgroundItem.NodePath))
                    {
                        ResourceFactory.Shared.LoadResource(new TextureResource("Map", backgroundItem.NodePath)
                        {
                            Origin = origin,
                            Delay = 0f,
                        });
                    }

                    var backgroundComponent = new BackgroundObj
                    {
                        Owner = entity.Id,
                        Textures = [background.NodePath],
                        Type = background.BackgroundType,
                        Cx = background.Cx,
                        Cy = background.Cy,
                        Rx = background.Rx,
                        Ry = background.Ry,
                        Animated = false,
                    };

                    EntityFactory.Shared.AddComponent(backgroundComponent);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    #endregion

    #region Load Objects

    private void LoadObjects(MapleMap map)
    {
        try
        {
            foreach (var mapObject in map.Objects)
            {
                var entity = EntityFactory.Shared.CreateEntity(mapObject.Layer, mapObject.NodePath, "MapObject");
                var transform = EntityFactory.Shared.GetComponent<TransformComponent>(entity.Id);
                transform.Position = new Vector2(mapObject.X, mapObject.Y);
                transform.Origin = Vector2.Zero;
                transform.Z = mapObject.Z;

                var objectNode = ResourceFactory.Shared.GetNode("Map", mapObject.NodePath)??
                           throw new NullReferenceException();
                var children = objectNode.GetChildren();
                
                if (objectNode.Type == NodeType.Bitmap)
                {
                    var origin = children.TryGetValue("origin", out _) ? children["origin"].GetVector() : Vector2.Zero;
                    transform.Origin = origin;
                    
                    var mapComponent = new MapObj()
                    {
                        Owner = entity.Id,
                        Textures = [mapObject.NodePath],
                        Alpha = 255,
                        Blend = false,
                        Loop = false,
                        Frame = 0,
                        FrameDelay = 0f,
                    };
                    EntityFactory.Shared.AddComponent(mapComponent);
                    
                    if (ResourceFactory.Shared.HasResource(objectNode.NodePath)) continue;
                    ResourceFactory.Shared.LoadResource(new TextureResource("Map", objectNode.NodePath)
                    {
                        Origin = origin,
                        Delay = 0f,
                    });
                }
                else
                {
                    var frames = new List<string>();
                    var alpha = new Queue<int>();
                    var blend = false;

                    foreach (var (_, animation) in children)
                    {
                        if (animation.Type != NodeType.Bitmap) continue; // TODO: Handle cases such as "blend", "obstacle", etc. exist
                        var nodes = animation.GetChildren();
                        var origin = nodes.TryGetValue("origin", out _) ? nodes["origin"].GetVector() : Vector2.Zero;
                        blend = animation.HasNode("a0");
                        if (blend)
                        {
                            var a0 = nodes.TryGetValue("a0", out _) ? nodes["a0"].GetInt() : 0;
                            var a1 = nodes.TryGetValue("a1", out _) ? nodes["a1"].GetInt() : 0;
                            alpha.Enqueue(a0);
                            alpha.Enqueue(a1);
                        }
                        frames.Add(animation.NodePath);
                        
                        if (ResourceFactory.Shared.HasResource(animation.NodePath)) 
                            continue;
                        var delay = nodes.TryGetValue("delay", out _) ? nodes["delay"].GetInt() : 150f;
                        ResourceFactory.Shared.LoadResource(new TextureResource("Map", animation.NodePath)
                        {
                            Origin = origin,
                            Delay = delay,
                        });
                    }

                    var mapComponent = new MapObj()
                    {
                        Owner = entity.Id,
                        Textures = frames,
                        Alpha = 255,
                        Blend = blend,
                        Loop = !blend,
                        Frame = 0,
                    };

                    if (blend)
                    {
                        mapComponent.Alpha0 = alpha.Dequeue();
                        mapComponent.Alpha1 = alpha.Dequeue();
                    }
                    
                    EntityFactory.Shared.AddComponent(mapComponent);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    #endregion

    #region Load Tiles

    private void LoadTiles(MapleMap map)
    {
        try
        {
            foreach (var tile in map.Tiles)
            {
                var entity = EntityFactory.Shared.CreateEntity(tile.Layer, tile.NodePath, "MapTile");
                var transform = EntityFactory.Shared.GetComponent<TransformComponent>(entity.Id);
                transform.Position = new Vector2(tile.X, tile.Y);
                transform.Z = tile.Z;
                
                var tileNode = ResourceFactory.Shared.GetNode("Map", tile.NodePath) ??
                               throw new NullReferenceException($"Failed to find [{tile.NodePath}]");
                var origin = ResourceFactory.Shared.GetChildNode("Map", tileNode, "origin")?.GetVector() ??
                             throw new NullReferenceException("Failed to find [origin] node");
                transform.Origin = origin;

                var tileComponent = new MapObj()
                {
                    Owner = entity.Id,
                    Textures = [tile.NodePath],
                    Alpha = 255,
                    Blend = false,
                    Loop = false,
                    Frame = 0,
                    FrameDelay = 0f,
                };
                
                EntityFactory.Shared.AddComponent(tileComponent);
                if (ResourceFactory.Shared.HasResource(tile.NodePath)) continue;
                ResourceFactory.Shared.LoadResource(new TextureResource("Map", tile.NodePath)
                {
                    Origin = origin,
                    Delay = 0f,
                });
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    #endregion
    
    #region Load Footholds

    private void LoadFootholds(MapleMap map)
    {
        foreach (var foothold in map.Footholds)
        {
            var entity = EntityFactory.Shared.CreateEntity(foothold.Layer, "MapFoothold", "MapFoothold");
            entity.Layer = foothold.Layer;
            var collision = new LineCollision()
            {
                Owner = entity.Id,
                Layer = foothold.Layer,
                Start = new Vector2(foothold.X1, foothold.Y1),
                End = new Vector2(foothold.X2, foothold.Y2),
                Bounds = new Rectangle(foothold.X1, foothold.Y1, foothold.X2 - foothold.X1, foothold.Y2 - foothold.Y1),
            };
            
            EntityFactory.Shared.AddComponent(collision);
        }
    }
    
    #endregion

    #region Draw/Update
    
    public void Draw()
    {
        var entities = EntityFactory.Shared.Entities;
        foreach (var entity in entities)
        {
            foreach (var system in DrawSystems)
                system.Draw(entity);
        }
    }

    public void Update(float timeDelta)
    {
        var entities = EntityFactory.Shared.Entities;
        foreach (var entity in entities)
        {
            foreach (var system in UpdateSystems)
                system.Update(entity, timeDelta);
        }
        
        _gravityResolver.Update(timeDelta);
        _footholdResolver.Update(timeDelta);
        
        EntityFactory.Shared.Sort();

        if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT))
            Camera.offset.X += 2f * timeDelta;
        else if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT))
            Camera.offset.X -= 2f * timeDelta;
        else if (Raylib.IsKeyDown(KeyboardKey.KEY_UP))
            Camera.offset.Y += 2f * timeDelta;
        else if (Raylib.IsKeyDown(KeyboardKey.KEY_DOWN))
            Camera.offset.Y -= 2f * timeDelta;
        
    }
    
    #endregion

    public void Shutdown()
    {
        DrawSystems.Clear();
        UpdateSystems.Clear();
    }
}