using MapleSyrup.Core;
using MapleSyrup.Core.Interface;
using MapleSyrup.Window;
using OpenTK.Graphics.OpenGL4;
using SDL2;

namespace MapleSyrup.Graphics;

public class RenderSystem : ISubsystem
{
    private readonly Engine engine;
    private readonly IWindow window;
    private IntPtr sdlContext;
    
    public RenderSystem(IWindow gameWindow)
    {
        window = gameWindow;
        engine = gameWindow.Engine;
    }
    
    public void Initialize()
    {
        InitializeOpenGL();
    }

    public void Update(float timeDelta)
    {
        
    }

    public void Shutdown()
    {
        SDL.SDL_GL_DeleteContext(sdlContext);
    }
    
    private void InitializeOpenGL()
    {
        sdlContext = SDL.SDL_GL_CreateContext(window.Handle);
        if (sdlContext == IntPtr.Zero)
            throw new Exception("Failed to create OpenGL context.");
        GL.LoadBindings(new SDLBindingsContext());
        GL.Viewport(0, 0, window.Width, window.Height);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
        SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 3);
        SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, (int)SDL.SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_CORE);
        //SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_DOUBLEBUFFER, 1);
        //SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_DEPTH_SIZE, 24);
        //SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_STENCIL_SIZE, 8);
        SDL.SDL_GL_SetSwapInterval(window.VSync ? 1 : 0);
    }
    
    public void Clear(float r, float g, float b, float a)
    {
        GL.ClearColor(r, g, b, a);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }
    
    public void SwapBuffers()
    {
        SDL.SDL_GL_SwapWindow(window.Handle);
    }
}