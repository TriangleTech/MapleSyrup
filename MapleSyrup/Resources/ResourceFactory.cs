using System.Collections;
using System.Collections.Concurrent;
using MapleSyrup.Nx;

namespace MapleSyrup.Resources;

public class ResourceFactory
{
    private readonly ConcurrentDictionary<string, IResource> _resources;
    private readonly ConcurrentQueue<IResource> _resourceQueue;
    private const int ResourceCapacity = 2048; // TODO: Increase this as needed.
    
    public static ResourceFactory Shared { get; private set; }

    public ResourceFactory()
    {
        Shared = this;
        _resources = new ConcurrentDictionary<string, IResource>(Environment.ProcessorCount, ResourceCapacity);
        _resourceQueue = new();
    }

    public void RegisterResource(IResource resource)
    {
        lock (this)
        {
            _resourceQueue.Enqueue(resource);
        }
    }

    public void LoadPendingTextures()
    {
        lock (this)
        {
            while (!_resourceQueue.IsEmpty)
            {
                _resourceQueue.TryDequeue(out var resource);
                if (resource == null) continue;
                
                var texture = NXFactory.Shared.GetNode(resource.MainFile, resource.Name)?.GetTexture() ?? throw new NullReferenceException();
                if (resource is TextureResource textureResource)
                {
                    textureResource.Texture = texture;
                }

                _resources.TryAdd(resource.Name, resource);
            }
        }
    }

    public T GetResource<T>(string resourceName) where T : IResource
    {
        lock (this)
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
    }

    public void DestroyResource(string resourceName)
    {
        lock (this)
        {
            if (!_resources.TryGetValue(resourceName, out var resource))
                return;
            _resources.TryRemove(resourceName, out resource);
            resource?.Destroy();
        }
    }

    public bool HasResource(string resourceName)
    {
        lock (this)
        {
            return _resources.TryGetValue(resourceName, out _);
        }
    }

    public void ShutDown()
    {
        lock (this)
        {
            foreach (var resource in _resources.Values)
                resource.Destroy();
            _resources.Clear();
        }
    }
}