using System.Collections.Concurrent;
using MapleSyrup.Resources;
using MapleSyrup.Resources.Nx;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Factories;

public class ResourceFactory : IFactory, IDisposable
{
    public Dictionary<string, Texture2D> textureCache = new();
    private Dictionary<string, NxFile> nxCache = new();
    private ResourceBackend backend;

    public ResourceFactory(ResourceBackend resourceBackend)
    {
        backend = resourceBackend;
    }

    public void Initialize()
    {
        // TODO: Add different backends to this
        
        foreach (var file in Directory.GetFiles("/home/beray/mapledev/v62/v62_nx"))
        {
            nxCache.Add(Path.GetFileNameWithoutExtension(file), new NxFile(file));
        }
    }

    public void Shutdown()
    {
        
    }

    public Texture2D CreateTexture(string texturePath)
    {
        switch (backend)
        {
            case ResourceBackend.Nx:
                return LoadNxTexture(texturePath);
            default:
                throw new NotImplementedException("No other backend is supported");
        }
    }

    private Texture2D LoadNxTexture(string texturePath)
    {
        var splitPath = texturePath.Split("/");
        var resource = nxCache[splitPath[0]];
        NxNode node = resource.BaseNode;

        for (int i = 1; i < splitPath.Length; i++)
            node = node[splitPath[i]];
        
        Console.WriteLine(node.Name);

        return null;
    }

    public void DeleteTexture(string texturePath)
    {
        if (texturePath == string.Empty)
            return;
        textureCache[texturePath].Dispose();
        _ = textureCache.Remove(texturePath);
    }

    public void Clear()
    {
        foreach (var tex in textureCache.Values)
            tex.Dispose();
        textureCache.Clear();
    }

    public void Dispose()
    {
        Clear();
        foreach (var nx in nxCache.Values)
        {
            nx.Dispose();
        }
        nxCache.Clear();
    }
}