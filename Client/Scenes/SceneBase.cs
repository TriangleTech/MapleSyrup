using System.Numerics;
using System.Runtime.CompilerServices;
using Client.ECS;
using Client.ECS.Components;
using Client.ECS.Components.Common;
using Client.ECS.Components.Map;
using Client.ECS.Systems;
using Client.Nx;
using Client.Resources;
using Client.Windowing;
using ZeroElectric.Vinculum;
using Transform = Client.ECS.Components.Common.Transform;

namespace Client.Scenes;

public abstract class SceneBase
{
    protected readonly List<IDrawSystem> DrawSystems;
    protected readonly List<IUpdateSystem> UpdateSystems;
    protected readonly string SceneName;
    public Camera2D Camera;

    public SceneBase(string sceneName)
    {
        DrawSystems = new List<IDrawSystem>(5);
        UpdateSystems = new List<IUpdateSystem>(5);
        SceneName = sceneName;
        Camera = new Camera2D()
        {
            offset = Vector2.Zero,
            rotation = 0f,
            target = new Vector2(0f, 0f),
            zoom = 1f
        };
    }

    public abstract void InitSystems();
    public abstract void LoadContent();

    #region Load Background

    protected void LoadBackground(MapleFiles file, NXNode imgNode)
    {
        Console.WriteLine("Loading background...");
        try
        {
            var back = NXFactory.Shared.GetChildNode(file, imgNode, "back") ??
                       throw new NullReferenceException("Failed to find [back] node in img file");
            for (var i = 0; i < back.ChildCount; i++)
            {
                var node = NXFactory.Shared.GetChildNode(file, back, i.ToString()) ?? throw new NullReferenceException($"Failed to find [{i}] node");
                var bS = NXFactory.Shared.GetChildNode(file, node, "bS")?.GetString() ??
                         throw new NullReferenceException("Failed to find [bS] node");
                var no = NXFactory.Shared.GetChildNode(file, node, "no")?.GetInt() ??
                         throw new NullReferenceException("Failed to find [no] node");
                var x = NXFactory.Shared.GetChildNode(file, node, "x")?.GetInt() ??
                        throw new NullReferenceException("Failed to find [x] node");
                var y = NXFactory.Shared.GetChildNode(file, node, "y")?.GetInt() ??
                        throw new NullReferenceException("Failed to find [y] node");
                var rx = NXFactory.Shared.GetChildNode(file, node, "rx")?.GetInt() ??
                         throw new NullReferenceException("Failed to find [rx] node");
                var ry = NXFactory.Shared.GetChildNode(file, node, "ry")?.GetInt() ??
                         throw new NullReferenceException("Failed to find [ry] node");
                var cx = NXFactory.Shared.GetChildNode(file, node, "cx")?.GetInt() ??
                         throw new NullReferenceException("Failed to find [cx] node");
                var cy = NXFactory.Shared.GetChildNode(file, node, "cy")?.GetInt() ??
                         throw new NullReferenceException("Failed to find [cy] node");
                var a = NXFactory.Shared.GetChildNode(file, node, "a")?.GetInt() ??
                        throw new NullReferenceException("Failed to find [a] node");
                //var front = NXFactory.Shared.GetChildNode(file, node, "front")?.GetInt() ??
                //            throw new NullReferenceException("Failed to find [front] node");
                var ani = NXFactory.Shared.GetChildNode(file, node, "ani")?.GetInt() ??
                          throw new NullReferenceException("Failed to find [ani] node");
                //var f = NXFactory.Shared.GetChildNode(file, node, "f")?.GetInt() ??
                //        throw new NullReferenceException("Failed to find [f] node");
                var type = NXFactory.Shared.GetChildNode(file, node, "type")?.GetInt() ??
                           throw new NullReferenceException("Failed to find [type] node");

                if (ani == 1)
                {
                    var backgroundSet = NXFactory.Shared.GetNode(MapleFiles.Map, $"Back/{bS}.img/ani") ?? throw new NullReferenceException("Failed to find background set");
                    var animatedNode = NXFactory.Shared.GetChildNode(MapleFiles.Map, backgroundSet, no.ToString()) ?? throw new NullReferenceException("Failed to find [no] node");
                    var frames = new List<string>();
                    
                    for (var j = 0; j < animatedNode.ChildCount; j++)
                    {
                        var texture = NXFactory.Shared.GetChildNode(MapleFiles.Map, animatedNode, j.ToString()) ?? 
                                      throw new NullReferenceException("Failed to find [texture] node");
                        var origin = NXFactory.Shared.GetChildNode(MapleFiles.Map, texture, "origin")?.GetVector() 
                                     ?? throw new NullReferenceException("Failed to find [origin] node");
                        var delay = NXFactory.Shared.GetChildNode(MapleFiles.Map, texture, "delay")?.GetInt() ??
                                    150f;
                        ResourceFactory.Shared.RegisterResource(new TextureResource(texture.NodePath)
                            {
                                Texture = texture.GetTexture(),
                                Origin = origin,
                                Delay = delay,
                            });
                        frames.Add(texture.NodePath);
                    }

                    // TODO: let's find a way to not call this twice...
                    var backgroundEntity = EntityFactory.Shared.CreateEntity($"Background_{i}", "Background");
                    backgroundEntity.Layer = -1;
                    var transform = EntityFactory.Shared.GetComponent<Transform>(backgroundEntity.Id);
                    transform.Position = new Vector2(x, y);
                    transform.Origin = Vector2.Zero;
                    transform.Z = 0;

                    var backgroundComponent = new BackgroundObj
                    {
                        Owner = backgroundEntity.Id,
                        Textures = frames,
                        Type = type,
                        Alpha = a,
                        Cx = cx,
                        Cy = cy,
                        Rx = rx,
                        Ry = ry,
                        Animated = frames.Count > 1,
                    };

                    EntityFactory.Shared.AddComponent(backgroundComponent);
                }
                else
                {
                    var backgroundSet = NXFactory.Shared.GetNode(MapleFiles.Map, $"Back/{bS}.img/back") ??
                                        throw new NullReferenceException("Failed to find background set");
                    var backgroundItem = NXFactory.Shared.GetChildNode(MapleFiles.Map, backgroundSet, no.ToString()) ??
                                         throw new NullReferenceException("Failed to find [no] node");
                    var origin =
                        NXFactory.Shared.GetChildNode(MapleFiles.Map, backgroundItem, "origin")?.GetVector() ??
                        throw new NullReferenceException("Failed to find [origin] node");
                    
                    var backgroundEntity = EntityFactory.Shared.CreateEntity($"Background_{i}", "Background");
                    backgroundEntity.Layer = -1;
                    
                    var transform = EntityFactory.Shared.GetComponent<Transform>(backgroundEntity.Id);
                    ResourceFactory.Shared.RegisterResource(new TextureResource(backgroundItem.NodePath) 
                        {
                            Texture = backgroundItem.GetTexture(),
                            Origin = origin,
                            Delay = 0f,
                        });
                    transform.Position = new Vector2(x, y);
                    transform.Origin = origin;
                    transform.Z = 0;

                    var backgroundComponent = new BackgroundObj
                    {
                        Owner = backgroundEntity.Id,
                        Textures = [backgroundItem.NodePath],
                        Type = type,
                        Alpha = a,
                        Cx = cx,
                        Cy = cy,
                        Rx = rx,
                        Ry = ry,
                        Animated = false,
                    };

                    EntityFactory.Shared.AddComponent(backgroundComponent);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    #endregion

    #region Load Objects

    protected void LoadObjects(MapleFiles file, NXNode imgNode, int i)
    {
        try
        {
            var layerNode = NXFactory.Shared.GetChildNode(file, imgNode, i.ToString()) ??
                            throw new NullReferenceException($"Failed to find [{i}] node");
            var objLayer = NXFactory.Shared.GetChildNode(file, layerNode, "obj") ??
                           throw new NullReferenceException("Failed to find [obj] node");

            if (objLayer.ChildCount == 0) return;

            Console.WriteLine($"Loading objects for layer {i}");
            for (var j = 0; j < objLayer.ChildCount; j++)
            {
                // These reference the file they come from, not the location of the item that is referenced.
                var objNode = NXFactory.Shared.GetChildNode(file, objLayer, j.ToString()) ??
                              throw new NullReferenceException("Failed to find [tile] node");
                var oS = NXFactory.Shared.GetChildNode(file, objNode, "oS")?.GetString() ??
                         throw new NullReferenceException("Failed to find [oS] node");
                var l0 = NXFactory.Shared.GetChildNode(file, objNode, "l0")?.GetString() ??
                         throw new NullReferenceException("Failed to find [l0] node");
                var l1 = NXFactory.Shared.GetChildNode(file, objNode, "l1")?.GetString() ??
                         throw new NullReferenceException("Failed to find [l1] node");
                var l2 = NXFactory.Shared.GetChildNode(file, objNode, "l2")?.GetString() ??
                         throw new NullReferenceException("Failed to find [l2] node");
                var x = NXFactory.Shared.GetChildNode(file, objNode, "x")?.GetInt() ??
                        throw new NullReferenceException("Failed to find [x] node");
                var y = NXFactory.Shared.GetChildNode(file, objNode, "y")?.GetInt() ??
                        throw new NullReferenceException("Failed to find [y] node");
                var z = NXFactory.Shared.GetChildNode(file, objNode, "z")?.GetInt() ??
                        throw new NullReferenceException("Failed to find [z] node");
                // TODO: Eventually find out what these do. Commented out to save CPU processing.
                //var zM = NXFactory.Shared.GetChildNode(file, objNode, "zM")?.GetInt() ??
                //         throw new NullReferenceException("Failed to find [zM] node");
                //var f = NXFactory.Shared.GetChildNode(file, objNode, "f")?.GetInt() ??
                //        throw new NullReferenceException("Failed to find [f] node"); // pretty sure this means flipped.
                var order = 30000 * i + z - 1073739824;

                var objSet = NXFactory.Shared.GetNode(MapleFiles.Map, $"Obj/{oS}.img/{l0}/{l1}/{l2}") ??
                             throw new NullReferenceException($"Failed to find [Obj/{oS}.img/{l0}/{l1}/{l2}] node");

                if (NXFactory.Shared.HasNode(MapleFiles.Map, objSet, "seat")) continue; // TODO: Seats
                if (NXFactory.Shared.HasNode(MapleFiles.Map, objSet, "blend")) continue; // TODO: Blend animation
                if (NXFactory.Shared.HasNode(MapleFiles.Map, objSet, "obstacle")) continue; // TODO: Obstacles
                if (NXFactory.Shared.HasNode(MapleFiles.Map, objSet, "damage")) continue; // TODO: Trap damage.

                if (objSet.ChildCount > 1)
                {
                    var frames = new List<string>(objSet.ChildCount);
                    for (var k = 0; k < objSet.ChildCount; k++)
                    {
                        var node = NXFactory.Shared.GetChildNode(MapleFiles.Map, objSet, k.ToString()) ??
                                   throw new NullReferenceException($"Failed to find [{objSet.NodePath}/{k}] node");
                        var origin = NXFactory.Shared.GetChildNode(MapleFiles.Map, node, "origin")?.GetVector() ??
                                     throw new NullReferenceException("Failed to find [origin] node");
                        var texture = node.GetTexture();
                        ResourceFactory.Shared.RegisterResource(new TextureResource(node.NodePath)
                        {
                            Texture = texture,
                            Origin = origin,
                            Delay = NXFactory.Shared.GetChildNode(MapleFiles.Map, node, "delay")?.GetInt() ??
                                    150f
                        });
                        frames.Add(node.NodePath);
                    }

                    var objEntity = EntityFactory.Shared.CreateEntity($"Obj_{i}_{j}", "Obj_Animated");
                    objEntity.Layer = i;
                    
                    var transform = EntityFactory.Shared.GetComponent<Transform>(objEntity.Id);
                    transform.Position = new Vector2(x, y);
                    transform.Z = order;

                    var animation = new MapObj
                    {
                        Owner = objEntity.Id,
                        Blend = false,
                        Frame = Random.Shared.Next(0, frames.Count),
                        FrameDelay = 0f,
                        Loop = true,
                        Textures = frames
                    };

                    EntityFactory.Shared.AddComponent(animation);
                }
                else
                {
                    var node = NXFactory.Shared.GetChildNode(MapleFiles.Map, objSet, "0") ??
                               throw new NullReferenceException("Failed to find [0] node");
                    var origin = NXFactory.Shared.GetChildNode(MapleFiles.Map, node, "origin")?.GetVector() ??
                                 throw new NullReferenceException("Failed to find [origin] node");
                    var objEntity = EntityFactory.Shared.CreateEntity($"Obj_{i}_{j}", "Obj_Static");
                    objEntity.Layer = i;
                    
                    var transform = EntityFactory.Shared.GetComponent<Transform>(objEntity.Id);
                    ResourceFactory.Shared.RegisterResource(new TextureResource(node.NodePath)
                    {
                        Texture = node.GetTexture(),
                        Origin = origin,
                        Delay = 0f,
                    });
                    transform.Position = new Vector2(x, y);
                    transform.Origin = origin;
                    transform.Z = order;

                    var mapObj = new MapObj()
                    {
                        Owner = objEntity.Id,
                        Textures = [node.NodePath],
                        Blend = false,
                        Loop = false,
                        Frame = 0,
                        FrameDelay = 0f,
                    };

                    EntityFactory.Shared.AddComponent(mapObj);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    #endregion

    #region Load Tiles

    protected void LoadTiles(MapleFiles file, NXNode imgNode, int i)
    {
        try
        {
            var layerNode = NXFactory.Shared.GetChildNode(file, imgNode, i.ToString()) ??
                            throw new NullReferenceException("Failed to find [layer] node");
            var tileLayer = NXFactory.Shared.GetChildNode(file, layerNode, "tile") ??
                            throw new NullReferenceException("Failed to find [tile] node");

            if (tileLayer.ChildCount == 0) return;
            var infoNode = NXFactory.Shared.GetChildNode(file, layerNode, "info") ??
                           throw new NullReferenceException("Failed to find [info] node");
            var tS = "";
            if (NXFactory.Shared.HasNode(file, infoNode, "tS"))
            {
                tS = NXFactory.Shared.GetChildNode(file, infoNode, "tS")?.GetString() ??
                     throw new NullReferenceException("Failed to find [tS] node");
            }
            else
            {
                tS = NXFactory.Shared.GetNode(file, $"{imgNode.NodePath}/0/info/tS")?.GetString() ??
                     throw new NullReferenceException("Failed to find [tS] node");
            }

            Console.WriteLine($"Loading tiles for layer {i}");
            for (var j = 0; j < tileLayer.ChildCount; j++)
            {
                var tileNode = NXFactory.Shared.GetChildNode(file, tileLayer, j.ToString()) ??
                               throw new NullReferenceException("Failed to find [tile] node");
                var x = NXFactory.Shared.GetChildNode(file, tileNode, "x")?.GetInt() ??
                        throw new NullReferenceException("Failed to find [x] node");
                var y = NXFactory.Shared.GetChildNode(file, tileNode, "y")?.GetInt() ??
                        throw new NullReferenceException("Failed to find [y] node");
                var zM = NXFactory.Shared.GetChildNode(file, tileNode, "zM")?.GetInt() ??
                         throw new NullReferenceException("Failed to find [zM] node");
                var u = NXFactory.Shared.GetChildNode(file, tileNode, "u")?.GetString() ??
                        throw new NullReferenceException("Failed to find [u] node");
                var no = NXFactory.Shared.GetChildNode(file, tileNode, "no")?.GetInt() ??
                         throw new NullReferenceException("Failed to find [no] node");

                var tile = NXFactory.Shared.GetNode(MapleFiles.Map, $"Tile/{tS}.img/{u}/{no}") ??
                           throw new NullReferenceException("Failed to find [tile] node");
                var origin = NXFactory.Shared.GetChildNode(MapleFiles.Map, tile, "origin")?.GetVector() ??
                             throw new NullReferenceException("Failed to find [origin] node");
                var z = NXFactory.Shared.GetChildNode(MapleFiles.Map, tile, "z")?.GetInt() ??
                        throw new NullReferenceException("Failed to find [z] node");
                var order = z + 10 * (3000 * (int)(i) - zM) - 1073721834;

                var tileEntity = EntityFactory.Shared.CreateEntity($"Tile_{i}_{j}", u);
                tileEntity.Layer = i;
                
                var transform = EntityFactory.Shared.GetComponent<Transform>(tileEntity.Id);
                ResourceFactory.Shared.RegisterResource(new TextureResource(tile.NodePath)
                {
                    Texture = tile.GetTexture(),
                    Origin = origin,
                    Delay = 0f
                });
                
                transform.Position = new Vector2(x, y);
                transform.Origin = origin;
                transform.Z = order;

                var mapObjComponent = new MapObj()
                {
                    Owner = tileEntity.Id,
                    Textures = [tile.NodePath],
                    Blend = false,
                    Loop = false,
                    Frame = 0,
                    FrameDelay = 0f,
                };
                EntityFactory.Shared.AddComponent(mapObjComponent);

                if (u != "enH0") continue;
                var collision = new LineCollision()
                {
                    Owner = tileEntity.Id,
                    Start = (transform.Position - transform.Origin) + new Vector2(-10, 10),
                    End = (transform.Position - transform.Origin) + new Vector2(110, 10),
                };

                EntityFactory.Shared.AddComponent(collision);

            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    #endregion

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

    public void Shutdown()
    {
        DrawSystems.Clear();
        UpdateSystems.Clear();
    }
}