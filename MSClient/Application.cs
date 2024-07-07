using MSClient.Managers;
using MSClient.NX;
using Raylib_CsLo;

namespace MSClient;

public class Application : IDisposable
{
    public Application()
    {
    }

    public void Run()
    {
        Raylib.InitWindow(1280, 768, "MapleSyrup");
        Raylib.SetTargetFPS(30);
        Raylib.SetTraceLogLevel(7);
        LoadContent();
        //var network = ServiceLocator.Get<NetworkManager>();
        var world = ServiceLocator.Get<WorldManager>();
        world.CreateWorld("100000000");

        while (!Raylib.WindowShouldClose())
        { 
            //network.ProcessResponses();
            Update(Raylib.GetFrameTime());
            
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.GRAY);
            Raylib.DrawFPS(0, 0);
            Draw(Raylib.GetFrameTime());
            Raylib.EndDrawing();
            //network.ProcessRequests();
        }
        UnloadContent();
        Raylib.CloseWindow();
    }
    
    private void LoadContent()
    {
        ServiceLocator.Register(new NxManager());
        //ServiceLocator.Register(new NetworkManager());
        ServiceLocator.Register(new ActorManager());
        ServiceLocator.Register(new WorldManager());
    }

    private void UnloadContent()
    {
        ServiceLocator.Release();
    }

    private void Update(float frameTime)
    {
        var world = ServiceLocator.Get<WorldManager>().GetWorld();
        world?.Update(frameTime);
    }

    private void Draw(float frameTime)
    {
        var world = ServiceLocator.Get<WorldManager>().GetWorld();
        world?.Draw(frameTime);
    }

    public void Dispose()
    {
        
    }
}