using MapleSyrup.GameObjects;
using MapleSyrup.Managers;
using MapleSyrup.Nx;
using MapleSyrup.Scene;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SDL2;

namespace MapleSyrup;

public class Application : Game
{
    private GraphicsDeviceManager _graphicsDeviceManager;
    private ResourceManager _resourceManager;
    private ActorManager _actorManager;
    private GameWorld _world;

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
    
    protected override void LoadContent()
    {
        _resourceManager = new(this);
        _actorManager = new(this);
        _resourceManager.Initialize();
        
        sb = new SpriteBatch(GraphicsDevice);
        _world = new GameWorld("000010000", ref _actorManager, ref _resourceManager);
        _world.Load();
        base.LoadContent();
    }

    protected override void UnloadContent()
    {
        _world.Destroy();
        _resourceManager.Destroy();
        _actorManager.Destroy();
        base.UnloadContent();
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.DarkGray);
        _world.Draw();
        base.Draw(gameTime);
    }
    
    protected override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();
        SDL.SDL_SetWindowTitle(Window.Handle, $"MapleSyrup | Actors:{_actorManager.Actors.Count()}");
        if (keyboard.IsKeyDown(Keys.Escape))
            Exit();
        _world.Update(gameTime);

        base.Update(gameTime);
    }
}