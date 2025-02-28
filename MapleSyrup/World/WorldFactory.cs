using MapleSyrup.Common.Map;

namespace MapleSyrup.World;

public class WorldFactory
{
    private WorldBase _world;
    private bool _dirty = false;

    public WorldBase World => _world;
    public bool SceneReady { get; private set; } = false;
    public static WorldFactory Shared { get; private set; }
    
    public WorldFactory()
    {
        Shared = this;
    }

    public void CreateScene<T>(MapleMap map) where T : WorldBase
    {
        lock (this)
        {
            if (_dirty)
                Shutdown();
            var scene = Activator.CreateInstance<T>();
            scene.InitSystems();
            scene.LoadContent(map);
            _world = scene;
            _dirty = true;
            SceneReady = true;
        }
    }

    public void ChangeScene<T>(MapleMap map) where T : WorldBase
    {
        SceneReady = false;
        _world.Shutdown();
        CreateScene<T>(map);
    }

    public void Shutdown()
    {
        _world.Shutdown();
        _dirty = false;
    }
}