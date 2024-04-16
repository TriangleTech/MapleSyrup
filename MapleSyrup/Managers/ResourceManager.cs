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
        _nxFiles["Mob"] = new NxFile("/home/beray/mapledev/v62/v62_nx/Mob.nx", _graphics);
        _nxFiles["Map"] = new NxFile("/home/beray/mapledev/v62/v62_nx/Map.nx", _graphics);
    }

    public NxFile this[string name] => _nxFiles[name];

    public void Destroy()
    {
        foreach (var (_, nx) in _nxFiles)
            nx.Dispose();
    }
}