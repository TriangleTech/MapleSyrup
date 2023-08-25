using MapleSharp.Graphics;

namespace MapleSharp.Window;

public interface IWindow
{
    string Title { get; set; }
    int Width { get; set; }
    int Height { get; set; }
    bool VSync { get; set; }
    bool IsRunning { get; }
    IntPtr Handle { get; }
    GraphicsDevice GraphicsDevice { get; set; }
    void Initialize();
    void OnLoad();
    void OnRender();
    void OnUpdate(float timeDelta);
    void OnUnload();
    void Run();
    IWindow AddPlugin(string pluginName, params object[] args);
    IWindow AddDependency(string pluginName, params object[] args);
}