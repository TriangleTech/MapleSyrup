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
            var layer = NXFactory.Shared.GetChildNode(file, imgNode, i.ToString()) ??
                        throw new NullReferenceException($"Failed to find [{i}] node");
            var obj = NXFactory.Shared.GetChildNode(file, layer, "obj") ??
                          throw new NullReferenceException($"Failed to find [obj] node");
            if (obj.ChildCount == 0) return;
            
            var objNodes = NXFactory.Shared.GetChildren(file, obj);
            foreach (var (_, objNode) in objNodes)
            {
                var nodes = NXFactory.Shared.GetChildren(file, objNode);
                var oS = nodes["oS"].GetString();
                var l0 = nodes["l0"].GetString();
                var l1 = nodes["l1"].GetString();
                var l2 = nodes["l2"].GetString();
                var x = nodes["x"].GetInt();
                var y = nodes["y"].GetInt();
                var z = nodes["z"].GetInt();
                // TODO: Eventually find out what these do. Commented out to save CPU processing.
                //var zM = NXFactory.Shared.GetChildNode(file, objNode, "zM")?.GetInt() ??
                //         throw new NullReferenceException("Failed to find [zM] node");
                //var f = NXFactory.Shared.GetChildNode(file, objNode, "f")?.GetInt() ??
                //        throw new NullReferenceException("Failed to find [f] node"); // pretty sure this means flipped.
                var order = 30000 * i + z - 1073739824;
                
                var objSet = NXFactory.Shared.GetNode(MapleFiles.Map, $"Obj/{oS}.img/{l0}/{l1}/{l2}") ??
                             throw new NullReferenceException($"Failed to find [Obj/{oS}.img/{l0}/{l1}/{l2}] node");
                var frameNodes = NXFactory.Shared.GetChildren(MapleFiles.Map, objSet);
                if (frameNodes.TryGetValue("seat", out _)) continue; // TODO: Seats
                if (frameNodes.TryGetValue("blend", out _)) continue; // TODO: Blend animation
                if (frameNodes.TryGetValue("obstacle", out _)) continue; // TODO: Obstacles
                if (frameNodes.TryGetValue("damage", out _)) continue; // TODO: Trap damage.

                if (frameNodes.Count > 1)
                {
                    var frames = new List<string>(objSet.ChildCount);
                    foreach(var (_, node) in frameNodes)
                    {
                        var origin = NXFactory.Shared.GetChildNode(MapleFiles.Map, node, "origin")?.GetVector() ??
                                     Vector2.Zero;
                        if (!ResourceFactory.Shared.HasResource(node.NodePath))
                        {
                            var texture = node.GetTexture();
                            ResourceFactory.Shared.RegisterResource(new TextureResource(node.NodePath)
                            {
                                Texture = texture,
                                Origin = origin,
                                Delay = NXFactory.Shared.GetChildNode(MapleFiles.Map, node, "delay")?.GetInt() ??
                                        150f
                            });
                        }

                        frames.Add(node.NodePath);
                    }

                    var objEntity = EntityFactory.Shared.CreateEntity($"Obj", "Obj_Animated");
                    var transform = EntityFactory.Shared.GetComponent<Transform>(objEntity.Id);
                    objEntity.Layer = i;
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
                    var node = frameNodes["0"];
                    var origin = NXFactory.Shared.GetChildNode(MapleFiles.Map, node, "origin")?.GetVector() ??
                                 Vector2.Zero;
                    var objEntity = EntityFactory.Shared.CreateEntity($"Obj", "Obj_Static");
                    var transform = EntityFactory.Shared.GetComponent<Transform>(objEntity.Id);

                    if (!ResourceFactory.Shared.HasResource(node.NodePath))
                    {
                        ResourceFactory.Shared.RegisterResource(new TextureResource(node.NodePath)
                        {
                            Texture = node.GetTexture(),
                            Origin = origin,
                            Delay = 0f,
                        });
                    }

                    objEntity.Layer = i;
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
            var info = NXFactory.Shared.GetChildNode(file, layerNode, "info") ??
                           throw new NullReferenceException("Failed to find [info] node");
            var infoNodes = NXFactory.Shared.GetChildren(file, info);
            
            var tS = "";
            if (infoNodes.TryGetValue("tS", out var tSNode))
            {
                tS = tSNode.GetString();
            }
            else
            {
                tS = NXFactory.Shared.GetNode(file, $"{imgNode.NodePath}/0/info/tS")?.GetString() ??
                     "grassySoil";
            }

            Console.WriteLine($"Loading tiles for layer {i}");
            
            var tileNodes = NXFactory.Shared.GetChildren(file, tileLayer);
            foreach (var (_, tileNode) in tileNodes)
            {
                var tile = NXFactory.Shared.GetChildren(file, tileNode);
                var x = tile["x"].GetInt();
                var y = tile["y"].GetInt();
                var zM = tile["zM"].GetInt();
                var u = tile["u"].GetString();
                var no = tile["no"].GetInt();
                
                var tileSet = NXFactory.Shared.GetNode(MapleFiles.Map, $"Tile/{tS}.img/{u}/{no}") ??
                           throw new NullReferenceException("Failed to find [tile] node");
                var setNodes = NXFactory.Shared.GetChildren(MapleFiles.Map, tileSet);
                var origin = setNodes["origin"].GetVector();
                var z = setNodes["z"].GetInt();
                var order = z + 10 * (3000 * (int)(i) - zM) - 1073721834;
                
                var tileEntity = EntityFactory.Shared.CreateEntity("Tile", u);
                var transform = EntityFactory.Shared.GetComponent<Transform>(tileEntity.Id);
                
                if (!ResourceFactory.Shared.HasResource(tileSet.NodePath))
                {
                    ResourceFactory.Shared.RegisterResource(new TextureResource(tileSet.NodePath)
                    {
                        Texture = tileSet.GetTexture(),
                        Origin = origin,
                        Delay = 0f
                    });
                }

                tileEntity.Layer = i;
                transform.Position = new Vector2(x, y);
                transform.Origin = origin;
                transform.Z = order;

                var mapObjComponent = new MapObj()
                {
                    Owner = tileEntity.Id,
                    Textures = [tileSet.NodePath],
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