using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Client.Actors;
using Client.Avatar;
using Client.Map;
using Client.Net;
using Client.NX;
using Raylib_CsLo;

namespace Client.Managers;

/// <summary>
/// The ActorManager class is responsible for managing game actors.
/// </summary>
public class ActorManager : IManager
{
    /// <summary>
    /// The lock object used for thread synchronization in the ActorManager class.
    /// </summary>
    private object _threadLock;

    public ActorManager()
    {
        _threadLock = new();
    }

    /// <summary>
    /// Initializes the ActorManager by creating and initializing a dictionary to store actors on different layers.
    /// </summary>
    public void Initialize()
    {
    }

    /// <summary>
    /// Shuts down the ActorManager.
    /// </summary
    public void Shutdown()
    {
    }

    #region Map Loading


    /// <summary>
    /// Creates a background actor based on the provided background node.
    /// </summary>
    /// <param name="background">The background node containing the information to create the background actor.</param>
    public void CreateBackground(NxNode background)
    {
        var world = ServiceLocator.Get<WorldManager>().GetWorld();
        var bS = background.GetString("bS");
        var no = background.GetInt("no");
        var x = background.GetInt("x");
        var y = background.GetInt("y");
        var rx = background.GetInt("rx");
        var ry = background.GetInt("ry");
        var cx = background.GetInt("cx");
        var cy = background.GetInt("cy");
        var a = background.GetInt("a");
        var front = background.GetInt("front");
        var ani = background.GetInt("ani");
        var f = background.GetInt("f");
        var type = background.GetInt("type");

        if (ani == 1)
        {
            var backgroundSet = ServiceLocator.Get<NxManager>().GetNode($"Back/{bS}.img");
            var back = backgroundSet["ani"][$"{no}"];
            var frames = new List<Texture>(back.ChildCount);
            for (var j = 0; j < back.ChildCount - 1; j++)
            {
                var texture = back.GetTexture($"{j}");
                frames.Add(texture);
            }
            
            if (cx == 0) cx = frames[0].width;
            if (cy == 0) cy = frames[0].height;

            world.AddActor(new Background
            {
                Node = back,
                ID = world.GenerateID(),
                Position = new Vector2(x, y),
                Origin = back["0"].GetVector("origin"),
                Z = no,
                Cx = cx,
                Cy = cy,
                Rx = rx,
                Ry = ry,
                BackgroundType = type,
                Layer = front == 1 ? ActorLayer.Foreground : ActorLayer.Background,
                Frames = frames,
                FrameCount = frames.Count,
                Animated = true,
                ActorType = ActorType.Background, 
                TexturePath = back.FullPath,
            });
        }
        else
        {
            var backgroundSet = ServiceLocator.Get<NxManager>().GetNode($"Back/{bS}.img");
            var back = backgroundSet["back"];
            var texture = back.GetTexture($"{no}");
            var origin = back[$"{no}"].GetVector("origin");
            if (cx == 0) cx = texture.width;
            if (cy == 0) cy = texture.height;
            
            world.AddActor(new Background
            {
                Node = back,
                ID = world.GenerateID(),
                Position = new Vector2(x, y),
                Origin = origin,
                Z = no,
                Cx = cx,
                Cy = cy,
                Rx = rx,
                Ry = ry,
                BackgroundType = type,
                Layer = front == 1 ? ActorLayer.Foreground : ActorLayer.Background,
                Frames = [texture],
                FrameCount = 1,
                Animated = false,
                ActorType = ActorType.Background,
                TexturePath = backgroundSet["back"][$"{no}"].FullPath,
                Width = texture.width,
                Height = texture.height,
            });
        }
        
        //Console.WriteLine($"Background with type: {type} was created");
    }

    /// <summary>
    /// Creates a tile actor based on the given tile data.
    /// </summary>
    /// <param name="tileSet">The tile set containing the textures for the tile.</param>
    /// <param name="layer">The layer index of the tile.</param>
    /// <param name="tile">The tile data containing the position, texture, origin, and order information.</param>
    public void CreateTile(NxNode tileSet, int layer, NxNode tile)
    {
        var world = ServiceLocator.Get<WorldManager>().GetWorld();
        var x = tile.GetInt("x");
        var y = tile.GetInt("y");
        var no = tile.GetInt("no");
        var u = tile.GetString("u");
        var texture = tileSet[u].GetTexture($"{no}");
        var origin = tileSet[u][$"{no}"].GetVector("origin");
        var z = tileSet[u][$"{no}"].GetInt("z");
        var zM = tile.GetInt("zM");
        var order = z + 10 * (3000 * (int)(ActorLayer.TileLayer0 + layer) - zM) - 1073721834;
        world.AddActor(new MapObject
        {
            Node = tile,
            ID = world.GenerateID(),
            Position = new Vector2(x, y),
            Origin = origin,
            Layer = ActorLayer.TileLayer0 + layer,
            Z = order,
            Frames = [texture],
            FrameCount = 1,
            Animated = false,
            Blend = false,
            LoopCount = -1,
            ActorType = ActorType.Tile,
        });

    }

    /// <summary>
    /// Creates an object in the game world based on the given NxNode and layer.
    /// </summary>
    /// <param name="obj">The NxNode representing the object to create.</param>
    /// <param name="layer">The layer on which to create the object.</param>
    public void CreateObject(NxNode obj, int layer)
    {
        var world = ServiceLocator.Get<WorldManager>().GetWorld();
        var oS = obj.GetString("oS");
        var l0 = obj.GetString("l0");
        var l1 = obj.GetString("l1");
        var l2 = obj.GetString("l2");
        var x = obj.GetInt("x");
        var y = obj.GetInt("y");
        var z = obj.GetInt("z");
        var f = obj.GetInt("f");
        var zM = obj.GetInt("zM");
        var order = 30000 * (int)(ActorLayer.TileLayer0 + layer) + z - 1073739824;
        var objSet = ServiceLocator.Get<NxManager>().GetNode($"Obj/{oS}.img");
        var node = objSet[l0][l1][l2];

        if (node.ChildCount > 1)
        {
            var frames = new List<Texture>(node.ChildCount);
            var blend = node["0"].Has("a0");
            for (var i = 0; i < node.ChildCount - 1; i++)
            {
                var texture = node.GetTexture($"{i}");
                frames.Add(texture);
            }

            world.AddActor(new MapObject
            {
                Node = node,
                ID = world.GenerateID(),
                Position = new Vector2(x, y),
                Origin = node["0"].GetVector("origin"),
                Layer = ActorLayer.TileLayer0 + layer,
                Z = order,
                Frames = frames,
                FrameCount = frames.Count,
                Animated = true,
                Blend = blend,
                LowerAlpha = blend ? node["0"].GetInt("a0") : 255,
                UpperAlpha = blend ? node["0"].GetInt("a1") : 255,
                LoopCount = node["0"].Has("repeat") ? node["0"].GetInt("repeat") : -1,
                ActorType = ActorType.AnimatedMapObject,
                TexturePath = node.FullPath
            });
        }
        else
        {
            var origin = node["0"].GetVector("origin");
            var texture = node.GetTexture("0");
            world.AddActor(new MapObject
            {
                Node = node,
                ID = world.GenerateID(),
                Position = new Vector2(x, y),
                Origin = origin,
                Layer = ActorLayer.TileLayer0 + layer,
                Z = order,
                Frames = [texture],
                FrameCount = 1,
                Animated = false,
                Blend = false,
                LoopCount = -1,
                ActorType = ActorType.StaticMapObject,
                TexturePath = $"{node.FullPath}/0"
            });
        }

    }

    #endregion
}