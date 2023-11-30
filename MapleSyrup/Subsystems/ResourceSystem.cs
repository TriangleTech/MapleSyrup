using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.Resources.Nx;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Subsystems;

public class ResourceSystem : ISubsystem
{
    private Dictionary<string, Texture2D> textureCache;
    public GameContext Context { get; private set; }
    public void Initialize(GameContext context)
    {
        Context = context;
        textureCache = new();
        Context.RegisterEventHandler(EventType.WorldChanged, OnWorldChanged);
    }

    public void Shutdown()
    {
        foreach (var tex in textureCache.Values)
            tex.Dispose();
        
        textureCache.Clear();
    }

    public Texture2D Get(string path)
    {
        if (textureCache.TryGetValue(path, out var texture))
            return texture;
        return LoadTexture(path);
    }

    private Texture2D LoadTexture(string path)
    {
        // TODO: Different resource systems ie nx, wz, etc.
        var nx = Context.GetSubsystem<NxSystem>();
        var split = path.Split("/");
        var node = nx.Get(split[0]).BaseNode;
        for (int i = 1; i < split.Length; i++)
            node = node[split[i]];
        textureCache[path] = node.To<NxBitmapNode>().GetTexture(Context.GraphicsDevice);
        
        return textureCache[path];
    }

    private void OnWorldChanged(EventData data)
    {
        foreach (var tex in textureCache.Values)
            tex.Dispose();
        
        textureCache.Clear();
    }
}