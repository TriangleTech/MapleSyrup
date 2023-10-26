using MapleSyrup.Core.Interface;
using SDL2;

namespace MapleSyrup.Core.Input;

public class InputSystem : ISubsystem
{
    public void Initialize()
    {
    }

    public void Update(float timeDelta)
    {
    }

    public void Shutdown()
    {
    }
    
    public void OnKeyDown(SDL.SDL_Keycode key)
    {
        Engine.Instance.GetSubsystem<EventSystem>().TriggerEvent("OnKeyDown", data: key);
    }
    
    public void OnKeyUp(SDL.SDL_Keycode key)
    {
        Engine.Instance.GetSubsystem<EventSystem>().TriggerEvent("OnKeyUp", data: key);
    }
    
    public void OnMouseButtonDown(SDL.SDL_MouseButtonEvent button)
    {
        Engine.Instance.GetSubsystem<EventSystem>().TriggerEvent("OnMouseButtonDown", data: button);
    }
    
    public void OnMouseButtonUp(SDL.SDL_MouseButtonEvent button)
    {
        Engine.Instance.GetSubsystem<EventSystem>().TriggerEvent("OnMouseButtonUp", data: button);
    }
    
    public void OnMouseMotion(SDL.SDL_MouseMotionEvent motion)
    {
        //Engine.Instance.GetSubsystem<EventSystem>().TriggerEvent("OnMouseMotion", data: motion);
    }
    
    public void OnMouseWheel(SDL.SDL_MouseWheelEvent wheel)
    {
        Engine.Instance.GetSubsystem<EventSystem>().TriggerEvent("OnMouseWheel", data: wheel);
    }
}