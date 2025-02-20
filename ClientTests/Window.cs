using System.Numerics;
using System.Text.Json;
using MapleSyrup.Gui.Containers;
using MapleSyrup.Gui.Widgets;
using MapleSyrup.Nx;
using ZeroElectric.Vinculum;

namespace ClientTests;

public class Window
{
    private readonly NXFactory _nxFactory;

    public Window()
    {
        _nxFactory = new NXFactory();
    }

    private void UnloadContent()
    {
        _nxFactory.Shutdown();
    }

    public void Run()
    {

        Raylib.InitWindow(1280, 768, "Client Testbed");
        Raylib.SetTargetFPS(30);
        Raylib.SetTraceLogLevel((int)TraceLogLevel.LOG_NONE);

        var boxContainer = new DialogContainer("", 300, 500)
        {
            HeaderColor = Raylib.BLUE,
            Header = "MyHeader",
            FontSize = 18,
            FontColor = Raylib.WHITE,
            OutlineColor = Raylib.WHITE,
            BackgroundColor = Raylib.DARKGRAY,
            Roundness = 0.2f,
            Alpha = 255,
            CanClose = true,
            CanMinimize = false,
        };

        while (!Raylib.WindowShouldClose())
        {
            var frameTime = Raylib.GetFrameTime() * 1000;
            boxContainer.Update(frameTime);
            
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.GRAY);
            
            boxContainer.Draw();
            
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