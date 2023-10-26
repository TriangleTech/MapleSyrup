using MapleSyrup.Core;
using MapleSyrup.Core.Input;
using MapleSyrup.Graphics;
using MapleSyrup.Nodes;
using MapleSyrup.Resources;
using MapleSyrup.Resources.Scripting;
using MapleSyrup.Scene;
using NLua;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SDL2;

namespace MapleSyrup.Window;

public class GameWindow : IWindow, IDisposable
{
    private string title;
    private int width, height;
    private bool vsync;
    private bool isRunning;
    private IntPtr sdlWindow;
    private Engine engine;

    public Engine Engine => engine;

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

    public IntPtr Handle => sdlWindow;

    public GameWindow()
    {
        title = "MapleSyrup";
        width = 1280;
        height = 768;
        vsync = true;
        isRunning = false;
        engine = new Engine(this);
    }

    public GameWindow(string title, int width, int height, bool vsync)
    {
        this.title = title;
        this.width = width;
        this.height = height;
        this.vsync = vsync;
        isRunning = false;
        engine = new Engine(this);
    }

    private void InitSdl()
    {
        sdlWindow = SDL.SDL_CreateWindow(Title, SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED, Width, Height,
            SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
        if (sdlWindow == IntPtr.Zero)
            throw new Exception(
                $"[GameWindow] Failed to create SDL window.  Exiting... SDL Error:  {SDL.SDL_GetError()} ");
    }
    
    public void Initialize()
    {
        InitSdl();
        InitSubsystems();
        InitEvents();
    }

    private void InitSubsystems()
    {
        Engine.AddSubsystem(new EventSystem());
        Engine.AddSubsystem(new LuaSystem());
        Engine.AddSubsystem(new RenderSystem(this));
        Engine.AddSubsystem(new SceneSystem(new Node()));
        Engine.AddSubsystem(new InputSystem());
        Engine.AddSubsystem(new NxSystem());
        Engine.AddSubsystem(new ResourceSystem(Engine));
    }

    private void InitEvents()
    {
        var events = Engine.GetSubsystem<EventSystem>();
        events.ListenForEvent("OnWindowResized", OnWindowResized);
    }

    public void OnLoad()
    {
        _ = Engine.GetSubsystem<ResourceSystem>().LoadShader("sprite", "Assets/Shaders/sprite.vert", "Assets/Shaders/sprite.frag");
        var child = new SpriteNode(Engine.GetSubsystem<ResourceSystem>().GetImage("map/Back/grassySoil.img/back/1"));
        var child2 = new SpriteNode(Engine.GetSubsystem<ResourceSystem>().GetImage("map/Back/grassySoil.img/back/2"));
        var child3 = new SpriteNode(Engine.GetSubsystem<ResourceSystem>().GetImage("map/Back/grassySoil.img/back/3"));
        var child4 = new SpriteNode(Engine.GetSubsystem<ResourceSystem>().GetImage("map/Back/grassySoil.img/back/6"));
        child.Z = 0;
        //child.Position = new Vector2(100, -180);
        //child.Origin = new Vector2(512, 289);
        child.Layer = RenderLayer.Background;
        
        child2.Z = 0;
        //child2.Position = new Vector2(300, 55);
        //child2.Origin = new Vector2(1246, 170);
        child2.Layer = RenderLayer.Background;
        
        child3.Z = 0;
        //child3.Position = new Vector2(179, -249);
        //child3.Origin = new Vector2(157, 134);
        child3.Layer = RenderLayer.Background;
        
        child4.Z = 0;
        //child4.Position = new Vector2(100, 180);
        //child4.Origin = new Vector2(512, 147);
        child4.Layer = RenderLayer.Background;
        
        Engine.GetSubsystem<SceneSystem>().AddChild(new CameraNode());
        Engine.GetSubsystem<SceneSystem>().AddChild(child);
        Engine.GetSubsystem<SceneSystem>().AddChild(child2);
        Engine.GetSubsystem<SceneSystem>().AddChild(child3);
        Engine.GetSubsystem<SceneSystem>().AddChild(child4);
    }
    
    public void OnRender()
    {
        Engine.GetSubsystem<RenderSystem>().Clear(0.2f, 0.2f, 0.2f, 1.0f);
        Engine.GetSubsystem<SceneSystem>().Render();
        Engine.GetSubsystem<RenderSystem>().SwapBuffers();
    }

    public virtual void OnUpdate(float timeDelta)
    {
        Engine.Instance.Update(timeDelta);
    }

    public virtual void OnUnload()
    {
        Engine.Instance.Shutdown();
    }

    public void Run()
    {
        isRunning = true;
        Initialize();
        OnLoad();

        while (isRunning)
        {
            SDL.SDL_PollEvent(out var sdlEvent);
            HandleEvent(sdlEvent);
            if (sdlEvent.type == SDL.SDL_EventType.SDL_QUIT)
                isRunning = false;
            var timeDelta = SDL.SDL_GetTicks() / 1000.0f;

            OnUpdate(timeDelta);
            OnRender();
        }
        
        OnUnload();
        SDL.SDL_DestroyWindow(sdlWindow);
        SDL.SDL_Quit();
    }

    private void HandleEvent(SDL.SDL_Event sdlEvent)
    {
        switch (sdlEvent.type)
        {
            case SDL.SDL_EventType.SDL_WINDOWEVENT:
                switch (sdlEvent.window.windowEvent)
                {
                    case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED:
                        var size = new Vector2(sdlEvent.window.data1, sdlEvent.window.data2);
                        Engine.GetSubsystem<EventSystem>().TriggerEvent("OnWindowResized", data: size);
                        break;
                }
                break;
            case SDL.SDL_EventType.SDL_KEYDOWN:
                Engine.GetSubsystem<InputSystem>().OnKeyDown(sdlEvent.key.keysym.sym);
                break;
            case SDL.SDL_EventType.SDL_KEYUP:
                Engine.GetSubsystem<InputSystem>().OnKeyUp(sdlEvent.key.keysym.sym);
                break;
            case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                Engine.GetSubsystem<InputSystem>().OnMouseButtonDown(sdlEvent.button);
                break;
            case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
                Engine.GetSubsystem<InputSystem>().OnMouseButtonUp(sdlEvent.button);
                break;
            case SDL.SDL_EventType.SDL_MOUSEMOTION:
                Engine.GetSubsystem<InputSystem>().OnMouseMotion(sdlEvent.motion);
                break;
            case SDL.SDL_EventType.SDL_MOUSEWHEEL:
                Engine.GetSubsystem<InputSystem>().OnMouseWheel(sdlEvent.wheel);
                break;
        }
    }

    private void OnWindowResized(object data)
    {
        var size = (Vector2)data;
        width = (int)size.X;
        height = (int)size.Y;
        GL.Viewport(0, 0, width, height);
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
        Engine.Instance.Dispose();
    }
}