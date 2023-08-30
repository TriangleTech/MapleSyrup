using MapleSyrup.Core;
using MapleSyrup.Graphics;
using MapleSyrup.Nodes;
using MapleSyrup.Resources;
using MapleSyrup.Scene;
using MapleSyrup.Scripting;
using NLua;
using OpenTK.Graphics.OpenGL4;
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
            SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL);
        if (sdlWindow == IntPtr.Zero)
            throw new Exception(
                $"[GameWindow] Failed to create SDL window.  Exiting... SDL Error:  {SDL.SDL_GetError()} ");
    }
    
    public void Initialize()
    {
        InitSdl();
        Engine.AddSubsystem(new EventSystem());
        Engine.AddSubsystem(new LuaSystem());
        Engine.AddSubsystem(new RenderSystem(this));
        Engine.AddSubsystem(new NxSystem());
        Engine.AddSubsystem(new ResourceSystem(Engine));
        Engine.AddSubsystem(new NodeSystem());
        Engine.AddSubsystem(new SceneSystem(new SceneNode()));
    }
    
    public void OnLoad()
    {
        _ = Engine.GetSubsystem<ResourceSystem>().LoadShader("sprite", "Assets/Shaders/sprite.vert", "Assets/Shaders/sprite.frag");
        var child = Engine.GetSubsystem<NodeSystem>()
            .CreateSpriteNode(Engine.GetSubsystem<ResourceSystem>().GetImage("map/Back/grassySoil.img/back/1"));
        Engine.GetSubsystem<SceneSystem>().AddChild(Engine.GetSubsystem<NodeSystem>().CreateNode<CameraNode>());
        Engine.GetSubsystem<SceneSystem>().AddChild(child);
    }

    float xpos = 0;

    public void OnRender()
    {
        Engine.GetSubsystem<RenderSystem>().Clear(0.2f, 0.2f, 0.2f, 1.0f);
        Engine.GetSubsystem<SceneSystem>().Render();
        //xpos += (float)Math.Sin(SDL.SDL_GetTicks() / 1000.0f) * 10f;
        //var view = Matrix4.LookAt(new Vector3(xpos, 0, 1.0f), new Vector3(xpos, 0, -1.0f), Vector3.UnitY);
        //var projection = Matrix4.CreateOrthographicOffCenter(0.0f, Width, Height, 0.0f, -1.0f, 1.0f);
        //shader.Use();
        //shader.SetInt("image", 0);
        //shader.SetMatrix4("projection", projection);
        //shader.SetMatrix4("view", view);
        //sprite.Draw();
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