using CM3D.Core.Windowing;
using MapleSharp.Events.Experimental;
using MapleSharp.Services;
using MapleSharp.Window;
using OpenTK.Graphics.OpenGL4;
using SDL2;

namespace MapleSharp.Graphics;

public class GraphicsDevice
{
    private IWindow window;
    private IntPtr sdlContext;
    
    public GraphicsDevice(IWindow gameWindow)
    {
        window = gameWindow;
        sdlContext = SDL.SDL_GL_CreateContext(window.Handle);
        if (sdlContext == IntPtr.Zero)
            throw new Exception("Failed to create OpenGL context.");
        GL.LoadBindings(new SDLBindingsContext());
        window.GraphicsDevice = this;
    }
    
    public void Clear(float r, float g, float b, float a)
    {
        GL.ClearColor(r, g, b, a);
        GL.Clear(ClearBufferMask.ColorBufferBit);
    }
    
    public void SwapBuffers()
    {
        SDL.SDL_GL_SwapWindow(window.Handle);
    }
    
    public void Release()
    {
        SDL.SDL_GL_DeleteContext(sdlContext);
    }
}