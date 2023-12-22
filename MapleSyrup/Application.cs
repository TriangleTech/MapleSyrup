using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.ECS.Components;
using MapleSyrup.Resources;
using MapleSyrup.Resources.Nx;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace MapleSyrup;

public class Application : Game
{
    private GraphicsDeviceManager graphicsDeviceManager;
    private GameContext context;
    private SpriteBatch spriteBatch;
    
    public Application()
    {
        graphicsDeviceManager = new GraphicsDeviceManager(this);
        graphicsDeviceManager.PreferredBackBufferWidth = 1280;
        graphicsDeviceManager.PreferredBackBufferHeight = 720;
        graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
        graphicsDeviceManager.ApplyChanges();
        context = new(this);
    }

    protected override void Initialize()
    {
        context.AddSubsystem<EventSystem>();
        context.AddSubsystem<GraphicsSystem>();
        context.AddSubsystem<ResourceSystem>();
        context.AddSubsystem<TimeSystem>();
        context.AddSubsystem<SceneSystem>();
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        context.GetSubsystem<ResourceSystem>().SetBackend(ResourceBackend.Nx);

        var scene = context.GetSubsystem<SceneSystem>();
        scene.LoadScene("000060000");
        spriteBatch = new SpriteBatch(GraphicsDevice);
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