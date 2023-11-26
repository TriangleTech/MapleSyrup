using MapleSyrup.ECS;
using MapleSyrup.Factories;
using Microsoft.Xna.Framework;

namespace MapleSyrup;

public class Application : Game
{
    private GraphicsDeviceManager graphicsDeviceManager;
    private World world;
    
    public Application()
    {
        graphicsDeviceManager = new GraphicsDeviceManager(this);
        graphicsDeviceManager.PreferredBackBufferWidth = 1280;
        graphicsDeviceManager.PreferredBackBufferHeight = 768;
        graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
        graphicsDeviceManager.ApplyChanges();
    }

    protected override void Initialize()
    {
        base.Initialize();

        world = World.CreateWorld();
        world.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        world.Update(gameTime.ElapsedGameTime.Seconds);
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        world.Draw();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        world.Dispose();
    }
}