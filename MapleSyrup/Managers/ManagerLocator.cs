namespace MapleSyrup.Managers;

public class ManagerLocator
{
    private readonly Scene _scene;
    private readonly List<IManager> _managers;

    public Scene Scene => _scene;

    public ManagerLocator(Scene scene)
    {
        _scene = scene;
        _managers = new();
    }

    public void RegisterManager(IManager manager)
    {
        if (_managers.Contains(manager))
            return;
        _managers.Add(manager);
    }

    public void Initialize()
    {
        for (int i = 0; i < _managers.Count; i++)
            _managers[i].Initialize(this);
    }

    public T GetManager<T>() where T : IManager
    {
        return (T)_managers.Find(mgr => mgr is T) ?? throw new NullReferenceException();
    }

    public void Shutdown()
    {
        for (int i = 0; i < _managers.Count; i++)
            _managers[i].Shutdown();
    }
}