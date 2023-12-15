using System.Diagnostics;
using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.Resources;
using MapleSyrup.Resources.Nx;
using Microsoft.Xna.Framework.Graphics;
using reWZ;
using reWZ.WZProperties;

namespace MapleSyrup.Subsystems;

public class ResourceSystem : ISubsystem
{
    public GameContext Context { get; private set; }
    private Dictionary<string, Texture2D> textures;
    private Dictionary<string, NxFile> nxFiles;
    private Dictionary<string, WZFile> wzFiles;
    private ResourceBackend resourceBackend;
    
    public void Initialize(GameContext context)
    {
        Context = context;
        textures = new();
    }

    public void Shutdown()
    {
        textures.Clear();
        textures = null;
        switch (resourceBackend)
        {
            case ResourceBackend.Nx:
                foreach (var (_, nxFile) in nxFiles)
                {
                    nxFile.Dispose();
                }
                nxFiles.Clear();
                nxFiles = null;
                break;
            case ResourceBackend.Wz:
                wzFiles.Clear();
                wzFiles = null;
                break;
        }
    }
    
    public void SetBackend(ResourceBackend type)
    {
        resourceBackend = type;
        switch (resourceBackend)
        {
            case ResourceBackend.Nx:
                nxFiles = new();
                nxFiles["Map"] = new NxFile("D:/v62/Map.nx"); 
                //LoadNxFiles();
                break;
            case ResourceBackend.Wz:
                wzFiles = new();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(resourceBackend), resourceBackend, null);
        }
    }
    
    private void LoadNxFiles()
    {
        var files = Directory.GetFiles("D:/v62/", "*.nx", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            var nxFile = new NxFile(file);
            nxFiles.Add(Path.GetFileNameWithoutExtension(file), nxFile);
        }
    }
    
    public Texture2D GetTexture(string name)
    {
        if (textures.TryGetValue(name, out var lookup))
            return lookup;

        var texture = LoadTexture(name);
        textures.Add(name, texture);
        return texture;
    }
    
    private Texture2D LoadTexture(string name)
    {
        switch (resourceBackend)
        {
            case ResourceBackend.Nx:
                return LoadNxTexture(name);
            case ResourceBackend.Wz:
                return LoadWzTexture(name);
            default:
                throw new ArgumentOutOfRangeException(nameof(resourceBackend), resourceBackend, null);
        }
    }
    
    private Texture2D LoadNxTexture(string name)
    {
        throw new NotImplementedException();
    }
    
    private Texture2D LoadWzTexture(string name)
    {
        throw new NotImplementedException();
    }

    public (ResourceType resourceType, object data) GetItem(string path)
    {
        switch (resourceBackend)
        {
            case ResourceBackend.Nx:
                return GetNxItem(path);
            case ResourceBackend.Wz:
                return GetWzItem(path);
            default:    
                throw new ArgumentOutOfRangeException(nameof(resourceBackend), resourceBackend, null);
        }
    }
    
    private (ResourceType, object) GetNxItem(string path)
    {
        var split = path.Split('/');
        var nxFile = nxFiles[split[0]];
        var node = nxFile.ResolvePath(path);
        switch (node.NodeType)
        {
            case NodeType.Bitmap:
                return (ResourceType.Image, GetTexture(path));
            case NodeType.Vector:
                return (ResourceType.Vector, node.GetData<NxVectorNode>());
            case NodeType.Audio:
                return (ResourceType.Audio, node.GetData<NxAudioNode>());
            case NodeType.String:
                return (ResourceType.String, node.GetData<NxStringNode>());
            case NodeType.Int64:
                return (ResourceType.Integer, node.GetData<NxIntNode>());
            case NodeType.Double:
                return (ResourceType.Double, node.GetData<NxDoubleNode>());
            default:
                throw new ArgumentOutOfRangeException(nameof(node.NodeType), node.NodeType, null);
        }
    }
    
    private (ResourceType, object) GetWzItem(string path)
    {
        throw new NotImplementedException();
    }
}