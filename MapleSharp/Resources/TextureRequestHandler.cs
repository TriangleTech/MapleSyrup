using MapleSharp.Events.Experimental;
using MapleSharp.Graphics;
using MapleSharp.Services;

namespace MapleSharp.Resources;

public class TextureRequestHandler : IEventHandler
{
    public EventType Type { get; set; }
    public Action<object> OnDispatchData { get; set; }
    public Func<object, object> OnRequestData { get; set; }
    
    private readonly ResourceFactory resourceFactory;
    
    public TextureRequestHandler(ResourceFactory resourceFactory)
    {
        this.resourceFactory = resourceFactory;
        Type = EventType.TextureRequest;
        OnDispatchData = null;
        OnRequestData = RequestData;
    }

    private object RequestData(object arg)
    {
        if (arg is not string path)
            throw new Exception("[TextureRequestHandler] Invalid argument type.");

        var nx = ServiceFactory.Instance.GetService<NxFactory>();
        var file = nx["effect"];
        var image = file.GetImage(path);
        var texture = new Texture(image);
        resourceFactory.RegisterTexture(path, texture);
        return texture;
    }
}