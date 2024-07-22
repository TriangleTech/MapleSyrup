using System.Diagnostics;
using System.Numerics;
using Client.Actors;
using Client.Managers;
using Client.NX;
using Raylib_CsLo;

namespace Client.Map;

public class MapLoader
{
    private NxNode _node;

    public bool Load(NxNode node)
    {
        _node = node;
        var actor = ServiceLocator.Get<ActorManager>();
        var timer = new Stopwatch();
        timer.Start();
        LoadBackground();
        for (var i = 0; i < 8; i++)
        {
            LoadObjects(i);
            LoadTile(i);
        }
        timer.Stop();
        actor.SortAll();
        Console.WriteLine($"Map fully loaded in: {timer.ElapsedMilliseconds} ms");
        return true;
    }
    
        #region Loading Functions
    
    private void LoadBackground()
    {
        var root = _node["back"];
        var actorManager = ServiceLocator.Get<ActorManager>();

        for (var i = 0; i < root.ChildCount; i++)
        {
            var background = root[$"{i}"];
            actorManager.CreateBackground(background);
        }
    }

    private void LoadTile(int layer)
    {
        var actorManager = ServiceLocator.Get<ActorManager>();
        var root = _node[$"{layer}"];
        if (root["tile"].ChildCount == 0)
            return;
        var tS = root.Has("info", out var newSet) ? newSet.GetString("tS") : _node["0"]["info"].GetString("tS");
        var tileSet = ServiceLocator.Get<NxManager>().Get(MapleFiles.Map).GetNode($"Tile/{tS}.img");

        for (var i = 0; i < root["tile"].ChildCount; i++)
        {
            var tile = root["tile"][$"{i}"]; 
            actorManager.CreateTile(tileSet, layer, tile);
        }
    }
    
    private void LoadObjects(int layer)
    {
        var actorManager = ServiceLocator.Get<ActorManager>();;
        var root = _node[$"{layer}"];
        
        for (var i = 0; i < root["obj"].ChildCount; i++)
        {
            var obj = root["obj"][$"{i}"];
            var oS = obj.GetString("oS");
            var l0 = obj.GetString("l0");
            var l1 = obj.GetString("l1");
            var l2 = obj.GetString("l2");
            var x = obj.GetInt("x");
            var y = obj.GetInt("y");
            var z = obj.GetInt("z");
            var f = obj.GetInt("f");
            var zM = obj.GetInt("zM");
            var order = (30000 * (int)(ActorLayer.TileLayer0 + layer) + z) - 1073739824;
            var objSet = ServiceLocator.Get<NxManager>().Get(MapleFiles.Map).GetNode($"Obj/{oS}.img");
            var node = objSet[l0][l1][l2];

            if (node.Has("blend"))
            {
                //LoadBlendAnimation();
                continue;
            }
            if (node.Has("obstacle"))
            {
                //LoadObstacle();
                continue;
            }
            if (node.Has("seat"))
            {
                //LoadSeat();
                continue;
            }
            if (node.ChildCount > 1)
            {
                LoadAnimatedObj(layer, obj, ref node);
                continue;
            }
            
            var origin = node["0"].GetVector("origin");
            var texture = node.GetTexture("0");
            actorManager.Create(new MapObject(node, texture, new Vector2(x, y), origin, ActorLayer.TileLayer0 + layer, order));
        }
    }

    private void LoadAnimatedObj(int layer, NxNode obj, ref NxNode node)
    {
        
        var actorManager = ServiceLocator.Get<ActorManager>();;
        var x = obj.GetInt("x");
        var y = obj.GetInt("y");
        var z = obj.GetInt("z");
        var f = obj.GetInt("f");
        var zM = obj.GetInt("zM");
        var order = (30000 * (int)(ActorLayer.TileLayer0 + layer) + z) - 1073739824;

        var frames = new List<Texture>(node.ChildCount);
        for (var i = 0; i < node.ChildCount - 1; i++)
        {
            var texture = node.GetTexture($"{i}");
            frames.Add(texture);
        }
        actorManager.Create(new MapObject(node, frames, new Vector2(x, y), ActorLayer.TileLayer0 + layer, order));
    }

    private void LoadPortals()
    {
        var actorManager = ServiceLocator.Get<ActorManager>();;
        var root = _node["portal"];
        var helper = ServiceLocator.Get<NxManager>().Get(MapleFiles.Map).GetNode("MapHelper.img");
        var portalNode = helper["portal"]["game"];

        for (var i = 0; i < root.ChildCount; i++)
        {
            var portal = root[$"{i}"];
            var portalName = portal.GetString("pn");
            var portalType = portal.GetInt("pt");
            var x = portal.GetInt("x");
            var y = portal.GetInt("y");
            var targetMap = portal.GetInt("tm");
            var targetName = portal.GetString("tn");

            portal.Has("delay", out _); // TODO: Handle later
            portal.Has("script", out _);
            portal.Has("hideToolTip", out _);
            portal.Has("onlyOnce", out _);

            switch (portalType)
            {
                case 0:
                case 1:
                case 2:
                {
                }
                    break;
                case 9:
                {
                }
                    break;
            }
        }
    }
    
    #endregion
}