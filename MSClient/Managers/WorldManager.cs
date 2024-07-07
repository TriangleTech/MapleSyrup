using MSClient.Scene;

namespace MSClient.Managers;

public class WorldManager : IManager
{
    private World? _world;

    public void Initialize()
    {
        _world = null;
    }

    public void Shutdown()
    {
        _world?.Clear();
    }

    public void CreateWorld(string img, bool isLogin = false)
    {
         // TODO: Fade.
        if (_world != null)
            _world.Clear();
        
        if (isLogin)
        {
            var ui = ServiceLocator.Get<NxManager>().Get(MapleFiles.UI);
            var uiNode = ui.GetNode("MapLogin.img");
            var map = ServiceLocator.Get<NxManager>().Get(MapleFiles.Map);
            var mapNode = map.GetNode($"Map{img[0]}/{img}.img");
            
            _world = new World(ref mapNode, ref uiNode);
            _world.Load();
            ServiceLocator.Get<ActorManager>().AddActor(_world);
        }
        else
        {
            var nx = ServiceLocator.Get<NxManager>().Get(MapleFiles.Map);
            var node = nx.GetNode($"Map{img[0]}/{img}.img");
            _world = new World(ref node);
            _world.Load();
            //ServiceLocator.Get<ActorManager>().AddActor(_world); // Do not do this...
        }
    }

    public World? GetWorld()
    {
        return _world;
    }
}