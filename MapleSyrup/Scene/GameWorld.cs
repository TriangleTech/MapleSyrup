using MapleSyrup.GameObjects;
using MapleSyrup.Managers;
using MapleSyrup.Nx;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Scene;

public class GameWorld
{
    private NxNode map;
    private string _mapId;
    private ActorManager _actor;
    private ResourceManager _resource;
    private SpriteBatch sb;
    
    public GameWorld(string mapId, ref ActorManager actorManager, ref ResourceManager resourceManager)
    {
        _mapId = mapId;
        _actor = actorManager;
        _resource = resourceManager;
        sb = new SpriteBatch(resourceManager.GraphicsDevice);
    }

    public void Load()
    {
        map = _resource["Map"].GetNode($"{_mapId}.img");

        LoadBackground();
        var layer = 0;
        do
        {
            // uncomment if you want it to be async loaded...not recommended, looks weird.
            //var layer1 = layer;
            //Task.Run(() => LoadTile(layer1));
            //Task.Run(() => LoadObjects(layer1));
            LoadTile(layer);
            LoadObjects(layer);
            layer++;
        } while (layer < 8);
        
        LoadPortals();
    }

    private void LoadBackground()
    {
        
    }

    private void LoadTile(int layer)
    {
        var root = map[$"{layer}"];
        var tS = root?["info"]?["tS"]?.GetString() ?? map["0"]?["info"]?["tS"]?.GetString();
        var tileSet = _resource["Map"].GetNode($"Tile/{tS}.img");

        foreach (var tile in root["tile"].Children)
        {
            var x = tile["x"].GetInt();
            var y = tile["y"].GetInt();
            var no = tile["no"].GetInt();
            var u = tile["u"].GetString();
            var texture = tileSet[u][$"{no}"].GetTexture(_resource.GraphicsDevice);
            var origin = tileSet[u][$"{no}"]["origin"].GetVector();
            var z = tileSet[u][$"{no}"]["z"].GetInt();
            var zM = tile["zM"].GetInt();
            var order = z + 10 * (3000 * (layer + 1) - zM) - 1073721834;
            
            _actor.CreateTile((ActorLayer)layer + 1, texture, new Vector2(x, y), origin, order);
        }
    }

    private void LoadObjects(int layer)
    {
        var root = map[$"{layer}"];
        foreach (var obj in root["obj"].Children)
        {
            var oS = obj["oS"].GetString();
            var l0 = obj["l0"].GetString();
            var l1 = obj["l1"].GetString();
            var l2 = obj["l2"].GetString();
            var x = obj["x"].GetInt();
            var y = obj["y"].GetInt();
            var z = obj["z"].GetInt();
            var f = obj["f"].GetInt();
            var zM = obj["x"].GetInt();
            var order = (30000 * (layer + 1) + z) - 1073739824;
            var objSet = _resource["Map"].GetNode($"Obj/{oS}.img");
            var node = objSet[l0][l1][l2];

            if (node.Children.Count > 1)
            {
                LoadAnimatedObj(layer, obj, ref node);
                continue;
            }

            var origin = node["0"]["origin"].GetVector();
            //var delay = node["0"]["delay"].GetInt();
            _actor.CreateObject((ActorLayer)layer + 1, node["0"].GetTexture(_resource.GraphicsDevice),
                new Vector2(x, y), origin, order, 150);
        }
    }

    private void LoadAnimatedObj(int layer, NxNode obj, ref NxNode node)
    {
        var x = obj["x"].GetInt();
        var y = obj["y"].GetInt();
        var z = obj["z"].GetInt();
        var f = obj["f"].GetInt();
        var zM = obj["x"].GetInt();
        var order = (30000 * (layer + 1) + z) - 1073739824;
        
    }

    private void LoadPortals()
    {
        var root = map[$"portal"];
        var helper = _resource["Map"].GetNode("MapHelper.img");
        var portalNode = helper["portal"]["game"];

        foreach (var portal in root.Children)
        {
            var portalName = portal["pn"].GetString();
            var portalType = portal["pt"].GetInt();
            var x = portal["x"].GetInt();
            var y = portal["y"].GetInt();
            var targetMap = portal["tm"].GetInt();
            var targetName = portal["tn"].GetString();

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
                    var actor = _actor.CreatePortal(portalNode["pv"], new Vector2(x, y), Vector2.Zero);
                    for (int i = 0; i < 7; i++)
                    {
                        actor.Animation.AddFrame(150, actor.Node[$"{i}"].GetTexture(_resource.GraphicsDevice));
                    }
                }
                    break;
                case 9:
                {
                    // var actor = _actor.CreatePortal(portalNode["psh"], new Vector2(x, y), Vector2.Zero);

                }
                    break;
            }
        }
    }

    public void Draw()
    {
        sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
        var actors = _actor.Actors.GetItems() as List<Actor>;
        for (int i = 0; i < actors.Count(); i++)
        {
            actors[i].Draw(sb);
        }
        sb.End();
    }

    public void Update(GameTime gameTime)
    {
        var actors = _actor.Actors.GetItems() as List<Actor>;
        for (int i = 0; i < actors.Count(); i++)
        {
            actors[i].Update(gameTime);
        }
    }

    public void Destroy()
    {
        map.Dispose();
        sb.Dispose();
    }
}