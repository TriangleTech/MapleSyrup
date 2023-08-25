
using MapleSharp.Events.Experimental;
using MapleSharp.Graphics;
using MapleSharp.NX;
using MapleSharp.Resources;
using MapleSharp.Services;
using SDL2;

namespace MapleSharp.Window;

public abstract class GameWindow : IWindow
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
    private EventFactory eventFactory;

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
        set { width = value; }
    }

    public int Height
    {
        get => height;
        set { height = value; }
    }

    public bool VSync
    {
        get => vsync;
        set { vsync = value; }
    }

    public bool IsRunning
    {
        get => isRunning;
    }

    public GraphicsDevice GraphicsDevice
    {
        get => graphicsDevice;
        set => graphicsDevice = value;
    }

    public IntPtr Handle
    {
        get => sdlWindow;
    }

    public GameWindow()
    {
        title = "MapleSharp";
        width = 1280;
        height = 768;
        vsync = true;
        isRunning = false;
    }
    
    public GameWindow(string title, int width, int height, bool vsync)
    {
        this.title = title;
        this.width = width;
        this.height = height;
        this.vsync = vsync;
        isRunning = false;
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
        eventFactory = serviceFactory.GetService<EventFactory>();
        eventFactory.RegisterEvent("testme", TestMethod);
    }

    private object TestMethod(object arg)
    {
        if ((int)arg == 69)
            return $"ROFL 420:{arg}";
        return $"LOL 69:{arg}";
    }

    public virtual void Initialize()
    {
        InitFactories();
        InitSdl();
    }

    public virtual void OnLoad()
    {
        
    }

    public virtual void OnRender()
    {
    }

    public virtual void OnUpdate(float timeDelta)
    {
        
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