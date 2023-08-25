namespace MapleSharp.Services;

public class ServiceFactory
{
    private List<object> services;
    private static ServiceFactory instance;
    
    public static ServiceFactory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ServiceFactory();
            }

            return instance;
        }
    }
    
    public ServiceFactory()
    {
        services = new ();
        instance = this;
    }
    
    public T GetService<T>() where T : class
    {
        var service = services.Find(x => x.GetType() == typeof(T));
        if (service == null)
        {
            service = Activator.CreateInstance<T>();
            services.Add(service);
        }
        return (T)service;
    }
}