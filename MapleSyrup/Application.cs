using MapleSyrup.GameObjects;
using MapleSyrup.GameObjects.Avatar;
using MapleSyrup.GameObjects.Components;
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
    }
    
    protected override void Initialize()
    {
        _graphicsDeviceManager.PreferredBackBufferWidth = 1280;
        _graphicsDeviceManager.PreferredBackBufferHeight = 768;
        _graphicsDeviceManager.ApplyChanges();
        
        base.Initialize();
    }
    
    protected override void LoadContent()
    {
        _resourceManager = new(this);
        _actorManager = new(this);
        _resourceManager.Initialize();
        
        _world = new GameWorld();
        _world.Load("000020000");

        var avatar = new Avatar();
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
        var frameRate = Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds);
        SDL.SDL_SetWindowTitle(Window.Handle, $"MapleSyrup | Actors:{_actorManager.Actors.Count()} | FPS: {frameRate}");
        Task.Run(() => _world.Update(gameTime));
        
        if (keyboard.IsKeyDown(Keys.S))
            _actorManager.SendIt();
        
        if (keyboard.IsKeyDown(Keys.Escape))
            Exit();
        base.Update(gameTime);
    }
}