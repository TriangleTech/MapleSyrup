using MapleSyrup.Core;
using MapleSyrup.Resources;
using MapleSyrup.Resources.Nx;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework;

namespace MapleSyrup;

public class Application : Game
{
    private GraphicsDeviceManager graphicsDeviceManager;
    private GameContext context;
    private MapleEngine engine;
    
    public Application()
    {
        graphicsDeviceManager = new GraphicsDeviceManager(this);
        graphicsDeviceManager.PreferredBackBufferWidth = 1280;
        graphicsDeviceManager.PreferredBackBufferHeight = 768;
        graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
        graphicsDeviceManager.ApplyChanges();
        context = new(this);
        engine = new MapleEngine(context);
    }

    protected override void Initialize()
    {
        engine.Initialize(ResourceBackend.Nx);
        var scene = context.GetSubsystem<SceneSystem>();
        scene.LoadScene("000010000");
        
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
        context.Shutdown();
        base.Dispose(disposing);
    }
}