using System.Diagnostics;
using MapleSyrup.EC;
using MapleSyrup.Event;
using MapleSyrup.Managers;
using MapleSyrup.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup;

public class Scene : Game, IEventListener
{
    private readonly GraphicsDeviceManager _graphicsDeviceManager;
    private readonly ManagerLocator _locator;
    private readonly List<IEntity> _entities;
    private List<IEntity> _sorted;
    private MapleMap _map;
    private SpriteBatch _spriteBatch;
    private bool _loaded;
    
    public EventFlag Flags { get; }
    public bool Loaded => _loaded;

    public Scene()
    {
        _graphicsDeviceManager = new(this);
        _graphicsDeviceManager.PreferredBackBufferWidth = 800;
        _graphicsDeviceManager.PreferredBackBufferHeight = 600;
        
        _locator = new(this);
        _entities = new();
        _sorted = new();
        _loaded = false;

        Flags = EventFlag.OnEntityCreated | EventFlag.OnEntityRemoved | EventFlag.OnEntityChanged | EventFlag.OnMapLoaded;
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
        var _event = _locator.GetManager<EventManager>();
        var map = _locator.GetManager<MapManager>();
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _event.Register(this);
        _map = map.Create(10000);
        
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
            _map.UpdateBackground(_sorted[i]);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.DarkGray);
        
        // TODO: Find a better way to flag that the entities have all been loaded.
        if (!_loaded)
            return;
        
        _map.RenderBackground(_spriteBatch, _sorted);
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap,
            DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
        for (int i = 0; i < _sorted.Count; i++)
        {
            _map.RenderTile(_spriteBatch, _sorted[i]);
            _map.RenderObj(_spriteBatch, _sorted[i]);
        }
        _spriteBatch.End();
        
        base.Draw(gameTime);
    }
    
    public void ProcessEvent(EventFlag flag)
    {
        switch (flag)
        {
            case EventFlag.OnLayerChanged:
                _sorted = _entities.OrderBy(x => x.Layer).ThenBy(x => x.Transform.zIndex).ToList();
                break;
            case EventFlag.OnMapLoaded:
                Console.WriteLine("Loaded");
                _loaded = true;
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
        
        // There will be a bombardment of events when its first initialized, so we'll manually
        // handled the sort in the LoadContent ourselves first. After that they'll go through
        // here.
        if (_loaded)
            _sorted = _entities.OrderBy(x => x.Layer).ThenBy(x => x.Transform.zIndex).ToList();
    }
    
    protected override void UnloadContent()
    {
        _spriteBatch.Dispose();
        _entities.Clear();
        _locator.Shutdown();
        base.UnloadContent();
    }
}