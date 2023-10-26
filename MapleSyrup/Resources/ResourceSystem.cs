using MapleSyrup.Core;
using MapleSyrup.Core.Interface;
using MapleSyrup.Graphics;
using MapleSyrup.Nodes;
using MapleSyrup.NX;

namespace MapleSyrup.Resources;

public class ResourceSystem : ISubsystem
{
    private readonly Dictionary<string, Image> imageCache = new();
    private readonly Dictionary<string, TextureNode> textureCache = new();
    private readonly Dictionary<string, Shader> shaderCache = new();
    private readonly NxSystem nxSystem;

    public ResourceSystem(Engine engine)
    {
        nxSystem = engine.GetSubsystem<NxSystem>();
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
    
    public Image LoadImage(string imagePath)
    {
        var splitPath = imagePath.Split('/');
        var nxNode = nxSystem.ResolvePath(imagePath);
        var image = nxNode.GetImage();
        if (image == null)
            throw new NullReferenceException("[ResourceSystem] Failed to load image.");
        imageCache.Add(imagePath.ToLower(), image);
        return image;
    }
    
    public Image GetImage(string imagePath)
    {
        if (imageCache.TryGetValue(imagePath.ToLower(), out var requestedImage))
        {
            return requestedImage;
        }

        return LoadImage(imagePath);
    }

    private TextureNode LoadTexture(string texturePath)
    {
        if (textureCache.TryGetValue(texturePath.ToLower(), out var requestedTexture))
        {
            return requestedTexture;
        }
        
        var image = nxSystem.ResolvePath(texturePath).GetImage();
        var texture = new TextureNode(image);
        if (texture == null)
            throw new NullReferenceException("[ResourceSystem] Failed to load texture.");
        textureCache.Add(texturePath.ToLower(), texture);
        
        return texture;
    }
    
    public TextureNode GetTexture(object texturePath)
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