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
            
            _actor.CreateTile((ActorLayer)layer + 1, texture, new Vector2(x, y), origin, z);
        }
    }

    private void LoadObjects(int layer)
    {
        var root = map[$"{layer}"];
        foreach (var tile in root["obj"].Children)
        {
        }
    }

    private void LoadPortals()
    {
        
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