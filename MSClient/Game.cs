using MapleSharp.Window;

namespace MSClient;

public class Game : GameWindow
{
    public Game()
        : base()
    {

    }
    
    public override void OnLoad()
    {
        base.OnLoad();
    }
    
    public override void OnRender()
    {
        GraphicsDevice.Clear(0.2f, 0.2f, 0.2f, 1.0f);
        GraphicsDevice.SwapBuffers();
        base.OnRender();
    }
    
    public override void OnUpdate(float timeDelta)
    {
        base.OnUpdate(timeDelta);
    }
    
    public override void OnUnload()
    {
        base.OnUnload();
    }
}