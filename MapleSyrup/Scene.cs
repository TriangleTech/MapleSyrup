using System.Diagnostics;
using MapleSyrup.EC;
using MapleSyrup.Event;
using MapleSyrup.Managers;
using MapleSyrup.Map;
using Microsoft.Xna.Framework;

namespace MapleSyrup;

public class Scene : Game, IEventListener
{
    public EventFlag Flags { get; }
    private readonly GraphicsDeviceManager _graphicsDeviceManager;
    private readonly ManagerLocator _locator;
    private readonly List<IEntity> _entities;
    private List<IEntity> _sorted;

    public Scene()
    {
        _graphicsDeviceManager = new(this);
        _graphicsDeviceManager.PreferredBackBufferWidth = 800;
        _graphicsDeviceManager.PreferredBackBufferHeight = 600;
        
        _locator = new(this);
        _entities = new();
        _sorted = new();

        Flags = EventFlag.OnEntityCreated | EventFlag.OnEntityRemoved | EventFlag.OnEntityChanged;
    }

    protected override void Initialize()
    {
        _locator.RegisterManager(new EventManager());
        _locator.RegisterManager(new ResourceManager(ResourceBackend.Nx));
        _locator.RegisterManager(new EntityManager());
        _locator.RegisterManager(new MapManager());
        _locator.Initialize();
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        // load the map information
        // sort by z index
        
        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        var _event = _locator.GetManager<EventManager>();
        _event.PollEvents();
        
        for (int i = 0; i < _sorted.Count; i++)
        {
            if (!(_sorted[i] & EntityFlag.Active))
                continue;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.DarkGray);

        for (int i = 0; i < _sorted.Count; i++)
        {
            if (!(_entities[i] & EntityFlag.Active))
                continue;
        }
        
        base.Draw(gameTime);
    }
    
    public void ProcessEvent(EventFlag flag)
    {
        switch (flag)
        {
            case EventFlag.OnLayerChanged:
                _sorted = _entities.OrderBy(x => x.Layer).ThenBy(x => x.Transform.zIndex).ToList();
                break;
        }
    }

    public void ProcessEvent(EventFlag flag, IEntity entity)
    {
        switch (flag)
        {
            case EventFlag.OnEntityCreated:
                _entities.Add(entity);
                break;
            case EventFlag.OnEntityChanged:
                break;
            case EventFlag.OnEntityRemoved:
                _entities.Remove(entity);
                break;
        }
        
        _sorted = _entities.OrderBy(x => x.Layer).ThenBy(x => x.Transform.zIndex).ToList();
    }
    
    protected override void UnloadContent()
    {
        _entities.Clear();
        _locator.Shutdown();
        base.UnloadContent();
    }
}