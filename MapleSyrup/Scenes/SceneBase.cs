using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using MapleSyrup.ECS.Components;
using MapleSyrup.Windowing;
using MapleSyrup.ECS;
using MapleSyrup.ECS.Components.Common;
using MapleSyrup.ECS.Components.Map;
using MapleSyrup.ECS.Systems;
using MapleSyrup.Networking;
using MapleSyrup.Networking.Packets;
using MapleSyrup.Nx;
using MapleSyrup.NX;
using MapleSyrup.Resources;
using MapleSyrup.Scenes.Map;
using ZeroElectric.Vinculum;
using Common_Transform = MapleSyrup.ECS.Components.Common.Transform;
using Transform = MapleSyrup.ECS.Components.Common.Transform;

namespace MapleSyrup.Scenes;

public abstract class SceneBase
{
    protected readonly List<IDrawSystem> DrawSystems;
    protected readonly List<IUpdateSystem> UpdateSystems;
    protected readonly string SceneName;
    public Camera2D Camera;
    public bool LoadingComplete { get; protected set; }

    public SceneBase(string sceneName)
    {
        DrawSystems = new List<IDrawSystem>(5);
        UpdateSystems = new List<IUpdateSystem>(5);
        SceneName = sceneName;
    }

    public abstract void InitSystems();

    public void LoadContent(MapleMap map)
    {
        LoadBackground(map);
        LoadObjects(map);
        LoadTiles(map);

        LoadingComplete = true;
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
                //entity.Layer = -1;
                transform.Position = new Vector2(background.X, background.Y);
                transform.Origin = Vector2.Zero;
                transform.Z = 0;

                var animated = background.NodePath.Contains("ani");
                if (animated)
                {
                    var animatedNode = NXFactory.Shared.GetNode(MapleFile.Map, background.NodePath)
                        ?? throw new NullReferenceException("Failed to find background animated node");
                    var nodes = NXFactory.Shared.GetChildren(MapleFile.Map, animatedNode);
                    var frames = new List<string>();
                    var alpha = new Queue<int>();
                    var blend = false;
                    
                    foreach (var (_, animation) in nodes)
                    {
                        var origin = NXFactory.Shared.GetChildNode(MapleFile.Map, animation, "origin")?.GetVector()
                                     ?? throw new NullReferenceException("Failed to find origin");
                        var delay = NXFactory.Shared.GetChildNode(MapleFile.Map, animation, "delay")?.GetInt() ??
                                    150f;
                        blend = NXFactory.Shared.HasNode(MapleFile.Map, animation, "a0");
                        if (blend)
                        {
                            var a0 = NXFactory.Shared.GetChildNode(MapleFile.Map, animation, "a0") 
                                     ?? throw new NullReferenceException("Failed to find a0");
                            var a1 = NXFactory.Shared.GetChildNode(MapleFile.Map, animation, "a1") 
                                     ?? throw new NullReferenceException("Failed to find a1");
                            alpha.Enqueue(a0.GetInt());
                            alpha.Enqueue(a1.GetInt());
                        }
                        if (!ResourceFactory.Shared.HasResource(animation.NodePath) && animation.Type == NodeType.Bitmap)
                        {
                            ResourceFactory.Shared.RegisterResource(new TextureResource(animation.NodePath)
                            {
                                MainFile = MapleFile.Map,
                                Origin = origin,
                                Delay = delay,
                            });
                            Console.WriteLine(origin);
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
                        Console.WriteLine("BLEND ANIMATION");
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
      
    }

    #endregion

    #region Load Tiles

    private void LoadTiles(MapleMap map)
    {
    }

    #endregion

    #region Draw/Update
    
    public void Draw()
    {
        if (!LoadingComplete) return;
        foreach (var system in DrawSystems)
        {
            system.Draw(EntityFactory.Shared, ResourceFactory.Shared);
        }
    }

    public void Update(float timeDelta)
    {
        if (!LoadingComplete) return;
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