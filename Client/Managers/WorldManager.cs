using System.Numerics;
using Client.Actors;
using Client.Map;
using Client.Scene;
using Raylib_CsLo;

namespace Client.Managers;

public class WorldManager : IManager
{
     private World? _world;
    private bool _isLoaded;
    private MapLoader _loader;
    private WorldState _state;

    public void Initialize()
    {
        _world = null;
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
        var ui = ServiceLocator.Get<NxManager>().Get(MapleFiles.UI);
        var uiNode = ui.GetNode("MapLogin.img");
        _isLoaded = _loader.Load(uiNode);

        LoadLoginComponents();
        
        if (_isLoaded)
            _world = new World(true);
    }

    private void LoadLoginComponents()
    {
        var actor = ServiceLocator.Get<ActorManager>();
        var ui = ServiceLocator.Get<NxManager>().Get(MapleFiles.UI);
        var uiNode = ui.GetNode("Login.img");
        
        // Load Frame
        {
            var frame = uiNode["Common"];
            var origin = frame["frame"].GetVector("origin");
            actor.Create(
                new MapObject(frame, frame.GetTexture("frame"), Vector2.Zero, origin, ActorLayer.Foreground, 0));
        }
        
        // Load Login Location
        {
            var loginLocation = uiNode["Common"]["loginlocation"];
            actor.Create(
                new MapObject(loginLocation, loginLocation.GetTexture("0"), Vector2.Zero, 
                    loginLocation["0"].GetVector("origin"), ActorLayer.Effects, 0));
            actor.Create(
                new MapObject(loginLocation, loginLocation.GetTexture("1"), Vector2.Zero,
                    loginLocation["1"].GetVector("origin"), ActorLayer.Effects, 1));
        }
        
        actor.SortAll();
    }

    public void CreateWorld(string img)
    {
        // TODO: Fade.
        if (_world != null)
            _world.Clear();

        var nx = ServiceLocator.Get<NxManager>().Get(MapleFiles.Map);
        var node = nx.GetNode($"Map{img[0]}/{img}.img");
        _isLoaded = _loader.Load(node);
        if (_isLoaded)
            _world = new World(false);    
    }

    public World? GetWorld()
    {
        return _world;
    }

    public Camera2D GetCamera()
    {
        if (_world == null) throw new Exception("World is null");
        return _world.Camera;
    }

    public void UpdateCamera(Vector2 position)
    {
        if (_world == null) return;
        
        _world.Camera.target = Vector2.Lerp(_world.Camera.target, position, 0.2f);
    }
}