using System.Numerics;
using System.Text.Json;
using MapleSyrup.Common.Map;
using MapleSyrup.NX;

namespace DataGenerator;

public class MapGenerator
{
    private readonly NXFile _map = new NXFile("D:/v41/Map.nx");
    private readonly NXFile _ui = new NXFile("D:/v41/UI.nx");

    public void GenerateMapData()
    {
        if (!Directory.Exists("MapData"))
        {
            Directory.CreateDirectory("MapData");
        }
        
        Console.WriteLine("Generating maps...");
        var mapleMap = new MapleMap();
        var mapNode = _map.GetNode("Map") ?? throw new NullReferenceException("Failed to find [Map] node");
        var mapNodes = mapNode.GetChildren();
        foreach (var (_, node) in mapNodes)
        {
            var mapImgs = node.GetChildren();
            foreach (var (id, map) in mapImgs)
            {
                var withoutImg = id.Replace(".img", "");
                if (withoutImg.Length < 9) 
                    continue;
                
                //ParseInfo(map, mapleMap.MapInfo);
                ParseBackgrounds(_map, map, mapleMap.Backgrounds);
                for (var i = 0; i < 8; i++)
                {
                    ParseObjects(_map, map, mapleMap.Objects, i);
                    ParseTiles(_map, map, mapleMap.Tiles, i);
                }
                ParseFootholds(map, mapleMap.Footholds);
            
                var jsonString = JsonSerializer.Serialize(mapleMap, MapleMapContext.Default.MapleMap);
                using var fs = File.Open($"MapData/{withoutImg}.json", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                using var writer = new StreamWriter(fs);
                writer.Write(jsonString);
                
                //File.WriteAllText($"MapData/{withoutImg}.json", jsonString);
                Console.WriteLine($"Map with ID: {withoutImg} was generated.");
                
                mapleMap.Clear();
            }
            
            //mapImgs.Clear();
        }
        
        //mapNodes.Clear();

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
    
    #region Backgrounds

    private void ParseBackgrounds(NXFile file, NXNode mapNode, List<MapBackground> backgrounds)
    {
        try
        {
            var back = file.GetChildNode(mapNode, "back") ??
                       throw new NullReferenceException("Failed to find [back] node in img file");
            
            var backgroundNodes = back.GetChildren();
            foreach (var (_, background) in backgroundNodes)
            {
                var node = background.GetChildren();
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
                
                //node.Clear();
            }
            
            //backgroundNodes.Clear();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    #endregion

    #region Objects
    
    private void ParseObjects(NXFile file, NXNode mapNode, List<MapObject> objects, int i)
    {
        try
        {
            var layer = file.GetChildNode(mapNode, i.ToString()) ?? throw new NullReferenceException();
            var obj = file.GetChildNode(layer, "obj") ??
                      throw new NullReferenceException($"Failed to find [obj] node");
            if (obj.ChildCount == 0) 
                return;

            var objNodes = obj.GetChildren();
            foreach (var (_, objNode) in objNodes)
            {
                var nodes = objNode.GetChildren();
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
                
                //nodes.Clear();
            }
            
            //objNodes.Clear();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    #endregion
    
    #region Tiles

    private void ParseTiles(NXFile file, NXNode mapNode, List<MapTile> mapTiles, int i)
    {
        try
        {
           
            var layerNode = file.GetChildNode(mapNode, i.ToString()) ??
                            throw new NullReferenceException("Failed to find [layer] node");
            var tileLayer = file.GetChildNode(layerNode, "tile") ??
                            throw new NullReferenceException("Failed to find [tile] node");
            if (tileLayer.ChildCount == 0) 
                return;
            
            var info = file.GetChildNode(layerNode, "info") ??
                       throw new NullReferenceException("Failed to find [info] node");
            var infoNodes = info.GetChildren();
            
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
            
            var tileNodes = tileLayer.GetChildren();
            foreach (var (_, tileNode) in tileNodes)
            {
                var tiles = tileNode.GetChildren();
                var x = tiles["x"].GetInt();
                var y = tiles["y"].GetInt();
                var zM = tiles["zM"].GetInt();
                var u = tiles["u"].GetString();
                var no = tiles["no"].GetInt();

                var tileSet = _map.GetNode($"Tile/{tS}.img/{u}/{no}") ??
                              throw new NullReferenceException("Failed to find [tile] node");
                var setNodes = tileSet.GetChildren();
                var z = setNodes.TryGetValue("z", out var zNode) ? zNode.GetInt() : 0;
                var order = z + 10 * (3000 * i - zM) - 1073721834;
                
                mapTiles.Add(new MapTile()
                {
                    Layer = i,
                    NodePath = tileSet.NodePath,
                    X = x,
                    Y = y,
                    Z = order,
                    TileType = 0,
                });
                
                //setNodes.Clear();
                //tiles.Clear();
            }
            
            //infoNodes.Clear();
            //tileNodes.Clear();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    #endregion
    
    #region Footholds

    private void ParseFootholds(NXNode mapNode, List<MapFoothold> mapFootholds)
    {
        var rootNode = _map.GetChildNode(mapNode, "foothold") ?? throw new NullReferenceException("Failed to find foothold node");
        var footholdLayers = rootNode.GetChildren();

        foreach (var (_, layer) in footholdLayers)
        {
            var footholdGroups = layer.GetChildren();
            foreach (var (_, group) in footholdGroups)
            {
                var footholds = group.GetChildren();
                foreach (var (_, foothold) in footholds)
                {
                    var nodes = foothold.GetChildren();
                    var x1 = nodes["x1"].GetInt();
                    var y1 = nodes["y1"].GetInt();
                    var x2 = nodes["x2"].GetInt();
                    var y2 = nodes["y2"].GetInt();

                    mapFootholds.Add(new MapFoothold()
                    {
                        Layer = int.Parse(layer.Name),
                        X1 = x1,
                        Y1 = y1,
                        X2 = x2,
                        Y2 = y2,
                    });
                    
                    //nodes.Clear();
                }
                
                //footholds.Clear();
            }
            
            //footholdGroups.Clear();
        }
        
        //footholdLayers.Clear();
    }
    
    #endregion
}