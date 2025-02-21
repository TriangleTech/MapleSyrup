using System.Numerics;
using MapleSyrup.Gui.Panels;
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

        var panel = LoginPanelExample();
        

        while (!Raylib.WindowShouldClose())
        {
            var frameTime = Raylib.GetFrameTime() * 1000;
            panel.Update(frameTime);
            
            Raylib.BeginDrawing();
            Raylib.ClearBackground(new Color(63, 52, 155, 255));
            panel.Draw();
            Raylib.DrawFPS(0, 0);
            Raylib.EndDrawing();
        }

        UnloadContent();
        Raylib.CloseWindow();
    }

    private StaticPanel LoginPanelExample()
    {
        var panel = new StaticPanel("")
        {
            BackgroundColor = new Color(157, 157, 157, 255),
            HasHeader = true,
            HeaderColor = new Color(32, 26, 29, 255),
            Width = 275,
            Height = 175,
            Movable = false,
            Position = new Vector2(Raylib.GetScreenWidth() / 4f, Raylib.GetScreenHeight() / 4f),
            Visible = true,
        };
        panel.AddWidget(new Label("headerLabel")
        {
            Text = "Login Form",
            FontSize = 16,
            FontColor = new Color(255, 255, 255, 255),
            Movable = false,
            Width = 50,
            Height = 50,
            Position = new Vector2(panel.Width / 3f, 10f),
        });
        panel.AddWidget(new Label("usernameLabel")
        {
            Text = "Username",
            FontSize = 16,
            FontColor = new Color(255, 255, 255, 255),
            Movable = false,
            Width = 50,
            Height = 50,
            Position = new Vector2(10f, panel.Height / 3f),
        });
        panel.AddWidget(new Label("passwordLabel")
        {
            Text = "Password",
            FontSize = 16,
            FontColor = new Color(255, 255, 255, 255),
            Movable = false,
            Width = 50,
            Height = 50,
            Position = new Vector2(10f, panel.Height - 70f),
        });
        panel.AddWidget(new TextBox("usernameBox")
        {
            BoxColor = new Color(177, 177, 177, 255),
            FontSize = 16,
            FontColor = new Color(255, 255, 255, 255),
            Movable = false,
            Width = 175,
            Height = 25,
            Position = new Vector2(90f, panel.Height / 3f),
            Roundness = 0.1f,
            CharacterLimit = 18
        });
        panel.AddWidget(new TextBox("passwordBox")
        {
            BoxColor = new Color(177, 177, 177, 255),
            FontSize = 16,
            FontColor = new Color(255, 255, 255, 255),
            Movable = false,
            Width = 175,
            Height = 25,
            Position = new Vector2(90f, panel.Height - 70f),
            Roundness = 0.1f,
            CharacterLimit = 18
        });

        return panel;
    }

    public void Dispose()
    {
        // TODO release managed resources here
        GC.SuppressFinalize(this);
    }
}