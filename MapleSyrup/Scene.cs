using System.Diagnostics;
using MapleSyrup.EC;
using MapleSyrup.Event;
using MapleSyrup.Managers;
using Microsoft.Xna.Framework;

namespace MapleSyrup;

public class Scene : Game
{
    private readonly GraphicsDeviceManager _graphicsDeviceManager;
    private readonly ManagerLocator _locator;
    private readonly List<Entity> _entities;

    public Scene()
    {
        _graphicsDeviceManager = new(this);
        _graphicsDeviceManager.PreferredBackBufferWidth = 800;
        _graphicsDeviceManager.PreferredBackBufferHeight = 600;
        
        _locator = new(this);
        _entities = new();
    }

    protected override void Initialize()
    {
        _locator.RegisterManager(new EventManager());
        _locator.RegisterManager(new ResourceManager(true));
        _locator.RegisterManager(new MapManager());
        _locator.Initialize();
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        // load the map information
        // sort by z index

        var events = _locator.GetManager<EventManager>();
        var map = _locator.GetManager<MapManager>();
        _ = map.Create(10000);
        
        events.Dispatch(EventFlag.OnMapLoaded);
        events.Dispatch(EventFlag.OnMapChanged);
        events.Dispatch(EventFlag.OnMapUnloaded);
        events.Dispatch(EventFlag.OnMapLoaded);
        
        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        var events = _locator.GetManager<EventManager>();
        events.PollEvents();
        
        for (int i = 0; i < _entities.Count; i++)
        {
            if (!(_entities[i] & EntityFlag.Active))
                continue;
        }
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.DarkGray);

        for (int i = 0; i < _entities.Count; i++)
        {
            if (!(_entities[i] & EntityFlag.Active))
                continue;
        }
        base.Draw(gameTime);
    }

    protected override void UnloadContent()
    {
        _entities.Clear();
        _locator.Shutdown();
        base.UnloadContent();
    }
}