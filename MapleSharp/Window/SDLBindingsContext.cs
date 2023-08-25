using OpenTK;
using SDL2;

namespace CM3D.Core.Windowing;

public class SDLBindingsContext : IBindingsContext
{
    public IntPtr GetProcAddress(string procName)
    {
        return SDL.SDL_GL_GetProcAddress(procName);
    }
}