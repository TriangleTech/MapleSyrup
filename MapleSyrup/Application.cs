using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.ECS.Components;
using MapleSyrup.Resources;
using MapleSyrup.Resources.Nx;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDL2;
using Color = Microsoft.Xna.Framework.Color;

namespace MapleSyrup;

public class Application : Game
{
    private GraphicsDeviceManager graphicsDeviceManager;
    private GameContext context;
    
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
        
        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        var events = context.GetSubsystem<EventSystem>();
        var eventData = new EventData()
        {
            ["GameTime"] = gameTime
        };
        events.Publish(EventType.OnUpdate, eventData);
        
        var scene = context.GetSubsystem<SceneSystem>().Current;
        var entities = scene.Entities.FindAll(x => x.IsEnabled);

        SDL.SDL_SetWindowTitle(this.Window.Handle, $"MapleSyrup - Number of Entities Visible {entities.Count}");
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        
        var events = context.GetSubsystem<EventSystem>();
        events.Publish(EventType.OnRender);
        base.Draw(gameTime);
    }

    protected override void Dispose(bool disposing)
    {
        context.Shutdown();
        base.Dispose(disposing);
    }
}