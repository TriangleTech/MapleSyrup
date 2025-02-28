using System.Collections.Concurrent;
using System.Numerics;
using System.Text.Json;
using MapleSyrup.Common.Map;
using MapleSyrup.NX;
using ZeroElectric.Vinculum;

namespace MapleSyrup.Resources;

public class ResourceFactory
{
    private readonly ConcurrentDictionary<string, ResourceBase> _resources;
    private readonly ConcurrentDictionary<string, NXFile> _nxFiles;
    private readonly ConcurrentQueue<ResourceBase> _resourceQueue;
    private const int ResourceCapacity = 2048; // TODO: Increase this as needed.
    private const string DefaultPath = "D:/v41";
    
    public static ResourceFactory Shared { get; private set; }

    public ResourceFactory()
    {
        Shared = this;
        _resources = new ConcurrentDictionary<string, ResourceBase>(Environment.ProcessorCount, ResourceCapacity);
        _resourceQueue = new();
        _nxFiles = new(Environment.ProcessorCount, 16)
        {
            ["Character"] = new(string.Concat(DefaultPath, "/Character.nx")),
            ["Effect"] = new(string.Concat(DefaultPath, "/Effect.nx")),
            ["Etc"] = new(string.Concat(DefaultPath, "/Etc.nx")),
            ["Map"] = new(string.Concat(DefaultPath, "/Map.nx")),
            ["Mob"] = new(string.Concat(DefaultPath, "/Mob.nx")),
            ["Npc"] = new(string.Concat(DefaultPath, "/Npc.nx")),
            ["Quest"] = new(string.Concat(DefaultPath, "/Quest.nx")),
            ["Reactor"] = new(string.Concat(DefaultPath, "/Reactor.nx")),
            ["Skill"] = new(string.Concat(DefaultPath, "/Skill.nx")),
            ["Sound"] = new(string.Concat(DefaultPath, "/Sound.nx")),
            ["TamingMob"] = new(string.Concat(DefaultPath, "/TamingMob.nx")),
            ["UI"] = new(string.Concat(DefaultPath, "/UI.nx"))
        };
    }

    public NXNode? GetNode(string file, string path)
    {
        return _nxFiles[file].GetNode(path);
    }

    /// <summary>
    /// Gets a node without verifying if it's the correct one. Only use this for '.img' nodes that are NOT in Map.nx EXCEPT for MAP IMGS (100000000.img is unique).
    /// Every other NX files does not have repeating node names. TODO: Verify.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="img"></param>
    /// <returns></returns>
    public NXNode? GetFastImg(string file, string img)
    {
        return _nxFiles[file].GetFastImg(img);
    }

    public NXNode? GetChildNode(string file, NXNode parent, string childName)
    {
        return _nxFiles[file].GetChildNode(parent, childName);
    }

    public MapleMap LoadMapData(string mapName)
    {
        using var json = File.OpenRead($"Custom/MapData/{mapName}.json");
        var map = JsonSerializer.Deserialize(json, MapleMapContext.Default.MapleMap) 
                  ?? throw new NullReferenceException("Failed to load map data");
        return map;
    }

    public void LoadResource(ResourceBase resource)
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
                
                resource.Texture = GetNode(resource.Directory, resource.ResourceName)?.GetTexture() 
                              ?? throw new NullReferenceException();

                _resources.TryAdd(resource.ResourceName, resource);
            }
        }
    }

    public T GetResource<T>(string resourceName) where T : ResourceBase
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
            
            foreach (var (_, nx) in _nxFiles)
                nx.Dispose();
            _nxFiles.Clear();
        }
    }
}