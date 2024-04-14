using MapleSyrup.GameObjects.Components;
using MapleSyrup.Nx;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Managers;

public class ResourceManager
{
    private static ResourceManager _instance;
    private readonly GraphicsDevice _graphics;
    private readonly Dictionary<string, NxFile> _nxFiles;

    public GraphicsDevice GraphicsDevice => _graphics;

    public static ResourceManager Instance => _instance;

    public ResourceManager(Application app)
    {
        _instance = this;
        _graphics = app.GraphicsDevice;
        _nxFiles = new();
    }

    public void Initialize()
    {
        _nxFiles["Mob"] = new NxFile("D:/v62/Mob.nx", _graphics);
    }

    public NxFile this[string name] => _nxFiles[name];

    public void Destroy()
    {
        foreach (var (_, nx) in _nxFiles)
            nx.Dispose();
    }
}