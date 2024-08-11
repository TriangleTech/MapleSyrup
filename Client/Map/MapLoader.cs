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
        var timer = new Stopwatch();
        timer.Start();
        LoadBackground();
        for (var i = 0; i < 8; i++)
        {
            LoadObjects(i);
            LoadTile(i);
        }
        timer.Stop();
        ServiceLocator.Get<WorldManager>().GetWorld().SortLayers();
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
            actorManager.CreateObject(obj, layer);
        }
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