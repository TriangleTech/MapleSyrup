using MapleSyrup.ECS;
using MapleSyrup.Factories;
using Microsoft.Xna.Framework;

namespace MapleSyrup;

public class Application : Game
{
    private GraphicsDeviceManager graphicsDeviceManager;
    private ServiceFactory service;
    
    public Application()
    {
        graphicsDeviceManager = new GraphicsDeviceManager(this);
        graphicsDeviceManager.PreferredBackBufferWidth = 1280;
        graphicsDeviceManager.PreferredBackBufferHeight = 768;
        graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
        graphicsDeviceManager.ApplyChanges();
        service = new ServiceFactory();
    }

    private void InitializeFactories()
    {
        service.AddFactory<ResourceFactory>();
        service.AddFactory<WorldFactory>();
    }

    protected override void Initialize()
    {
        base.Initialize();
        InitializeFactories();
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing); }
}