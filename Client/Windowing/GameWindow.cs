using System.Text.Json;
using Client.ECS;
using Client.Nx;
using Client.Resources;
using Client.Scenes;
using ZeroElectric.Vinculum;

namespace Client.Windowing;

public class GameWindow : IDisposable
{
    private WindowConfig _windowConfig;
    private readonly ResourceFactory _resourceFactory;
    private readonly SceneFactory _sceneFactory;
    private readonly EntityFactory _entityFactory;
    private readonly NXFactory _nxFactory;
    private SceneBase _mainSceneBase;

    public GameWindow()
    {
        _resourceFactory = new ResourceFactory();
        _sceneFactory = new SceneFactory(this);
        _entityFactory = new EntityFactory();
        _nxFactory = new NXFactory();
    }
    
    private void LoadConfig()
    {
        if (!File.Exists("config.json"))
        {
            var config = new WindowConfig()
            {
                Width = 1280,
                Height = 720,
                Title = "MapleSyrup",
                Fullscreen = false
            };
            var jsonString = JsonSerializer.Serialize(config);
            File.WriteAllText("window_config.json", jsonString);
            _windowConfig = config;
        }
        else
        {
            var jsonString = File.ReadAllText("window_config.json");
            _windowConfig = JsonSerializer.Deserialize<WindowConfig>(jsonString) ?? throw new Exception();
        }
    }

    private void Initialize()
    {
        LoadConfig();
        Raylib.InitWindow(_windowConfig.Width, _windowConfig.Height, _windowConfig.Title);
        Raylib.SetTargetFPS(30);
        Raylib.SetTraceLogLevel((int)TraceLogLevel.LOG_NONE);
    }

    private void LoadContent()
    {
        _mainSceneBase = _sceneFactory.CreateScene("100000000");
        _mainSceneBase.InitSystems();
        _mainSceneBase.LoadContent();
    }

    private void UnloadContent()
    {
        _mainSceneBase.Shutdown();
        _resourceFactory.ShutDown();
        _entityFactory.Shutdown();
        _nxFactory.Shutdown();
    }

    public void Run()
    {
        Initialize();
        LoadContent();

        while (!Raylib.WindowShouldClose())
        {
            var frameTime = Raylib.GetFrameTime() * 1000;
            _mainSceneBase.Update(frameTime);
            
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.GRAY);
            Raylib.BeginMode2D(_mainSceneBase.Camera);
            _mainSceneBase.Draw();
            Raylib.EndMode2D();
            Raylib.DrawFPS(0, 0);
            Raylib.EndDrawing();
        }
        
        UnloadContent();
        Raylib.CloseWindow();
    }
    
    public void Dispose()
    {
        // TODO release managed resources here
        GC.SuppressFinalize(this);
    }
}