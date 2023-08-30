using OpenTK;
using SDL2;

namespace MapleSyrup.Window;

public class SDLBindingsContext : IBindingsContext
{
    public IntPtr GetProcAddress(string procName)
    {
        return SDL.SDL_GL_GetProcAddress(procName);
    }
}