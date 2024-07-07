using System.Diagnostics;
using System.Runtime.CompilerServices;
using MapleSyrup.EC;
using MapleSyrup.EC.Components;
using MapleSyrup.Event;
using MapleSyrup.Managers;
using MapleSyrup.Player;
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void LoadBackground()
    {
        var resource = _locator.GetManager<ResourceManager>();
        var entity = _locator.GetManager<EntityManager>();

        for (int i = 0; i < resource.GetNodeCount("Map",$"Map/Map{_worldId[0]}/{_worldId}.img"); i++)
        {
            var no = resource.GetInt("Map", $"Map/Map{_worldId[0]}/{_worldId}.img/back/{i}/no");
            var x = resource.GetInt("Map", $"Map/Map{_worldId[0]}/{_worldId}.img/back/{i}/x");;
            var y = resource.GetInt("Map", $"Map/Map{_worldId[0]}/{_worldId}.img/back/{i}/y");;
            var rx = resource.GetInt("Map", $"Map/Map{_worldId[0]}/{_worldId}.img/back/{i}/rx");;
            var ry = resource.GetInt("Map", $"Map/Map{_worldId[0]}/{_worldId}.img/back/{i}/ry");;
            var type = resource.GetInt("Map", $"Map/Map{_worldId[0]}/{_worldId}.img/back/{i}/type");;
            var cx = resource.GetInt("Map", $"Map/Map{_worldId[0]}/{_worldId}.img/back/{i}/cx");;
            var cy = resource.GetInt("Map", $"Map/Map{_worldId[0]}/{_worldId}.img/back/{i}/cy");;
            var bS = resource.GetString("Map", $"Map/Map{_worldId[0]}/{_worldId}.img/back/{i}/bS");
            var a = resource.GetInt("Map", $"Map/Map{_worldId[0]}/{_worldId}.img/back/{i}/a");;
            var front = resource.GetInt("Map", $"Map/Map{_worldId[0]}/{_worldId}.img/back/{i}/front");;
            var ani = resource.GetInt("Map", $"Map/Map{_worldId[0]}/{_worldId}.img/back/{i}/ani");;
            var f = resource.GetInt("Map", $"Map/Map{_worldId[0]}/{_worldId}.img/back/{i}/f");;
            
            if (bS == string.Empty)
                continue;

            if (ani == 0)
            {
                var origin = resource.GetVector("Map", $"Back/{bS}.img/back/{no}");
                var background = entity.Create<MapBackground>();
                background.Layer = front == 1
                    ? RenderLayer.Foreground
                    : RenderLayer.Background;
                background.Parallax.Type = (BackgroundType)type;
                background.Transform.zIndex = 0;
                background.Transform.Position = new Vector2((int)x,
                    y);
                background.Transform.Origin = origin;
                background.Parallax.Rx = rx;
                background.Parallax.Ry = ry;
                background.Texture = resource.GetTexture("Map", $"Back/{bS}.img/back/{no}");
            }
            else
            {
                
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void LoadTile()
    {
        var resource = _locator.GetManager<ResourceManager>();
        var entity = _locator.GetManager<EntityManager>();
        var layer = 0;
        var tS = resource.GetString("Map", $"Map/Map{_worldId[0]}/{_worldId}.img/0/info/tS");
        do
        {
            var path = $"Map/Map{_worldId[0]}/{_worldId}.img/{layer}/tile";
            for (int i = 0; i < resource.GetNodeCount("Map", path); i++)
            {
                var tSVal = resource.GetString("Map", $"Map/Map{_worldId[0]}/{_worldId}.img/{layer}/info/tS");
                if (tSVal != string.Empty && layer != 0) 
                    tS = tSVal;

                var x = resource.GetInt("Map", $"{path}/{i}/x");
                var y = resource.GetInt("Map", $"{path}/{i}/y");
                var u = resource.GetString("Map", $"{path}/{i}/u");
                var no = resource.GetInt("Map", $"{path}/{i}/no");
                var zM = resource.GetInt("Map", $"{path}/{i}/zM");
                var tilePath = $"Tile/{tS}.img/{u}/{no}";
                var z = resource.GetInt("Map", $"{tilePath}/z");
                var origin = resource.GetVector("Map", $"{tilePath}/origin");
                var tile = entity.Create<MapTile>();
                tile.Layer = (RenderLayer)layer + 1;
                tile.Transform.zIndex = z + 10 * (3000 * (layer + 1) - zM) - 1073721834;
                tile.Transform.Position = new Vector2(x, y);
                tile.Transform.Origin = origin;
                tile.Texture = resource.GetTexture("Map", tilePath);
            }

            layer++;
        } while (layer < 8);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void LoadObj()
    {
        var layer = 0;
        var resource = _locator.GetManager<ResourceManager>();
        var entity = _locator.GetManager<EntityManager>();
        do
        {
            var path = $"Map/Map{_worldId[0]}/{_worldId}.img/{layer}/obj";
            for (int i = 0; i < resource.GetNodeCount("Map", path); i++)
            {
                var oS = resource.GetString("Map", $"{path}/{i}/oS");
                var l0 = resource.GetString("Map", $"{path}/{i}/l0");
                var l1 = resource.GetString("Map", $"{path}/{i}/l1");
                var l2 = resource.GetString("Map", $"{path}/{i}/l2");
                var x = resource.GetInt("Map", $"{path}/{i}/x");
                var y = resource.GetInt("Map", $"{path}/{i}/y");
                var z = resource.GetInt("Map", $"{path}/{i}/z");
                var f = resource.GetInt("Map", $"{path}/{i}/f");
                var zM = resource.GetInt("Map", $"{path}/{i}/zM");

                var objPath = $"Obj/{oS}.img/{l0}/{l1}/{l2}";
                var nodeCount = resource.GetNodeCount("Map", objPath);

                var obj = entity.Create<MapObj>();
                obj.Layer = (RenderLayer)layer + 1;
                obj.Transform.zIndex = (30000 * layer + z) - 1073739824;
                obj.Transform.Position = new Vector2(x, y);

                if (nodeCount == 1)
                {
                    var origin = (Vector2)resource.GetVector("Map", $"{objPath}/0/origin");
                    obj.Transform.Origin = origin;
                    obj.Texture = resource.GetTexture("Map", $"{objPath}/0");
                }
                else
                {
                    /*
                    // TODO: Handle Obstacles
                    if (resource.Contains(
                            $"Map/Obj/{objData[$"{i}", "oS"]}.img/{objData[$"{i}", "l0"]}/{objData[$"{i}", "l1"]}/{objData[$"{i}", "l2"]}/obstacle",
                            out _))
                        entity.Remove(obj);
                    else
                    {
                        LoadAnimatedObject(ref obj, nodeCount, i, objData);
                    }*/
                    entity.Remove(obj);
                }
            }

            layer++;
        } while (layer < 8);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void LoadAnimatedObject(ref MapObj obj, int nodeCount, int increment, VariantMap<string, string, object> objData)
    {
        /*var i = increment;
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
            }
        }*/
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void LoadPortals()
    {
        var resource = _locator.GetManager<ResourceManager>();
        var entity = _locator.GetManager<EntityManager>();
        var worldPath = $"Map/Map{_worldId[0]}/{_worldId}.img/portal";
        var portalCount = resource.GetNodeCount("Map", $"{worldPath}");

        for (int i = 0; i < portalCount; i++)
        {
            var name = resource.GetString("Map", $"{worldPath}/{i}/pn");
            var portalType = resource.GetInt("Map", $"{worldPath}/{i}/pt");
            // script was previously here
            var x = resource.GetInt("Map", $"{worldPath}/{i}/x");
            var y = resource.GetInt("Map", $"{worldPath}/{i}/y");
            var targetMap = resource.GetInt("Map", $"{worldPath}/{i}/tm"); // The map it leads to
            var targetPortal = resource.GetString("Map", $"{worldPath}/{i}/tn"); // The portal you end up on
            var portal = entity.Create<Portal>();
            portal.Name = name;
            portal.Script = "";
            portal.TargetMap = targetMap;
            portal.TargetPortal = targetPortal;
            portal.PortalId = i;
            portal.Transform.Position = new(x, y);
            portal.Transform.Origin = resource.GetVector("Map", $"MapHelper.img/portal/game/pv/0");
            portal.Layer = RenderLayer.Foreground;
            portal.Animation.AddDelay(100);

            switch (portalType)
            {
                case 2:
                case 4:
                case 7:
                    portal.Type = PortalType.Visible;
                    portal.Texture = resource.GetTexture("Map", "MapHelper.img/portal/game/pv/0");
                    for (var j = 0; j < 8; j++)
                    {
                        var origin = (Vector2)resource.GetVector("Map", $"MapHelper.img/portal/game/pv/{j}/origin");
                        portal.Animation.AddFrame(portal.Transform.Position, origin, resource.GetTexture("Map", $"MapHelper.img/portal/game/pv/{j}"));
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
                DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
            spriteBatch.Draw(background.Texture, background.Transform.Position, null, Color.White,
                0f, background.Transform.Origin, 1f, SpriteEffects.None, 0f);
            spriteBatch.End();
        }
    }

    public void UpdateBackground(IEntity entity, CameraComponent camera)
    {
        if (!(entity & EntityFlag.Background))
            return;
        var background = entity as MapBackground;
        background.Parallax.UpdateMatrix(camera);
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