
using MapleSharp.Core;
using MapleSharp.Core.Event;
using MapleSharp.Graphics;
using MapleSharp.Resources;
using MapleSharp.Services;
using SDL2;

namespace MapleSharp.Window;

public abstract class GameWindow : EngineObject, IWindow
{
    private string title;
    private int width, height;
    private bool vsync;
    private bool isRunning;
    private IntPtr sdlWindow;
    private GraphicsDevice graphicsDevice;
    
    // Factories
    private ServiceFactory serviceFactory;
    private NxFactory nxFactory;
    private ResourceFactory resourceFactory;

    public Engine Engine { get; }

    public string Title
    {
        get => title;
        set
        {
            title = value;

            if (sdlWindow != IntPtr.Zero)
                SDL.SDL_SetWindowTitle(sdlWindow, title);
        }
    }

    public int Width
    {
        get => width;
        set => width = value;
    }

    public int Height
    {
        get => height;
        set => height = value;
    }

    public bool VSync
    {
        get => vsync;
        set => vsync = value;
    }

    public bool IsRunning => isRunning;

    public GraphicsDevice GraphicsDevice
    {
        get => graphicsDevice;
        set => graphicsDevice = value;
    }

    public IntPtr Handle => sdlWindow;

    public GameWindow(Engine engine) : base(engine)
    {
        title = "MapleSharp";
        width = 1280;
        height = 768;
        vsync = true;
        isRunning = false;
        Engine = engine;
    }
    
    public GameWindow(string title, int width, int height, bool vsync, Engine engine) : base(engine)
    {
        this.title = title;
        this.width = width;
        this.height = height;
        this.vsync = vsync;
        isRunning = false;
        Engine = engine;
    }

    private void InitSdl()
    {
        sdlWindow = SDL.SDL_CreateWindow(Title, SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED, Width, Height,
            SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL);
        if (sdlWindow == IntPtr.Zero)
            throw new Exception(
                $"[GameWindow] Failed to create SDL window.  Exiting... SDL Error:  {SDL.SDL_GetError()} ");
        graphicsDevice = new GraphicsDevice(this);
    }
    
    private void InitFactories()
    {
        serviceFactory = new ServiceFactory();
        nxFactory = serviceFactory.GetService<NxFactory>();
        resourceFactory = serviceFactory.GetService<ResourceFactory>();
    }

    public virtual void Initialize()
    {
        Engine.AddSubsystem(new EventSystem());
        InitFactories();
        InitSdl();
    }
    
    private Sprite sprite;

    public virtual void OnLoad()
    {
        
    }

    public virtual void OnRender()
    {
        graphicsDevice.Clear(0.2f, 0.2f, 0.2f, 1.0f);
        graphicsDevice.SwapBuffers();
    }

    public virtual void OnUpdate(float timeDelta)
    {
        Engine.Update(timeDelta);
    }

    public virtual void OnUnload()
    {
    }

    public void Run()
    {
        isRunning = true;
        Initialize();
        OnLoad();

        while (isRunning)
        {
            SDL.SDL_PollEvent(out var sdlEvent);
            if (sdlEvent.type == SDL.SDL_EventType.SDL_QUIT)
                isRunning = false;

            OnUpdate(0.0f);
            OnRender();
        }
        
        OnUnload();
        graphicsDevice.Release();
        SDL.SDL_DestroyWindow(sdlWindow);
        SDL.SDL_Quit();
    }

    public IWindow AddPlugin(string pluginName, params object[] args)
    {
        throw new NotImplementedException();
    }

    public IWindow AddDependency(string pluginName, params object[] args)
    {
        throw new NotImplementedException();
    }
}