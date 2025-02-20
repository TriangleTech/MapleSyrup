using System.Numerics;
using MapleSyrup.ECS;
using MapleSyrup.ECS.Components.Map;
using MapleSyrup.ECS.Systems;
using MapleSyrup.Common.Map;
using MapleSyrup.Nx;
using MapleSyrup.NX;
using MapleSyrup.Resources;
using ZeroElectric.Vinculum;
using Transform = MapleSyrup.ECS.Components.Common.Transform;

namespace MapleSyrup.Scenes;

public abstract class SceneBase
{
    protected readonly List<IDrawSystem> DrawSystems;
    protected readonly List<IUpdateSystem> UpdateSystems;
    public Camera2D Camera;

    public SceneBase()
    {
        DrawSystems = new List<IDrawSystem>(5);
        UpdateSystems = new List<IUpdateSystem>(5);
    }

    public abstract void InitSystems();

    public virtual void LoadContent(MapleMap map)
    {
        Task.Run(() => LoadBackground(map));
        Task.Run(() => LoadObjects(map));
        Task.Run(() => LoadTiles(map));
    }

    #region Load Background

    private void LoadBackground(MapleMap map)
    {
        try
        {
            foreach (var background in map.Backgrounds)
            {
                var entity = EntityFactory.Shared.CreateEntity(-1, background.NodePath, "Background");
                var transform = EntityFactory.Shared.GetComponent<Transform>(entity.Id);
                transform.Position = new Vector2(background.X, background.Y);
                transform.Origin = Vector2.Zero;
                transform.Z = 0;

                var animated = background.NodePath.Contains("ani");
                if (animated)
                {
                    var animatedNode = NXFactory.Shared.GetNode(MapleFile.Map, background.NodePath)
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
                            ResourceFactory.Shared.RegisterResource(new TextureResource(animation.NodePath)
                            {
                                MainFile = MapleFile.Map,
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
                    var backgroundItem = NXFactory.Shared.GetNode(MapleFile.Map, background.NodePath) ??
                                        throw new NullReferenceException("Failed to find background");
                    var origin =
                        NXFactory.Shared.GetChildNode(MapleFile.Map, backgroundItem, "origin")?.GetVector() ??
                        throw new NullReferenceException("Failed to find [origin] node");
                    transform.Origin = origin;
                    
                    if (!ResourceFactory.Shared.HasResource(backgroundItem.NodePath))
                    {
                        ResourceFactory.Shared.RegisterResource(new TextureResource(backgroundItem.NodePath)
                        {
                            MainFile = MapleFile.Map,
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
                var transform = EntityFactory.Shared.GetComponent<Transform>(entity.Id);
                transform.Position = new Vector2(mapObject.X, mapObject.Y);
                transform.Origin = Vector2.Zero;
                transform.Z = mapObject.Z;

                var item = NXFactory.Shared.GetNode(MapleFile.Map, mapObject.NodePath)??
                           throw new NullReferenceException();
                var children = item.GetChildren();
                
                if (item.Type == NodeType.Bitmap)
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
                    
                    if (ResourceFactory.Shared.HasResource(item.NodePath)) continue;
                    ResourceFactory.Shared.RegisterResource(new TextureResource(item.NodePath)
                    {
                        MainFile = MapleFile.Map,
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
                        
                        if (ResourceFactory.Shared.HasResource(animation.NodePath)) continue;
                        var delay = nodes.TryGetValue("delay", out _) ? nodes["delay"].GetInt() : 150f;
                        ResourceFactory.Shared.RegisterResource(new TextureResource(animation.NodePath)
                        {
                            MainFile = MapleFile.Map,
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
                var transform = EntityFactory.Shared.GetComponent<Transform>(entity.Id);
                transform.Position = new Vector2(tile.X, tile.Y);
                transform.Z = tile.Z;
                
                var tileNode = NXFactory.Shared.GetNode(MapleFile.Map, tile.NodePath) ??
                               throw new NullReferenceException($"Failed to find [{tile.NodePath}]");
                var origin = NXFactory.Shared.GetChildNode(MapleFile.Map, tileNode, "origin")?.GetVector() ??
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
                ResourceFactory.Shared.RegisterResource(new TextureResource(tile.NodePath)
                {
                    MainFile = MapleFile.Map,
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

    #region Draw/Update
    
    public void Draw()
    {
        foreach (var system in DrawSystems)
        {
            system.Draw(EntityFactory.Shared, ResourceFactory.Shared);
        }
    }

    public void Update(float timeDelta)
    {
        foreach (var system in UpdateSystems)
        {
            system.Update(EntityFactory.Shared, ResourceFactory.Shared, timeDelta);
        }
        
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