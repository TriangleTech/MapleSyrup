using Raylib_CsLo;

namespace MSClient.Managers;

public class ResourceManager : IManager
{
    private readonly Dictionary<string, Texture> _textureCache;
    
    public ResourceManager()
    {
        _textureCache = new();
    }

    public void Initialize()
    {
        
    }

    public void Shutdown()
    {
    }
}