using System.Text.Json.Nodes;
using MapleSyrup.ECS;
using MapleSyrup.Nx;
using MapleSyrup.Scenes.Map;
using MapleSyrup.Windowing;

namespace MapleSyrup.Scenes;

public class SceneFactory
{
    private SceneBase _scene;
    private bool _dirty = false;

    public SceneBase Scene => _scene;
    public bool SceneReady { get; private set; } = false;
    public static SceneFactory Shared { get; private set; }
    
    public SceneFactory()
    {
        Shared = this;
    }

    public void CreateScene<T>(string sceneName, MapleMap map) where T : SceneBase
    {
        lock (this)
        {
            if (_dirty)
                Shutdown();

            var type = typeof(T);
            if (type == typeof(LoginScene))
            {
                _scene = new LoginScene(sceneName);
                _scene.InitSystems();
                _scene.LoadContent(map);
            }
            else if (type == typeof(WorldScene))
            {
                _scene = new WorldScene(sceneName);
            }
            else
            {
                throw new Exception($"Unknown scene type: {type}");
            }

            _dirty = true;
            SceneReady = true;
        }
    }

    public void Shutdown()
    {
        _scene.Shutdown();
        _dirty = false;
    }
}