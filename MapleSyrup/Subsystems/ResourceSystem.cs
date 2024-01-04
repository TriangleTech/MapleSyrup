using System.Diagnostics;
using System.Runtime.CompilerServices;
using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.Resource;
using MapleSyrup.Resource.Nx;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Subsystems;

public class ResourceSystem : ISubsystem
{
    public GameContext Context { get; private set; }
    private Dictionary<string, Texture2D> textures;
    private Dictionary<string, NxFile> nxFiles;
    private Dictionary<string, object> wzFiles;
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
                nxFiles["Character"] = new NxFile("D:/v62/Character.nx");
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
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Texture2D LoadTexture(string name)
    {
        if (textures.TryGetValue(name, out var lookup))
            return lookup;
        
        Texture2D texture;
        switch (resourceBackend)
        {
            case ResourceBackend.Nx:
                texture = LoadNxTexture(name);
                break;
            case ResourceBackend.Wz:
                texture = LoadWzTexture(name);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(resourceBackend), resourceBackend, null);
        }
        
        textures.Add(name, texture);
        return textures[name];
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Texture2D LoadNxTexture(string name)
    {
        var split = name.Split('/');
        var nxFile = nxFiles[split[0]];
        var node = nxFile.ResolvePath(name);
        if (node.Name != split.Last())
            return null;
        var texture = node.To<NxBitmapNode>().GetTexture(Context.GraphicsDevice);
        
        return texture;
    }
    
    private Texture2D LoadWzTexture(string name)
    {
        throw new NotImplementedException();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetNodeCount(string path)
    {
        switch (resourceBackend)
        {
            case ResourceBackend.Nx:
                return GetNxNodeCount(path);
            case ResourceBackend.Wz:
                return GetWzNodeCount(path);
            default:
                throw new ArgumentOutOfRangeException(nameof(resourceBackend), resourceBackend, null);
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetNxNodeCount(string path)
    {
        var split = path.Split('/');
        var nxFile = nxFiles[split[0]];
        var node = nxFile.ResolvePath(path);
        if (node.Name != split.Last())
            return 0;
        return node.Children.Count;
    }
    
    private int GetWzNodeCount(string path)
    {
        throw new NotImplementedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private (ResourceType resourceType, object data) GetItem(string path)
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
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private (ResourceType, object) GetNxItem(string path)
    {
        var split = path.Split('/');
        var nxFile = nxFiles[split[0]];
        var node = nxFile.ResolvePath(path);
        if (node.Name != split.Last())
            return (ResourceType.Unknown, null);
        
        switch (node.NodeType)
        {
            case NodeType.Bitmap:
                return (ResourceType.Image, LoadTexture(path));
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
            case NodeType.NoData:
                return (ResourceType.Directory, null);
            default:
                return (ResourceType.Unknown, null);
        }
    }
    
    private (ResourceType, object) GetWzItem(string path)
    {
        throw new NotImplementedException();
    }

    public (ResourceType resourceType, object data) GetMapInfo(string path)
    {
        return GetItem($"Map/Map/Map{path[0]}/{path}");
    }

    public (ResourceType resourceType, object data) GetBackground(string path)
    {
        return GetItem($"Map/Back/{path}");
    }

    public (ResourceType resourceType, object data) GetTile(string path)
    {
        return GetItem($"Map/Tile/{path}");
    }

    public (ResourceType resourceType, object data) GetObject(string path)
    {
        return GetItem($"Map/Obj/{path}");
    }

    public (ResourceType resourceType, object data) GetCharItem(string path)
    {

        return GetItem($"Character/{path}");
    }

    public (ResourceType resourceType, object data) GetMapHelper(string path)
    {
        return GetItem($"Map/MapHelper.img/{path}");
    }
}