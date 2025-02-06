using System.Collections;

namespace Client.Resources;

public class ResourceFactory
{
    private readonly Dictionary<string, IResource> _resources;
    private const int ResourceCapacity = 2048; // TODO: Increase this as needed.
    
    public static ResourceFactory Shared { get; private set; }

    public ResourceFactory()
    {
        Shared = this;
        _resources = new Dictionary<string, IResource>(ResourceCapacity);
    }

    public void RegisterResource(IResource resource)
    {
        _resources.TryAdd(resource.Name, resource);
    }

    public T GetResource<T>(string resourceName) where T : IResource
    {
        try
        {
            if (_resources.TryGetValue(resourceName, out var resource))
                return (T)resource;
        }
        catch (Exception)
        {
            // ignored
        }
        
        // TODO: Fallback on a default resource, so the client doesn't crash.
        throw new KeyNotFoundException($"Resource [{resourceName}] not registered");
    }

    public void DestroyResource(string resourceName)
    {
        if (!_resources.TryGetValue(resourceName, out var resource))
            return;
        resource.Destroy();
        _resources.Remove(resourceName);
    }

    public bool HasResource(string resourceName)
    {
        return _resources.TryGetValue(resourceName, out _);
    }

    public void ShutDown()
    {
        foreach (var resource in _resources.Values)
            resource.Destroy();
        _resources.Clear();
    }
}