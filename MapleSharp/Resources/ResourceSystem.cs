using MapleSharp.Core;
using MapleSharp.Core.Interface;
using MapleSharp.Graphics;
using MapleSharp.NX;

namespace MapleSharp.Resources;

public class ResourceSystem : ISubsystem
{
    private readonly Dictionary<string, Texture> textureCache = new();
    private readonly Dictionary<string, Shader> shaderCache = new();
    private readonly NxSystem nxSystem;

    public ResourceSystem(Engine engine)
    {
        //nxSystem = engine.GetSubsystem<NxSystem>();
    }
    
    public void Initialize()
    {
    }

    public void Update(float timeDelta)
    {
    }

    public void Shutdown()
    {
        textureCache.Clear();
        shaderCache.Clear();
    }
    
    public Texture LoadTexture(string texturePath)
    {
        if (textureCache.TryGetValue(texturePath.ToLower(), out var requestedTexture))
        {
            return requestedTexture;
        }
        
        var image = nxSystem.ResolvePath<BitmapNode>(texturePath).GetBitmap();
        var texture = new Texture(image);
        textureCache.Add(texturePath.ToLower(), texture);
        
        return texture;
    }
    
    public Texture GetTexture(object texturePath)
    {
        if (texturePath is not string)
        {
            throw new ArgumentException($"[ResourceSystem] Texture path must be a string. ({texturePath})");
        }
        
        if (textureCache.TryGetValue(((string)texturePath).ToLower(), out var requestedTexture))
        {
            return requestedTexture;
        }

        return LoadTexture((string)texturePath);
    }
    
    public Shader LoadShader(string resourceName, string vertexShader, string fragmentShader, string geometryShader = null)
    {
        var shader = new Shader(vertexShader, fragmentShader, geometryShader);
        shaderCache.Add(resourceName, shader);
        return shader;
    }
    
    public Shader GetShader(string resourceName)
    {
        if (shaderCache.TryGetValue(resourceName, out var requestedShader))
        {
            return requestedShader;
        }
        
        throw new NullReferenceException($"[ResourceSystem] Attempted to get shader that does not exist. ({resourceName})");
    }
}