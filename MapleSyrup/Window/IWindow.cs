using MapleSyrup.Graphics;
using MapleSyrup.Core;

namespace MapleSyrup.Window;

public interface IWindow
{
    string Title { get; set; }
    int Width { get; set; }
    int Height { get; set; }
    bool VSync { get; set; }
    bool IsRunning { get; }
    IntPtr Handle { get; }
    Engine Engine { get; }
    void Initialize();
    void OnLoad();
    void OnRender();
    void OnUpdate(float timeDelta);
    void OnUnload();
    void Run();
    IWindow AddPlugin(string pluginName, params object[] args);
    IWindow AddDependency(string pluginName, params object[] args);
}