using System.Text.Json;
using CommunityToolkit.HighPerformance;
using MapleSyrup.NX;

namespace MapGenerator;

public class Generator
{
    private readonly NXFile _map = new NXFile("D:/v41/Map.nx");
    private readonly NXFile _ui = new NXFile("D:/v41/UI.nx");

    public void Generate()
    {
        if (!Directory.Exists("MapData"))
        {
            Directory.CreateDirectory("MapData");
        }
        
        Console.WriteLine("Generating maps...");
        var mapleMap = new MapleMap();
        
        var mapNode = _map.GetNode("Map") ?? throw new NullReferenceException("Failed to find [Map] node");
        var mapNodes = _map.GetChildren(mapNode);
        foreach (var (_, node) in mapNodes)
        {
            var mapIds = _map.GetChildren(node);
            foreach (var (id, map) in mapIds)
            {
                var withoutImg = id.Replace(".img", "");
                if (withoutImg.Length < 9) continue;
                if (File.Exists($"MapData/{withoutImg}.json")) continue;
                
                ParseBackgrounds(_map, map, mapleMap.Backgrounds);
                for (var i = 0; i < 8; i++)
                {
                    ParseObjects(_map, map, mapleMap.Objects, i);
                    ParseTiles(_map, map, mapleMap.Tiles, i);
                }
            
                var jsonString = JsonSerializer.Serialize(mapleMap, MapleMapContext.Default.MapleMap);
                File.WriteAllText($"MapData/{withoutImg}.json", jsonString);
                Console.WriteLine($"Map with ID: {withoutImg} was generated.");
                mapleMap.Clear();
            }
        }

        {
            var login = _ui.GetFastImg("MapLogin.img") 
                           ?? throw new NullReferenceException("Failed to find [MapLogin.img] node");
            var withoutImg = login.Name.Replace(".img", "");
            ParseBackgrounds(_ui, login, mapleMap.Backgrounds);
            for (var i = 0; i < 8; i++)
            {
                ParseObjects(_ui, login, mapleMap.Objects, i);
                ParseTiles(_ui, login, mapleMap.Tiles, i);
            }

            var jsonString = JsonSerializer.Serialize(mapleMap, MapleMapContext.Default.MapleMap);
            File.WriteAllText($"MapData/{withoutImg}.json", jsonString);
            Console.WriteLine($"Map with ID: {withoutImg} was generated.");
        }
        
        mapleMap.Clear();
        _map.Dispose();
        _ui.Dispose();
    }

    private void ParseBackgrounds(NXFile file, NXNode mapNode, List<MapBackground> backgrounds)
    {
        try
        {
            if (!file.HasNode(mapNode, "back")) return;
            var back = file.GetChildNode(mapNode, "back") ??
                       throw new NullReferenceException("Failed to find [back] node in img file");
            var backgroundNodes = file.GetChildren(back);
            foreach (var (_, background) in backgroundNodes)
            {
                var node = file.GetChildren(background);
                var bS = node["bS"].GetString();
                var no = node["no"].GetInt();
                var x = node["x"].GetInt();
                var y = node["y"].GetInt();
                var rx = node["rx"].GetInt();
                var ry = node["ry"].GetInt();
                var cx = node["cx"].GetInt();
                var cy = node["cy"].GetInt();
                var a = node["a"].GetInt();
                var front = node.TryGetValue("front", out var frontNode) ? frontNode.GetInt() : 0;
                var ani = node.TryGetValue("ani", out var aniNode) ? aniNode.GetInt() : 0;
                var f = node.TryGetValue("f", out var fNode) ? fNode.GetInt() : 0;
                var type = node["type"].GetInt();

                if (ani == 1)
                {
                    backgrounds.Add(new MapBackground()
                    {
                        NodePath = $"Back/{bS}.img/ani/{no}",
                        BackgroundType = type,
                        X = x,
                        Y = y,
                        Rx = rx,
                        Ry = ry,
                        Cx = cx,
                        Cy = cy,
                        Z = 0,
                    });
                }
                else
                {
                    backgrounds.Add(new MapBackground()
                    {
                        NodePath = $"Back/{bS}.img/back/{no}",
                        BackgroundType = type,
                        X = x,
                        Y = y,
                        Rx = rx,
                        Ry = ry,
                        Cx = cx,
                        Cy = cy,
                        Z = 0,
                    });
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void ParseObjects(NXFile file, NXNode mapNode, List<MapObject> objects, int i)
    {
        try
        {
            if (!file.HasNode(mapNode, i.ToString())) return;
            var layer = file.GetChildNode(mapNode, i.ToString()) ?? throw new NullReferenceException();
            var obj = file.GetChildNode(layer, "obj") ??
                      throw new NullReferenceException($"Failed to find [obj] node");
            if (obj.ChildCount == 0) return;

            var objNodes = file.GetChildren(obj);
            foreach (var (_, objNode) in objNodes)
            {
                var nodes = file.GetChildren(objNode);
                var oS = nodes["oS"].GetString();
                var l0 = nodes["l0"].GetString();
                var l1 = nodes["l1"].GetString();
                var l2 = nodes["l2"].GetString();
                var x = nodes["x"].GetInt();
                var y = nodes["y"].GetInt();
                var z = nodes["z"].GetInt();
                // TODO: Eventually find out what these do. Commented out to save CPU processing.
                //var zM = nodes["zM"].GetInt();
                //var f = nodes["f"].GetInt(); // Pretty sure this means flipped
                var order = 30000 * i + z - 1073739824;
                
                objects.Add(new MapObject()
                {
                    Layer = i,
                    NodePath = $"Obj/{oS}.img/{l0}/{l1}/{l2}",
                    ObjType = 0,
                    X = x,
                    Y = y,
                    Z = order,
                });
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void ParseTiles(NXFile file, NXNode mapNode, List<MapTile> tiles, int i)
    {
        try
        {
            if (!file.HasNode(mapNode, i.ToString())) return;
            var layerNode = file.GetChildNode(mapNode, i.ToString()) ??
                            throw new NullReferenceException("Failed to find [layer] node");
            var tileLayer = file.GetChildNode(layerNode, "tile") ??
                            throw new NullReferenceException("Failed to find [tile] node");
            if (tileLayer.ChildCount == 0) return;
            
            var info = file.GetChildNode(layerNode, "info") ??
                       throw new NullReferenceException("Failed to find [info] node");
            var infoNodes = file.GetChildren(info);
            
            var tS = "";
            if (infoNodes.TryGetValue("tS", out var tSNode))
            {
                tS = tSNode.GetString();
            }
            else
            {
                tS = file.GetNode($"{mapNode.NodePath}/0/info/tS")?.GetString() ??
                     "grassySoil";
            }
            
            var tileNodes = file.GetChildren(tileLayer);
            foreach (var (_, tileNode) in tileNodes)
            {
                var tile = file.GetChildren(tileNode);
                var x = tile["x"].GetInt();
                var y = tile["y"].GetInt();
                var zM = tile["zM"].GetInt();
                var u = tile["u"].GetString();
                var no = tile["no"].GetInt();

                var tileSet = _map.GetNode($"Tile/{tS}.img/{u}/{no}") ??
                              throw new NullReferenceException("Failed to find [tile] node");
                var setNodes = _map.GetChildren(tileSet);
                var z = setNodes.TryGetValue("z", out var zNode) ? zNode.GetInt() : 0;
                var order = z + 10 * (3000 * i - zM) - 1073721834;
                
                tiles.Add(new MapTile()
                {
                    Layer = i,
                    NodePath = tileSet.NodePath,
                    X = x,
                    Y = y,
                    Z = order,
                    TileType = 0,
                });
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}