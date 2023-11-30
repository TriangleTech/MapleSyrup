using MapleSyrup.Core;
using MapleSyrup.Gameplay;
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
    private MapleEngine engine;
    private MapleWorld world;
    
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
        engine.Initialize();
        world = context.GetSubsystem<WorldSystem>().Create("000010000"); // TODO: Once verified find where to actually put this...
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
        base.Dispose(disposing); 
        context.Shutdown();
    }
}