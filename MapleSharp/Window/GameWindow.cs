
using MapleSharp.Core;
using MapleSharp.Core.Event;
using MapleSharp.Graphics;
using MapleSharp.Resources;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SDL2;

namespace MapleSharp.Window;

public abstract class GameWindow : EngineObject, IWindow, IDisposable
{
    private string title;
    private int width, height;
    private bool vsync;
    private bool isRunning;
    private IntPtr sdlWindow;
    private GraphicsDevice graphicsDevice;
    
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

    public virtual void Initialize()
    {
        Engine.AddSubsystem(new EventSystem());
        Engine.AddSubsystem(new NxSystem());
        Engine.AddSubsystem(new ResourceSystem(Engine));
        InitSdl();
    }
    
    private Sprite sprite;
    private Shader shader;

    public virtual void OnLoad()
    {
        shader = GetSubsystem<ResourceSystem>().LoadShader("sprite", "default.vert", "default.frag");
        sprite = new Sprite(GetSubsystem<ResourceSystem>().GetTexture("map/Back/grassySoil.img/back/1"));
        GL.Viewport(0, 0, Width, Height);
    }

    float xpos = 0;
    public virtual void OnRender()
    {
        graphicsDevice.Clear(0.2f, 0.2f, 0.2f, 1.0f);
        xpos += (float)Math.Sin(SDL.SDL_GetTicks() / 1000.0f) * 10f;
        var view = Matrix4.LookAt(new Vector3(xpos, 0, 1.0f), new Vector3(xpos, 0, -1.0f), Vector3.UnitY);
        var projection = Matrix4.CreateOrthographicOffCenter(0.0f, Width, Height, 0.0f, -1.0f, 1.0f);
        shader.Use();
        shader.SetInt("image", 0);
        shader.SetMatrix4("projection", projection);
        shader.SetMatrix4("view", view);
        sprite.Draw();
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
            var timeDelta = SDL.SDL_GetTicks() / 1000.0f;

            OnUpdate(timeDelta);
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
    
    public void Dispose()
    {
        Engine.Dispose();
    }
}