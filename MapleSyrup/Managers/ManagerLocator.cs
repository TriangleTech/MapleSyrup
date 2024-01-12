using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Managers;

/// <summary>
/// Contains the various managers used through the engine.
/// </summary>
public class ManagerLocator
{
    private readonly Scene _scene;
    private readonly List<IManager> _managers;

    public Scene Scene => _scene;
    public GraphicsDevice GraphicsDevice => _scene.GraphicsDevice;

    public ManagerLocator(Scene scene)
    {
        _scene = scene;
        _managers = new();
    }

    /// <summary>
    /// Registers a manager to the locator
    /// </summary>
    /// <param name="manager"></param>
    public void RegisterManager(IManager manager)
    {
        if (_managers.Contains(manager))
            return;
        _managers.Add(manager);
    }

    /// <summary>
    /// Initializes all managers added.
    /// </summary>
    public void Initialize()
    {
        for (int i = 0; i < _managers.Count; i++)
            _managers[i].Initialize(this);
    }

    /// <summary>
    /// Gets a manager from the list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public T GetManager<T>() where T : IManager
    {
        return (T)_managers.Find(mgr => mgr is T) ?? throw new NullReferenceException();
    }

    /// <summary>
    /// Shutdowns/Disposes all the managers.
    /// </summary>
    public void Shutdown()
    {
        for (int i = 0; i < _managers.Count; i++)
            _managers[i].Shutdown();
    }
}