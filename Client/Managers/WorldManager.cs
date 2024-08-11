using System.Numerics;
using Client.Actors;
using Client.Map;
using Client.Net;
using Client.Scene;
using Raylib_CsLo;

namespace Client.Managers;

public class WorldManager : IManager
{
    private IWorld? _world;
    private bool _isLoaded;
    private MapLoader _loader;
    private WorldState _state;
    private WorldInfo _info;

    public WorldState State => _state;
    public WorldInfo WorldInfo => _info;
    
    public Texture Minimap { get; internal set; }
    public Vector2 WorldSize { get; internal set; }
    public Rectangle WorldBounds { get; internal set; }
    

    public void Initialize()
    {
        _isLoaded = false;
        _loader = new();
    }

    public void Shutdown()
    {
        _world?.Clear();
    }

    public void SetState(WorldState state)
    {
        if (_state == state) return;
        _state = state;
    }

    public void CreateLogin()
    {
        _world = new Login();
        var uiNode = ServiceLocator.Get<NxManager>().GetNode("MapLogin.img");
        _info = new WorldInfo(uiNode["info"]);
        if (uiNode?.Has("miniMap") ?? false)
        {
            Minimap = uiNode.GetTexture("canvas");
            WorldSize = new Vector2(uiNode["miniMap"].GetInt("width"),
                uiNode["miniMap"].GetInt("height"));
            WorldBounds = new Rectangle(WorldSize.X / 2f, WorldSize.Y / 2f, WorldSize.X, WorldSize.Y);
        }
        else
        {
            Minimap = new Texture() { width = 1, height = 1 };
            if (WorldInfo.VRTop != 0)
            {
                WorldBounds = new Rectangle(WorldInfo.VRLeft, WorldInfo.VRTop, WorldInfo.VRRight - WorldInfo.VRLeft,
                    WorldInfo.VRBottom - WorldInfo.VRTop);
            }
        }
        _isLoaded = _loader.Load(uiNode);
        _world.Load();
    }

    public void CreateWorld(int id)
    {
        // TODO: Fade.
        var strId = GetMapId(id);
        _world?.Clear();
        _world = null;
        var node = ServiceLocator.Get<NxManager>().GetNode($"Map{strId[0]}/{strId}.img");
        _info = new WorldInfo(node["info"]);
        _isLoaded = _loader.Load(node);
    }

    public IWorld GetWorld()
    {
        return _world ?? throw new Exception("World is null");
    }

    public Camera2D GetCamera()
    {
        return _world?.Camera ?? throw new Exception("World is null");;
    }

    public void UpdateCamera(Vector2 position)
    {
        _world?.UpdateCamera(position);
    }

    public void UpdateZoom(float zoom)
    {
        _world.UpdateZoom(zoom);
    }

    public void ProcessPackets(InPacket response)
    {
        _world?.ProcessPacket(response);
    }

    public string GetMapId(int id)
    {
        return id.ToString().PadLeft(9, '0');
    }
}