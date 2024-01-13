using MapleSyrup.EC;
using MapleSyrup.Event;
using MapleSyrup.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Map;

public class MapleMap : IEventListener
{
    private readonly ManagerLocator _locator;
    private readonly string _worldId;

    public EventFlag Flags { get; }

    public MapleMap(string mapId, ManagerLocator locator)
    {
        Flags = EventFlag.OnMapChanged;
        _locator = locator;
        _locator.GetManager<EventManager>().Register(this);
        _worldId = mapId;
    }

    public void Load()
    {
        LoadInfo();
        LoadBackground();
        LoadTile();
        LoadObj();
        
        var events = _locator.GetManager<EventManager>();
        events.Dispatch(EventFlag.OnMapLoaded);
    }

    public void Unload()
    {
        var events = _locator.GetManager<EventManager>();
        events.Dispatch(EventFlag.OnMapUnloaded);
    }

    public void ProcessEvent(EventFlag flag)
    {

    }

    public void ProcessEvent(EventFlag flag, IEntity entity)
    {

    }

    #region Load Functions

    private void LoadInfo()
    {
        var resource = _locator.GetManager<ResourceManager>();
    }

    private void LoadBackground()
    {
        var resource = _locator.GetManager<ResourceManager>();
        var entity = _locator.GetManager<EntityManager>();
        var backgroundData = resource.LoadMapData($"{_worldId}.img/back");

        for (int i = 0; i < resource.GetBackgroundCount($"{_worldId}.img"); i++)
        {
            if ((string)backgroundData[$"{i}", "bS"] == string.Empty)
                continue;

            if ((int)backgroundData[$"{i}", "ani"] == 0)
            {
                var origin =
                    (Vector2)resource.GetOrigin(
                        $"Map/Back/{backgroundData[$"{i}", "bS"]}.img/back/{backgroundData[$"{i}", "no"]}");

                var background = entity.Create<MapBackground>();
                background.Layer = (int)backgroundData[$"{i}", "front"] == 1
                    ? RenderLayer.Foreground
                    : RenderLayer.Background;
                background.Parallax.Type = (BackgroundType)backgroundData[$"{i}", "type"];
                background.Transform.zIndex = 0;
                background.Transform.Position = new Vector2((int)backgroundData[$"{i}", "x"],
                    (int)backgroundData[$"{i}", "y"]);
                background.Transform.Origin = origin;
                background.Parallax.Rx = (int)backgroundData[$"{i}", "rx"];
                background.Parallax.Ry = (int)backgroundData[$"{i}", "ry"];
                background.Parallax.Texture = resource.GetBackground(
                    $"{backgroundData[$"{i}", "bS"]}.img/back/{backgroundData[$"{i}", "no"]}");

                /*switch ((BackgroundType)backgroundData[$"{i}", "type"])
                {
                    case BackgroundType.Default:
                        var background = entity.Create<MapBackground>();
                        background.Layer = (int)backgroundData[$"{i}", "front"] == 1
                            ? RenderLayer.Foreground
                            : RenderLayer.Background;
                        background.Parallax.Type = (BackgroundType)backgroundData[$"{i}", "type"];
                        background.Transform.zIndex = 0;
                        background.Transform.Position = new Vector2((int)backgroundData[$"{i}", "x"],
                            (int)backgroundData[$"{i}", "y"]);
                        background.Transform.Origin = origin;
                        background.Parallax.Rx = (int)backgroundData[$"{i}", "rx"];
                        background.Parallax.Ry = (int)backgroundData[$"{i}", "ry"];
                        background.Parallax.Texture = resource.GetBackground(
                            $"{backgroundData[$"{i}", "bS"]}.img/back/{backgroundData[$"{i}", "no"]}");
                        break;
                    case BackgroundType.HorizontalTiling:
                        break;
                    case BackgroundType.HorizontalScrolling:
                        break;
                    case BackgroundType.HorizontalScrollingHVTiling:
                        break;
                    case BackgroundType.VerticalTiling:
                        break;
                    case BackgroundType.VerticalScrolling:
                        break;
                    case BackgroundType.VerticalScrollingHVTiling:
                        break;
                }*/
            }
            else
            {
                //var animationCount = resource.GetNodeCount($"Map/Back/{bS}.img/ani/{no}");
            }
        }
    }

    private void LoadTile()
    {
        var resource = _locator.GetManager<ResourceManager>();
    }

    private void LoadObj()
    {
        var resource = _locator.GetManager<ResourceManager>();
        var entity = _locator.GetManager<EntityManager>();
        var layer = 0;
        do
        {
            for (int i = 0; i < resource.GetTileCount($"{_worldId}.img/{layer}"); i++)
            {
                using var tileData = resource.LoadMapData($"{_worldId}.img/{layer}/tile");
                if (resource.Contains($"Map/Map/Map{_worldId[0]}/{_worldId}.img/{layer}/info/tS", out var tS))
                    tileData[$"{layer}", "tS"] = (string)tS;
                else
                    tileData[$"{layer}", "tS"] = tileData[$"{0}", "tS"];

                _ = resource.Contains(
                    $"Map/Tile/{tileData[$"{layer}", "tS"]}.img/{tileData[$"{i}", "u"]}/{tileData[$"{i}", "no"]}/z",
                    out var z);
                tileData[$"{i}", "z"] = z;
                var origin = (Vector2)resource.GetOrigin(
                    $"Map/Tile/{tileData[$"{layer}", "tS"]}.img/{tileData[$"{i}", "u"]}/{tileData[$"{i}", "no"]}");
                var tile = entity.Create<MapTile>();
                tile.Layer = (RenderLayer)layer + 1;
                tile.Transform.zIndex = (int)tileData[$"{i}", "z"] +
                                        10 * (3000 * (layer + 1) - (int)tileData[$"{i}", "zM"]) -
                                        1073721834;

                tile.Transform.Position = new Vector2((int)tileData[$"{i}", "x"], (int)tileData[$"{i}", "y"]);
                tile.Transform.Origin = origin;
                tile.Texture = resource.GetTile(
                    $"{tileData[$"{layer}", "tS"]}.img/{tileData[$"{i}", "u"]}/{tileData[$"{i}", "no"]}");
            }

            layer++;
        } while (layer < 8);
    }

    #endregion

    #region Draw/Update Functions

    public void RenderBackground(SpriteBatch spriteBatch, List<IEntity> sorted)
    {
        for (int i = 0; i < sorted.Count; i++)
        {
            if (!(sorted[i] & EntityFlag.Background))
                continue;
            
            var background = sorted[i] as MapBackground;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap,
                DepthStencilState.Default, RasterizerState.CullNone, null, background.Parallax.GetMatrix());
            spriteBatch.Draw(background.Parallax.Texture, background.Transform.Position, null, Color.White,
                0f, background.Transform.Origin, 1f, SpriteEffects.None, 0f);
            spriteBatch.End();
        }
    }

    public void UpdateBackground(IEntity entity)
    {
        if (!(entity & EntityFlag.Background))
            return;
        var background = entity as MapBackground;
        background.Parallax.UpdateMatrix();
    }

    public void RenderTile(SpriteBatch spriteBatch, IEntity entity)
    {
        if (!(entity & EntityFlag.MapTile))
            return;
        var tile = entity as MapTile;
        spriteBatch.Draw(tile.Texture, tile.Transform.Position, null, Color.White,
            0f, tile.Transform.Origin, 1f, SpriteEffects.None, 0f);
    }

    public void RenderObj(SpriteBatch spriteBatch, IEntity obj)
    {

    }

    public void UpdateObj(MapObj obj)
    {

    }

    #endregion
}