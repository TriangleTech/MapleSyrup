using System.Diagnostics;
using ImGuiNET;
using MapleSyrup.EC;
using MapleSyrup.EC.Components;
using MapleSyrup.Event;
using MapleSyrup.Managers;
using MapleSyrup.Map;
using MapleSyrup.Player;
using MapleSyrup.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDL2;
using Num = System.Numerics;

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
    private Avatar _avatar;
    private CameraComponent _camera;
    
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
        SDL.SDL_SetWindowTitle(Window.Handle, "MapleSyrup - Status: LOADING...");
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _camera = new CameraComponent(_avatar);
        _camera.Position = Vector2.Zero;
        _camera.Viewport = GraphicsDevice.Viewport;
        
        var _event = _locator.GetManager<EventManager>();
        var map = _locator.GetManager<MapManager>();
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _event.Register(this);
        _map = map.Create(10000);
        
        
        //_avatar = new Avatar(_locator);
        //_avatar.Transform.Position = new Vector2(0, 0);
        //_avatar.Camera = _camera;
        
        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        var _event = _locator.GetManager<EventManager>();
        _event.PollEvents();

        Task.Run(() =>
        {
            for (int i = 0; i < _sorted.Count; i++)
            {
                if (!(_sorted[i] & EntityFlag.Active))
                    continue;
                _map.UpdateBackground(_sorted[i], _camera);
                _map.UpdateObj(_sorted[i], gameTime);
                _map.UpdatePortal(_sorted[i], gameTime);
            }
        });

        /*
        Task.Run(() =>
        {
            _avatar.UpdatePlayer(gameTime);
            _avatar.TestInput();
            _camera.UpdateMatrix(_avatar);
        });*/
        
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
            _map.RenderPortal(_spriteBatch, _sorted[i]);
        }
        //_avatar.DrawPlayer(_spriteBatch);
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
                Console.WriteLine("Fully Loaded");
                _loaded = true;
                _sorted = _entities.OrderBy(x => x.Layer).ThenBy(x => x.Transform.zIndex).ToList();
                SDL.SDL_SetWindowTitle(Window.Handle, "MapleSyrup - Status: LOADED");
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
        //_avatar.CleanUp();
        foreach (var entity in _entities)
        {
            entity.CleanUp();
        }
        _entities.Clear();
        _locator.Shutdown();
        base.UnloadContent();
    }
}