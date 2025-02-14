using System.Text.Json;
using MapleSyrup.ECS;
using MapleSyrup.Networking;
using MapleSyrup.Nx;
using MapleSyrup.Resources;
using MapleSyrup.Scenes;
using ZeroElectric.Vinculum;

namespace MapleSyrup.Windowing;

public class GameWindow : IDisposable
{
    private WindowConfig _windowConfig;
    private readonly ResourceFactory _resourceFactory;
    private readonly SceneFactory _sceneFactory;
    private readonly EntityFactory _entityFactory;
    private readonly NXFactory _nxFactory;
    private readonly NetworkClient _client;

    public GameWindow()
    {
        _resourceFactory = new ResourceFactory();
        _sceneFactory = new SceneFactory();
        _entityFactory = new EntityFactory();
        _nxFactory = new NXFactory();
        _client = new NetworkClient();
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
            var jsonString = JsonSerializer.Serialize(config, ConfigContext.Default.WindowConfig);
            File.WriteAllText("window_config.json", jsonString);
            _windowConfig = config;
        }
        else
        {
            var jsonString = File.ReadAllText("window_config.json");
            _windowConfig = JsonSerializer.Deserialize(jsonString, ConfigContext.Default.WindowConfig) ?? throw new Exception();
        }
    }

    private void Initialize()
    {
        _client.ConnectAsync();
    }

    private void LoadContent()
    {
        LoadConfig();
    }

    private void UnloadContent()
    {
        _client.Stop();
        _sceneFactory.Shutdown();
        _resourceFactory.ShutDown();
        _entityFactory.Shutdown();
        _nxFactory.Shutdown();
    }

    public void Run()
    {
        Initialize();
        LoadContent();
        
        Raylib.InitWindow(_windowConfig.Width, _windowConfig.Height, _windowConfig.Title);
        Raylib.SetTargetFPS(30);
        Raylib.SetTraceLogLevel((int)TraceLogLevel.LOG_NONE);

        while (!Raylib.WindowShouldClose())
        {
            if (!_sceneFactory.SceneReady) continue;
            _resourceFactory.LoadPendingTextures();
            
            var frameTime = Raylib.GetFrameTime() * 1000;
            _sceneFactory.Scene.Update(frameTime);
            
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.GRAY);
            Raylib.BeginMode2D(_sceneFactory.Scene.Camera);
            _sceneFactory.Scene.Draw();
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