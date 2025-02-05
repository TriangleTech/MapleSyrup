using Client.ECS;
using Client.Nx;
using Client.Windowing;

namespace Client.Scenes;

public class SceneFactory
{
    private GameWindow _gameWindow;
    public static SceneFactory Shared { get; private set; }
    
    public SceneFactory(GameWindow gameWindow)
    {
        _gameWindow = gameWindow;
        Shared = this;
    }
    
    public WorldScene CreateScene(string sceneName)
    {
        return new WorldScene(sceneName);
    }

    public LoginScene CreateLogin(string sceneName)
    {
        return new LoginScene(sceneName);
    }
}