using MapleSyrup.GameObjects;
using MapleSyrup.GameObjects.Components;
using MapleSyrup.Managers;
using MapleSyrup.Nx;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MapleSyrup.Scene;

public class GameWorld
{
    private NxNode _map;
    private string _mapId;
    private SpriteBatch sb;
    private Camera _camera;
    private object _threadLock;
    
    public GameWorld(string mapId)
    {
        _mapId = mapId;
        _camera = new Camera();
        _threadLock = new();
        sb = new SpriteBatch(ResourceManager.Instance.GraphicsDevice);
    }

    public void Load()
    {
        var resourceManager = ResourceManager.Instance;
        _map = resourceManager["Map"].GetNode($"{_mapId}.img");

        //LoadBackground();
        var layer = 0;
        do
        {
            // uncomment if you want it to be async loading...not recommended, looks weird.
            //var layer1 = layer;
            //Task.Run(() => LoadTile(layer1));
            //Task.Run(() => LoadObjects(layer1));
            LoadTile(layer);
            LoadObjects(layer);
            layer++;
        } while (layer < 8);
        
        LoadPortals();
    }

    #region [PRIVATE] Load Functions
    
    private void LoadBackground()
    {
        var root = _map["back"];

        foreach (var background in _map.Children)
        {
            var bS = background["bS"].GetString();
            var x = 0;
            var y = 0;
            var rx = 0;
            var ry = 0;
            var cx = 0;
            var cy = 0;
            
        }
    }

    private void LoadTile(int layer)
    {
        var resourceManager = ResourceManager.Instance;
        var actorManager = ActorManager.Instance;
        var root = _map[$"{layer}"];
        if (root["tile"].Children.Count == 0)
            return;
        var tS = root.Has("info", out var newSet) ? newSet["tS"].GetString() : _map["0"]["info"]["tS"].GetString();
        var tileSet = resourceManager["Map"].GetNode($"Tile/{tS}.img");

        foreach (var tile in root["tile"].Children)
        {
            var x = tile["x"].GetInt();
            var y = tile["y"].GetInt();
            var no = tile["no"].GetInt();
            var u = tile["u"].GetString();
            var texture = tileSet[u][$"{no}"].GetTexture(resourceManager.GraphicsDevice);
            var origin = tileSet[u][$"{no}"]["origin"].GetVector();
            var z = tileSet[u][$"{no}"]["z"].GetInt();
            var zM = tile["zM"].GetInt();
            var order = z + 10 * (3000 * (layer + 1) - zM) - 1073721834;
            
            actorManager.CreateTile((ActorLayer)layer + 1, texture, new Vector2(x, y), origin, order);
        }
    }

    private Random random = new Random();
    
    private void LoadObjects(int layer)
    {
        var resourceManager = ResourceManager.Instance;
        var actorManager = ActorManager.Instance;
        var root = _map[$"{layer}"];
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
            var zM = obj["zM"].GetInt();
            var order = (30000 * (layer + 1) + z) - 1073739824;
            var objSet = resourceManager["Map"].GetNode($"Obj/{oS}.img");
            var node = objSet[l0][l1][l2];

            if (node.Has("blend", out _))
            {
                LoadBlendAnimation();
                continue;
            }
            if (node.Has("obstacle", out _))
            {
                LoadObstacle();
                continue;
            }
            if (node.Has("seat", out _))
            {
                LoadSeat();
                continue;
            }
            if (node.Children.Count > 1)
            {
                LoadAnimatedObj(layer, obj, ref node);
                continue;
            }
            
            var origin = node["0"]["origin"].GetVector();
            actorManager.CreateObject((ActorLayer)layer + 1, node["0"].GetTexture(resourceManager.GraphicsDevice),
                new Vector2(x, y), origin, order, 0);
            
        }
    }

    private void LoadAnimatedObj(int layer, NxNode obj, ref NxNode node)
    {
        var resourceManager = ResourceManager.Instance;
        var actorManager = ActorManager.Instance;
        var x = obj["x"].GetInt();
        var y = obj["y"].GetInt();
        var z = obj["z"].GetInt();
        var f = obj["f"].GetInt();
        var zM = obj["zM"].GetInt();
        var order = (30000 * (layer + 1) + z) - 1073739824;

        var actor = actorManager.CreateObject((ActorLayer)layer + 1, new Vector2(x, y), node["0"]["origin"].GetVector(),
            order);
        var count = node.Children.Count; // Yes I know I can just make a variable node count but uhhh me no feel like it
        actor.Node = node;

        for (var i = 0; i < count - 1; i++)
        {
            var texture = node[$"{i}"].GetTexture(resourceManager.GraphicsDevice);
            actor.Animation.AddFrame(node.Has("delay", out var delay) ? delay.GetInt() : 150, texture);
        }
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
        var resourceManager = ResourceManager.Instance;
        var actorManager = ActorManager.Instance;
        var root = _map[$"portal"];
        var helper = resourceManager["Map"].GetNode("MapHelper.img");
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
                    var actor = actorManager.CreatePortal(portalNode["pv"], new Vector2(x, y), Vector2.Zero);
                    for (int i = 0; i < 7; i++)
                    {
                        actor.Animation.AddFrame(150, actor.Node[$"{i}"].GetTexture(resourceManager.GraphicsDevice));
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
    
    #endregion

    public void Draw()
    {
        lock (_threadLock)
        {
            var actorManager = ActorManager.Instance;
            var actors = actorManager.Actors.GetItems() as List<Actor>;
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp,
                DepthStencilState.Default, RasterizerState.CullNone, null, _camera.GetTransform());
            for (int i = 0; i < actors.Count; i++)
            {
                actors[i].Draw(sb);
            }

            sb.End();
        }
    }

    public void Update(GameTime gameTime)
    {
        lock(_threadLock)
        {
            var keyboard = Keyboard.GetState();
            var actorManager = ActorManager.Instance;
            var actors = actorManager.Actors.GetItems() as List<Actor>;
            for (int i = 0; i < actors.Count(); i++)
            {
                actors[i].Update(gameTime);
            }

            _camera.UpdateMatrix();

            if (keyboard.IsKeyDown(Keys.Left))
                _camera.Position.X -= 10f;
            if (keyboard.IsKeyDown(Keys.Right))
                _camera.Position.X += 10f;
            if (keyboard.IsKeyDown(Keys.Up))
                _camera.Position.Y -= 10f;
            if (keyboard.IsKeyDown(Keys.Down))
                _camera.Position.Y += 10f;
        }
    }

    public void Destroy()
    {
        _map.Dispose();
        sb.Dispose();
    }
}