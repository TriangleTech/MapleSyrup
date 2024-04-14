using MapleSyrup.GameObjects;
using MapleSyrup.Managers;
using MapleSyrup.Nx;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MapleSyrup;

public class Application : Game
{
    private GraphicsDeviceManager _graphicsDeviceManager;
    private ResourceManager _resourceManager;
    private ActorManager _actorManager;
    
    public Application()
        : base()
    {
        _graphicsDeviceManager = new GraphicsDeviceManager(this);
        _graphicsDeviceManager.PreferredBackBufferWidth = 800;
        _graphicsDeviceManager.PreferredBackBufferHeight = 600;
    }

    private SpriteBatch sb;
    protected override void Initialize()
    {
        base.Initialize();
    }

    private Mob mob;
    
    protected override void LoadContent()
    {
        _resourceManager = new(this);
        _actorManager = new(this);
        
        _resourceManager.Initialize();

        mob = _actorManager.CreateMob("0100100");
        
        sb = new SpriteBatch(GraphicsDevice);
        
        base.LoadContent();
    }

    protected override void UnloadContent()
    {
        _resourceManager.Destroy();
        _actorManager.Destroy();
        base.UnloadContent();
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.DarkGray);
        sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
        sb.End();
        base.Draw(gameTime);
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.M))
            mob.StateMachine.ChangeState("move");
        
        if (keyboard.IsKeyDown(Keys.S))
            mob.StateMachine.ChangeState("stand");
        
        if (keyboard.IsKeyDown(Keys.H))
            mob.StateMachine.ChangeState("hit1");
        
        if (keyboard.IsKeyDown(Keys.D))
            mob.StateMachine.ChangeState("die1");


        mob.Update(gameTime);
        
        base.Update(gameTime);
    }
}