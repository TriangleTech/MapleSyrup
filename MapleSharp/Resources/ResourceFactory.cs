using MapleSharp.Events.Experimental;
using MapleSharp.Graphics;
using MapleSharp.Services;

namespace MapleSharp.Resources;

public class ResourceFactory
{
    private readonly Dictionary<string, Texture> textures = new();
    private readonly EventFactory eventFactory;
    
    public ResourceFactory()
    {
        eventFactory = ServiceFactory.Instance.GetService<EventFactory>();
        eventFactory.RegisterEvent(new TextureRequestHandler(this));
    }
    
    public void RegisterTexture(string path, Texture texture)
    {
        if (textures.ContainsKey(path))
            return;
        textures.Add(path, texture);
    }
}