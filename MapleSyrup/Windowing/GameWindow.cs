using System.Numerics;
using System.Text.Json;
using MapleSyrup.ECS;
using MapleSyrup.ECS.Components.Common;
using MapleSyrup.Networking;
using MapleSyrup.Physics.Components;
using MapleSyrup.Resources;
using MapleSyrup.Tests.Components;
using MapleSyrup.World;
using ZeroElectric.Vinculum;

namespace MapleSyrup.Windowing;

public class GameWindow : IDisposable
{
    private WindowConfig _windowConfig;
    private readonly ResourceFactory _resourceFactory;
    private readonly WorldFactory _worldFactory;
    private readonly EntityFactory _entityFactory;
    private readonly NetworkClient _client;

    public GameWindow()
    {
        _resourceFactory = new ResourceFactory();
        _worldFactory = new WorldFactory();
        _entityFactory = new EntityFactory();
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
            var jsonString = JsonSerializer.Serialize(config, WindowConfigContext.Default.WindowConfig);
            File.WriteAllText("window_config.json", jsonString);
            _windowConfig = config;
        }
        else
        {
            var jsonString = File.ReadAllText("window_config.json");
            _windowConfig = JsonSerializer.Deserialize(jsonString, WindowConfigContext.Default.WindowConfig) ?? throw new Exception();
        }
    }

    private void Initialize()
    {
        //Task.Factory.StartNew(() => _client.ConnectAsync());
    }

    private void LoadContent()
    {
        LoadConfig();
    }

    private void UnloadContent()
    {
        _client.Stop();
        _worldFactory.Shutdown();
        _resourceFactory.ShutDown();
        _entityFactory.Shutdown();
    }

    public void Run()
    {
        Initialize();
        LoadContent();

        Raylib.InitWindow(_windowConfig.Width, _windowConfig.Height, _windowConfig.Title);
        Raylib.SetTargetFPS(30);
        Raylib.SetTraceLogLevel((int)TraceLogLevel.LOG_NONE);

        var mapdata = _resourceFactory.LoadMapData("100000000");
        _worldFactory.CreateScene<MainWorld>(mapdata);

        while (!Raylib.WindowShouldClose())
        {
            if (!_worldFactory.SceneReady) continue;
            _resourceFactory.LoadPendingTextures();
            _entityFactory.ProcessPending();

            var frameTime = Raylib.GetFrameTime() * 1000;
            _worldFactory.World.Update(frameTime);

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.GRAY);
            Raylib.BeginMode2D(_worldFactory.World.Camera);
            _worldFactory.World.Draw();
            Raylib.EndMode2D();
            Raylib.DrawFPS(0, 0);
            Raylib.EndDrawing();

            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_RIGHT))
            {
                var entity = _entityFactory.CreateEntity(0, "TestBox", "Test");
                var transform = _entityFactory.GetComponent<TransformComponent>(entity.Id);
                transform.Position = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), _worldFactory.World.Camera);
                var square = new RedSquare()
                {
                    Owner = entity.Id,
                    Bounds = new Rectangle(transform.Position.X, transform.Position.Y, 50, 50)
                };
                var collision = new BoxCollision()
                {
                    Owner = entity.Id,
                    Bounds = square.Bounds
                };
                var gravity = new GravityController()
                {
                    Owner = entity.Id,
                    Weight = 2.0f,
                };
                
                _entityFactory.AddComponent(collision);
                _entityFactory.AddComponent(square);
                _entityFactory.AddComponent(gravity);
            }
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