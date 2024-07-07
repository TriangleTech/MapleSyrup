namespace MSClient.Managers;

public static class ServiceLocator
{
    private static readonly HashSet<IManager> _managers = new();

    public static void Register<T>(T manager) where T : IManager
    {
        manager.Initialize();
        _managers.Add(manager);
    }

    public static T Get<T>() where T : IManager
    {
        return (T)_managers.First(x => x is T);
    }

    public static void Release()
    {
        foreach (var manager in _managers)
        {
            manager.Shutdown();
        }
        
        _managers.Clear();
    }
}