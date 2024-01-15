using System.Diagnostics;
using MapleSyrup.EC;
using MapleSyrup.EC.Components;
using MapleSyrup.Event;
using MapleSyrup.Managers;
using MapleSyrup.Utilities;
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
        LoadPortals();
        
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
                background.Texture = resource.GetBackground(
                    $"{backgroundData[$"{i}", "bS"]}.img/back/{backgroundData[$"{i}", "no"]}");
            }
            else
            {
                
            }
        }
    }

    private void LoadTile()
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

    private void LoadObj()
    {
        var layer = 0;
        var resource = _locator.GetManager<ResourceManager>();
        var entity = _locator.GetManager<EntityManager>();
        do
        {
            for (int i = 0; i < resource.GetObjectCount($"{_worldId}.img/{layer}"); i++)
            {
                using var objData = resource.LoadMapData($"{_worldId}.img/{layer}/obj");
                var nodeCount =
                    resource.GetNodeCount(
                        $"Map/Obj/{objData[$"{i}", "oS"]}.img/{objData[$"{i}", "l0"]}/{objData[$"{i}", "l1"]}/{objData[$"{i}", "l2"]}");

                var x = (int)objData[$"{i}", "x"];
                var y = (int)objData[$"{i}", "y"];
                var z = (int)objData[$"{i}", "z"];

                var obj = entity.Create<MapObj>();
                obj.Layer = (RenderLayer)layer + 1;
                obj.Transform.zIndex = (int)(30000 * layer + z) - 1073739824;
                obj.Transform.Position = new Vector2(x, y);

                if (objData.Contains($"{i}", "r"))
                    obj.Flags &= ~EntityFlag.Active;

                if (nodeCount == 1)
                {
                    var origin = (Vector2)resource.GetOrigin(
                        $"Map/Obj/{objData[$"{i}", "oS"]}.img/{objData[$"{i}", "l0"]}/{objData[$"{i}", "l1"]}/{objData[$"{i}", "l2"]}/0");
                    obj.Transform.Origin = origin;
                    obj.Texture = resource.GetMapObject(
                        $"{objData[$"{i}", "oS"]}.img/{objData[$"{i}", "l0"]}/{objData[$"{i}", "l1"]}/{objData[$"{i}", "l2"]}/0");
                }
                else
                {
                    // TODO: Handle Obstacles
                    if (resource.Contains(
                            $"Map/Obj/{objData[$"{i}", "oS"]}.img/{objData[$"{i}", "l0"]}/{objData[$"{i}", "l1"]}/{objData[$"{i}", "l2"]}/obstacle",
                            out _))
                        entity.Remove(obj);
                    else
                    {
                        LoadAnimatedObject(ref obj, nodeCount, i, objData);
                    }
                }
            }

            layer++;
        } while (layer < 8);
    }

    private void LoadAnimatedObject(ref MapObj obj, int nodeCount, int increment, VariantMap<string, string, object> objData)
    {
        var i = increment;
        var resource = _locator.GetManager<ResourceManager>();
        var entity = _locator.GetManager<EntityManager>();
        var x = (int)objData[$"{i}", "x"];
        var y = (int)objData[$"{i}", "y"];
        var firstOrigin = (Vector2)resource.GetOrigin(
            $"Map/Obj/{objData[$"{i}", "oS"]}.img/{objData[$"{i}", "l0"]}/{objData[$"{i}", "l1"]}/{objData[$"{i}", "l2"]}/0");
        obj.Transform.Position = new Vector2(x, y);
        obj.Transform.Origin = firstOrigin;
        obj.Flags |= EntityFlag.AniObj;
        obj.CFlags |= ComponentFlag.Animation;
        obj.Animation = new AnimationComponent(obj);

        // TODO: Handle seats
        if (resource.Contains(
                $"Map/Obj/{objData[$"{i}", "oS"]}.img/{objData[$"{i}", "l0"]}/{objData[$"{i}", "l1"]}/{objData[$"{i}", "l2"]}/seat",
                out _))
        {
            entity.Remove(obj);
            return;
        }

        // TODO: Handle blend 
        if (resource.Contains(
                $"Map/Obj/{objData[$"{i}", "oS"]}.img/{objData[$"{i}", "l0"]}/{objData[$"{i}", "l1"]}/{objData[$"{i}", "l2"]}/blend",
                out _))
        {
            entity.Remove(obj);
            return;
        }

        // Frame Animation
        if (!resource.Contains(
                $"Map/Obj/{objData[$"{i}", "oS"]}.img/{objData[$"{i}", "l0"]}/{objData[$"{i}", "l1"]}/{objData[$"{i}", "l2"]}/0/a0",
                out _))
        {
            obj.Texture = resource.GetMapObject(
                $"{objData[$"{i}", "oS"]}.img/{objData[$"{i}", "l0"]}/{objData[$"{i}", "l1"]}/{objData[$"{i}", "l2"]}/0");
           
            for (int j = 0; j < nodeCount; j++)
            {
                var origin = (Vector2)resource.GetOrigin(
                    $"Map/Obj/{objData[$"{i}", "oS"]}.img/{objData[$"{i}", "l0"]}/{objData[$"{i}", "l1"]}/{objData[$"{i}", "l2"]}/{j}");
                obj.Animation.AddFrame(new Vector2(x, y), origin, resource.GetMapObject(
                    $"{objData[$"{i}", "oS"]}.img/{objData[$"{i}", "l0"]}/{objData[$"{i}", "l1"]}/{objData[$"{i}", "l2"]}/{j}"));
                
                if (resource.Contains(
                        $"Map/Obj/{objData[$"{i}", "oS"]}.img/{objData[$"{i}", "l0"]}/{objData[$"{i}", "l1"]}/{objData[$"{i}", "l2"]}/{j}/delay",
                        out var delay))
                {
                    obj.Animation.AddDelay((int)delay);
                }
                else
                {
                    obj.Animation.AddDelay(100);
                    Debug.WriteLine("Delay not found, using default of 100");
                }
            }
        }

        // Blend Animation
        if (resource.Contains(
                $"Map/Obj/{objData[$"{i}", "oS"]}.img/{objData[$"{i}", "l0"]}/{objData[$"{i}", "l1"]}/{objData[$"{i}", "l2"]}/0/a0",
                out _))
        {
            entity.Remove(obj);
            /*
            obj.AddComponent(new BlendAnimation());
            var origin = (Vector2)resource.GetOrigin(
                $"Map/Obj/{objData[$"{i}", "oS"]}.img/{objData[$"{i}", "l0"]}/{objData[$"{i}", "l1"]}/{objData[$"{i}", "l2"]}/0");
            var blend = obj.GetComponent<BlendAnimation>();
            transform.Origin = origin;
            transform.Position = new Vector2(x, y);
            for (int j = 0; j < nodeCount; j++)
            {
                _ = resource.Contains(
                    $"Map/Obj/{objData[$"{i}", "oS"]}.img/{objData[$"{i}", "l0"]}/{objData[$"{i}", "l1"]}/{objData[$"{i}", "l2"]}/{j}/delay",
                    out var a0);
                blend.Frames.Add(resource.GetMapObject(
                    $"{objData[$"{i}", "oS"]}.img/{objData[$"{i}", "l0"]}/{objData[$"{i}", "l1"]}/{objData[$"{i}", "l2"]}/{j}"));
                blend.Alpha.Add((byte)(int)a0);
                if (resource.Contains(
                        $"Map/Obj/{objData[$"{i}", "oS"]}.img/{objData[$"{i}", "l0"]}/{objData[$"{i}", "l1"]}/{objData[$"{i}", "l2"]}/{j}/a1",
                        out var a1))
                    blend.Alpha.Add((byte)(int)a1);
                if (resource.Contains(
                        $"Map/Obj/{objData[$"{i}", "oS"]}.img/{objData[$"{i}", "l0"]}/{objData[$"{i}", "l1"]}/{objData[$"{i}", "l2"]}/{j}/delay",
                        out var delay))
                    blend.Delay.Add((int)delay);
            }*/
        }
    }

    private void LoadPortals()
    {
        var resource = _locator.GetManager<ResourceManager>();
        var entity = _locator.GetManager<EntityManager>();
        var worldPath = $"Map/Map/Map{_worldId[0]}/{_worldId}.img";
        var portalCount = resource.GetNodeCount($"{worldPath}/portal");
        using var portalData = resource.LoadMapData($"{_worldId}.img/portal");

        for (int i = 0; i < portalCount; i++)
        {
            var name = (string)portalData[$"{i}", "pn"];
            var portalType = (int)portalData[$"{i}", "pt"];
            // script was previously here
            var x = (int)portalData[$"{i}", "x"];
            var y = (int)portalData[$"{i}", "y"];
            var targetMap = (int)portalData[$"{i}", "tm"];; // The map it leads to
            var targetPortal = (string)portalData[$"{i}", "tn"];; // The portal you end up on
            var portal = entity.Create<Portal>();
            portal.Name = name;
            portal.Script = "";
            portal.TargetMap = targetMap;
            portal.TargetPortal = targetPortal;
            portal.PortalId = i;
            portal.Transform.Position = new(x, y);
            portal.Transform.Origin = (Vector2)resource.GetOrigin($"Map/MapHelper.img/portal/game/pv/0");
            portal.Layer = RenderLayer.Foreground;
            portal.Animation.AddDelay(100);

            switch (portalType)
            {
                case 2:
                case 4:
                case 7:
                    portal.Type = PortalType.Visible;
                    portal.Texture = resource.GetPortal($"pv/0");
                    for (var j = 0; j < 8; j++)
                    {
                        var origin = (Vector2)resource.GetOrigin($"Map/MapHelper.img/portal/game/pv/{j}");
                        portal.Animation.AddFrame(portal.Transform.Position, origin, resource.GetPortal($"pv/{j}"));
                        portal.Animation.AddDelay(100);
                    }
                    break;
                case 0:
                case 1:
                case 3:
                case 9:
                case 0x0C:
                case 0x0D:
                    portal.Type = PortalType.Hidden;
                    portal.Flags &= ~EntityFlag.Active; // TODO: Change this so it has a hidden flag
                    entity.Remove(portal);
                    break;
                case 6: // wtf is this??? In map 100000000 it spawns 4 random portals
                case 10:
                case 0x0B:
                    portal.Type = PortalType.ScriptedHidden;
                    portal.Flags &= ~EntityFlag.Active; // TODO: Change this so it has a scripted hidden flag
                    entity.Remove(portal);
                    break;
                default:
                    Console.WriteLine($"Unknown Portal Type Detected: {portalType}");
                    break;
            }
        }
    }

    #endregion

    #region Draw/Update Functions

    public void RenderBackground(SpriteBatch spriteBatch, List<IEntity> sorted)
    {
        for (int i = 0; i < sorted.Count; i++)
        {
            if (!(sorted[i] & EntityFlag.Background) || !(sorted[i] & EntityFlag.Active))
                continue;
            
            var background = sorted[i] as MapBackground;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap,
                DepthStencilState.Default, RasterizerState.CullNone, null, background.Parallax.GetMatrix());
            spriteBatch.Draw(background.Texture, background.Transform.Position, null, Color.White,
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
        if (!(entity & EntityFlag.MapTile) || !(entity & EntityFlag.Active))
            return;
        var tile = entity as MapTile;
        spriteBatch.Draw(tile.Texture, tile.Transform.Position, null, Color.White,
            0f, tile.Transform.Origin, 1f, SpriteEffects.None, 0f);
    }

    public void RenderObj(SpriteBatch spriteBatch, IEntity entity)
    {
        if (!(entity & EntityFlag.MapObject) || !(entity & EntityFlag.Active))
            return;
        var obj = entity as MapObj;
        spriteBatch.Draw(obj.Texture, obj.Transform.Position, null, Color.White,
            0f, obj.Transform.Origin, 1f, SpriteEffects.None, 0f);
    }

    public void UpdateObj(IEntity entity, GameTime gameTime)
    {
        if (!(entity & EntityFlag.AniObj) || !(entity & EntityFlag.Active))
            return;
        var ani = entity as MapObj;
        
        // This is where the CFlags come in handy, to check what component is inside of it without
        // having to iterate the entire component multiple times a frame.
        if (entity & ComponentFlag.Animation)
        {
            if (ani.Animation.Advance(gameTime.ElapsedGameTime.Milliseconds))
                ani.Animation.NextFrame();
        }
    }

    public void RenderPortal(SpriteBatch spriteBatch, IEntity entity)
    {
        if (!(entity & EntityFlag.Portal) || !(entity & EntityFlag.Active))
            return;
        var portal = entity as Portal;
        spriteBatch.Draw(portal.Texture, portal.Transform.Position, null, Color.White,
            0f, portal.Transform.Origin, 1f, SpriteEffects.None, 0f);
    }

    public void UpdatePortal(IEntity entity, GameTime gameTime)
    {
        if (!(entity & EntityFlag.Portal) || !(entity & EntityFlag.Active))
            return;
        
        var ani = entity as Portal;
        if (ani.Animation.Advance(gameTime.ElapsedGameTime.Milliseconds))
            ani.Animation.NextFrame();
    }

    #endregion
}