using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.Resource;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SDL2;
using Color = Microsoft.Xna.Framework.Color;

namespace MapleSyrup;

public class Application : Game
{
    private GraphicsDeviceManager graphicsDeviceManager;
    private GameContext context;
    private bool isFullscreen = false;
    
    public Application()
    {
        graphicsDeviceManager = new GraphicsDeviceManager(this);
        graphicsDeviceManager.PreferredBackBufferWidth = 800;
        graphicsDeviceManager.PreferredBackBufferHeight = 600;
        graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
        graphicsDeviceManager.ApplyChanges();
        context = new(this);
    }

    protected override void Initialize()
    {
        context.AddSubsystem<EventSystem>();
        context.AddSubsystem<ResourceSystem>();
        context.AddSubsystem<TimeSystem>();
        context.AddSubsystem<SceneSystem>();
        SDL.SDL_SetWindowTitle(Window.Handle, "MapleSyrup");
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        context.GetSubsystem<ResourceSystem>().SetBackend(ResourceBackend.Nx);
        var scene = context.GetSubsystem<SceneSystem>();
        scene.LoadScene("100000000");

        _ = scene.CreateTestPlayer();
        
        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.F5))
        {
            if (!isFullscreen)
            {
                graphicsDeviceManager.PreferredBackBufferWidth = 1920;
                graphicsDeviceManager.PreferredBackBufferHeight = 1080;
                graphicsDeviceManager.IsFullScreen = true;
                graphicsDeviceManager.ApplyChanges();
                isFullscreen = true;
            }
            else
            {
                graphicsDeviceManager.PreferredBackBufferWidth = 800;
                graphicsDeviceManager.PreferredBackBufferHeight = 600;
                graphicsDeviceManager.IsFullScreen = false;
                graphicsDeviceManager.ApplyChanges();
                isFullscreen = false;
            }
        }

        var events = context.GetSubsystem<EventSystem>();
        var eventData = new EventData()
        {
            ["GameTime"] = gameTime
        };
        events.Publish("ENGINE_UPDATE", eventData);
        
        var scene = context.GetSubsystem<SceneSystem>().Current;
        var entities = scene.Entities.FindAll(x => x.IsEnabled);

        SDL.SDL_SetWindowTitle(this.Window.Handle, $"MapleSyrup - Number of Entities Visible {entities.Count}");
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(0x33, 0x66, 0xCC));
        
        var events = context.GetSubsystem<EventSystem>();
        events.Publish("ENGINE_RENDER");
        base.Draw(gameTime);
    }

    protected override void Dispose(bool disposing)
    {
        context.Shutdown();
        base.Dispose(disposing);
    }
}