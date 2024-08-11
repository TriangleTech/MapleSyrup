using System.Numerics;
using Raylib_CsLo;

namespace Client.Managers;

public class InputManager : IManager
{
    public void Initialize()
    {
    }

    public void Shutdown()
    {
    }

    public Vector2 MouseToScreen => Raylib.GetMousePosition();

    public Vector2 MouseToWorld
    {
        get
        {
            var world = ServiceLocator.Get<WorldManager>();
            return Raylib.GetScreenToWorld2D(MouseToScreen, world.GetCamera());
        }
    }

    public Rectangle MouseRec
    {
        get
        {
            return new Rectangle(MouseToWorld.X, MouseToWorld.Y, 1, 1);
        }
    }

    public bool IsKeyDown(KeyboardKey key) => Raylib.IsKeyDown(key);
    public bool IsKeyPressed(KeyboardKey key) => Raylib.IsKeyPressed(key);
    public bool IsKeyRelease(KeyboardKey key) => Raylib.IsKeyReleased(key);
    public bool IsKeyUp(KeyboardKey key) => Raylib.IsKeyUp(key);
    public float GetMouseWheel() => Raylib.GetMouseWheelMove();
}