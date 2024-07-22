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
        Raylib.InitWindow(800, 600, "MapleSyrup");
        Raylib.SetTargetFPS(30);
        Raylib.SetTraceLogLevel((int)TraceLogLevel.LOG_NONE);
        RayGui.GuiLoadStyleDefault();
        
        LoadContent();
        //var network = ServiceLocator.Get<NetworkManager>();
        var world = ServiceLocator.Get<WorldManager>();
        world.SetState(WorldState.StartUp);
        //world.CreateLogin();
        world.CreateWorld("100000000");
        while (!Raylib.WindowShouldClose())
        { 
            //network.ProcessResponses();
            Update(Raylib.GetFrameTime() * 1000);
            
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.GRAY);
            Draw(Raylib.GetFrameTime() * 1000);
            Raylib.DrawFPS(0, 0);
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
        Task.Run(() => world?.Update(frameTime));
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