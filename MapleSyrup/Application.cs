using MapleSyrup.Core;
using Microsoft.Xna.Framework;

namespace MapleSyrup;

public class Application : Game
{
    private GraphicsDeviceManager graphicsDeviceManager;
    private MapleContext context;
    private MapleEngine engine;
    
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
        context = new(this);
        engine = new MapleEngine(context);
        engine.Initialize();
        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        engine.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        engine.Render();
        base.Draw(gameTime);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing); }
}