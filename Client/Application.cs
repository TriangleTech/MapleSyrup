using Client.Managers;
using Client.NX;
using Client.Scene;
using Raylib_CsLo;

namespace Client;

public class Application : IDisposable
{
    public Application()
    {
    }

    public void Run()
    {
        Raylib.InitWindow(AppConfig.ScreenWidth, AppConfig.ScreenHeight, AppConfig.ClientName);
        Raylib.SetTargetFPS(30);
        Raylib.SetTraceLogLevel((int)TraceLogLevel.LOG_NONE);

        LoadContent();
        var net = ServiceLocator.Get<NetworkManager>();
        var world = ServiceLocator.Get<WorldManager>();
        var ui = ServiceLocator.Get<UIManager>();

        net.Connect();
        world.SetState(WorldState.StartUp);
        world.CreateLogin();
        var scene = world.GetWorld() ?? throw new ArgumentNullException("world.GetWorld()");
        while (!Raylib.WindowShouldClose() && !AppConfig.CloseWindow)
        {
            var frameTime = Raylib.GetFrameTime() * 1000;
            net.HandlePacket();
            scene.Update(frameTime);
            ui.Update(frameTime);
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.GRAY);
            Raylib.BeginMode2D(world.GetCamera());
            scene.Draw(frameTime);
            ui.Draw(frameTime);
            Raylib.EndMode2D();
            Raylib.DrawFPS(0, 0);
            Raylib.EndDrawing();

            if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_ALT) && Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER))
            {
                if (!AppConfig.IsFullscreen)
                {
                    AppConfig.ScreenWidth = 1024;
                    AppConfig.ScreenHeight = 768;
                    Raylib.SetWindowSize(AppConfig.ScreenWidth, AppConfig.ScreenHeight);
                    var widthDiff = (float)AppConfig.ScreenWidth / AppConfig.OriginalWidth;
                    var heightDiff = (float)AppConfig.ScreenHeight / AppConfig.OriginalHeight;
                    world.GetWorld().UpdateZoom(Math.Min(widthDiff, heightDiff));
                }
                else
                {
                    AppConfig.ScreenWidth = 800;
                    AppConfig.ScreenHeight = 600;
                    Raylib.SetWindowSize(AppConfig.ScreenWidth, AppConfig.ScreenHeight);
                    var widthDiff = (float)AppConfig.ScreenWidth / AppConfig.OriginalWidth;
                    var heightDiff = (float)AppConfig.ScreenHeight / AppConfig.OriginalHeight;
                    world.GetWorld().UpdateZoom(Math.Min(widthDiff, heightDiff));
                }

                AppConfig.IsFullscreen = !AppConfig.IsFullscreen;
            }
        }

        net.Disconnect();
        UnloadContent();
        Raylib.CloseWindow();
    }

    private void LoadContent()
    {
        ServiceLocator.Register(new NxManager());
        ServiceLocator.Register(new NetworkManager());
        ServiceLocator.Register(new ActorManager());
        ServiceLocator.Register(new InputManager());
        ServiceLocator.Register(new UIManager());
        ServiceLocator.Register(new WorldManager());
    }

    private void UnloadContent()
    {
        ServiceLocator.Release();
    }

    public void Dispose()
    {

    }
}