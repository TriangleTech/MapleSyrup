namespace MapleSyrup.Factories;

public class ServiceFactory
{
    private List<IFactory> factories = new();
    private static ServiceFactory instance;

    public static ServiceFactory Shared => instance;

    public ServiceFactory()
    {
        instance = this;
    }

    public void AddFactory<T>() where T : IFactory
    {
        if (factories.Any(x => x is T))
            return;
        var factory = Activator.CreateInstance<T>();
        factory.Initialize();
        
        factories.Add(factory);
    }

    public T GetFactory<T>()
    {
        return (T)factories.Find(x => x is T) ?? throw new NullReferenceException();
    }
}