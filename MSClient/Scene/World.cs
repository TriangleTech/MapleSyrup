using System.Diagnostics;
using System.Numerics;
using MSClient.Actors;
using MSClient.Managers;
using MSClient.Map;
using MSClient.Net;
using MSClient.NX;
using Raylib_CsLo;

namespace MSClient.Scene;

public class World : Actor
{
    private bool _isLogin;
    private readonly NxNode _ui;
    
    public WorldState State { get; set; }
    
    public World(ref NxNode mapNode)
    : base(ref mapNode)
    {
        _isLogin = false;
    }

    public World(ref NxNode mapNode, ref NxNode uiNode)
    : base(ref mapNode)
    {
        _isLogin = true;
        _ui = uiNode;
    }

    public void Load()
    {
        var timer = new Stopwatch();
        timer.Start();
        for (var i = 0; i < 8; i++)
        {
            LoadObjects(i);
            LoadTile(i);
        }
        timer.Stop();
        Console.WriteLine($"Map fully loaded in: {timer.ElapsedMilliseconds} ms");
    }
    
    #region Loading Functions
    
    private void LoadBackground()
    {
        var root = _node["back"];

       for (var i = 0; i < root.ChildCount; i++)
       {
           var background = root[$"{i}"];
           var bS = background.GetString("bS");
            var x = background.GetInt("x");
            var y = background.GetInt("y");
            var rx = background.GetInt("rx");
            var ry = background.GetInt("ry");
            var cx = background.GetInt("cx");
            var cy = background.GetInt("cy");
            
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
            var x = tile.GetInt("x");
            var y = tile.GetInt("y");
            var no = tile.GetInt("no");
            var u = tile.GetString("u");
            var texture = tileSet[u].GetTexture($"{no}");
            var origin = tileSet[u][$"{no}"].GetVector("origin");
            var z = tileSet[u][$"{no}"].GetInt("z");
            var zM = tile.GetInt("zM");
            var order = z + 10 * (3000 * (layer + 1) - zM) - 1073721834;
            
            actorManager.Create(new MapObject(ref tile, texture, new Vector2(x, y), origin, ActorLayer.TileLayer0 + layer, order));
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
            var order = (30000 * (layer + 1) + z) - 1073739824;
            var objSet = ServiceLocator.Get<NxManager>().Get(MapleFiles.Map).GetNode($"Obj/{oS}.img");
            var node = objSet[l0][l1][l2];

            if (node.Has("blend", out _))
            {
                //LoadBlendAnimation();
                continue;
            }
            if (node.Has("obstacle", out _))
            {
                //LoadObstacle();
                continue;
            }
            if (node.Has("seat", out _))
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
            
            actorManager.Create(new MapObject(ref node, texture, new Vector2(x, y), origin, ActorLayer.TileLayer0 + layer, order));
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
        var order = (30000 * (layer + 1) + z) - 1073739824;

        var frames = new List<Texture>(node.ChildCount);
        for (var i = 0; i < node.ChildCount - 1; i++)
        {
            var texture = node.GetTexture($"{i}");
            frames.Add(texture);
        }
        actorManager.Create(new MapObject(ref node, frames, new Vector2(x, y), ActorLayer.TileLayer0 + layer, order));
    }

    private void LoadBlendAnimation()
    {
        
    }

    private void LoadObstacle()
    {
        
    }

    private void LoadSeat()
    {
        
    }

    private void LoadPortals()
    {
        
        var actorManager = ServiceLocator.Get<ActorManager>();;
        var root = _node[$"portal"];
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
    
    public override void ProcessPacket(PacketResponse response)
    {
        if (_isLogin)
        {
            switch (response.Opcode) 
            {
                
            }
        }
        else
        {
            switch (response.Opcode)
            {
                
            }
        }
    }
    
    public override void Update(float frameTime)
    {
        var actor = ServiceLocator.Get<ActorManager>();
        actor.Update(frameTime);
        actor.ValidateActors();
    }

    public override void Draw(float frameTime)
    {
        var actor = ServiceLocator.Get<ActorManager>();
        actor.Draw(frameTime);
    }
    
    public override void Clear()
    {
        var actor = ServiceLocator.Get<ActorManager>();
        actor.ClearActors();
    }
}