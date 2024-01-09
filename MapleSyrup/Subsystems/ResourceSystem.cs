using System.Diagnostics;
using System.Runtime.CompilerServices;
using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.Resource;
using MapleSyrup.Resource.Nx;
using MapleSyrup.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Subsystems;

public class ResourceSystem : ISubsystem
{
    public GameContext Context { get; private set; }
    private readonly Dictionary<string, Texture2D> textureCache;
    private readonly Dictionary<string, NxFile> nxFiles;
    private ResourceBackend resourceBackend;

    public ResourceSystem()
    {
        textureCache = new();
        nxFiles = new();
        resourceBackend = ResourceBackend.Nx;
    }

    public void Initialize(GameContext context)
    {
        Context = context;
        nxFiles["Map"] = new NxFile("D:/v62/Map.nx");
        //LoadNxFiles();
    }

    private void LoadNxFiles()
    {
        var files = Directory.GetFiles("D:/v62/", ".nx");

        foreach (var file in files)
            nxFiles[Path.GetFileNameWithoutExtension(file)] = new NxFile(file);
    }

    public void Shutdown()
    {

    }

    public bool Contains(string path, out object? data)
    {
        switch (resourceBackend)
        {
            case ResourceBackend.Nx:
                return SearchNxPath(path, out data, out _);
            case ResourceBackend.Wz:
                throw new NotImplementedException();
        }

        throw new Exception("Resource Backend not set");
    }

    private bool SearchNxPath(string path, out object? data, out int count)
    {
        var split = path.Split("/");
        var nxFile = nxFiles[split[0]];
        var root = nxFile.BaseNode;

        for (int i = 1; i < split.Length; i++)
        {
            root = root[split[i]];
        }
        
        switch (root.NodeType)
        {
            case NodeType.Audio:
                throw new NotImplementedException();
            case NodeType.Bitmap:
                data = root.To<NxBitmapNode>().GetTexture(Context.GraphicsDevice);
                count = root.ChildCount;
                return true;
            case NodeType.Double:
                data = root.To<NxDoubleNode>().GetDouble();
                count = root.ChildCount;
                return true;
            case NodeType.Int64:
                data = root.To<NxIntNode>().GetInt();
                count = root.ChildCount;
                return true;
            case NodeType.String:
                data = root.To<NxStringNode>().GetString();
                count = root.ChildCount;
                return true;
            case NodeType.Vector:
                data = root.To<NxVectorNode>().GetVector();
                count = root.ChildCount;
                return true;
            case NodeType.NoData:
                data = null;
                count = root.ChildCount;
                break;
            default:
                data = null;
                count = 0;
                return false;
        }

        return false;
    }

    private Texture2D? GetTexture(string path)
    {
        if (textureCache.TryGetValue(path, out var tex))
            return tex;

        switch (resourceBackend)
        {
            case ResourceBackend.Nx:
                SearchNxPath(path, out var texture, out _);
                if (texture is not Texture2D)
                    return null;
                return texture as Texture2D;
            case ResourceBackend.Wz:
                throw new NotImplementedException();
        }

        throw new Exception("Failed to retrieve texture");
    }

    public Texture2D? GetBackground(string path)
    {
        return GetTexture($"Map/Back/{path}");
    }

    public Texture2D? GetMapObject(string path)
    {
        return GetTexture($"Map/Obj{path}");
    }

    public Texture2D? GetTile(string path)
    {
        return GetTexture($"Map/Tile/{path}");
    }

    public VariantSet<object> LoadMapData(string path)
    {
        var truePath = $"Map/Map/Map{path[0]}/{path}";
        var nxFile = nxFiles["Map"];
        var root = nxFile.BaseNode;
        var split = truePath.Split("/");
        var variant = new VariantSet<object>();

        for (int i = 1; i < split.Length; i++)
        {
            root = root[split[i]];
        }
        
        foreach (var nextChild in root.Children.Values)
        foreach (var lastChild in nextChild.Children.Values)
        {
            if (resourceBackend == ResourceBackend.Nx)
            {
                switch (lastChild.NodeType)
                {
                    case NodeType.Double:
                        variant[nextChild.Name][lastChild.Name] = lastChild.To<NxDoubleNode>().GetDouble();
                        break;
                    case NodeType.Int64:
                        variant[nextChild.Name][lastChild.Name] = lastChild.To<NxIntNode>().GetInt();
                        break;
                    case NodeType.String:
                        variant[nextChild.Name][lastChild.Name] = lastChild.To<NxStringNode>().GetString();
                        break;
                    case NodeType.Vector:
                        variant[nextChild.Name][lastChild.Name] = lastChild.To<NxVectorNode>().GetVector();
                        break;
                    case NodeType.NoData:
                        break;
                }
            }
        }
        
        return variant;
    }
    
    public VariantSet<object> LoadMapInfo(string path)
    {
        var truePath = $"Map/Map/Map{path[0]}/{path}/info";
        var nxFile = nxFiles["Map"];
        var root = nxFile.BaseNode;
        var split = truePath.Split("/");
        var variant = new VariantSet<object>();

        for (int i = 1; i < split.Length; i++)
        {
            root = root[split[i]];
        }

        foreach(var node in root.Children.Values)
        {
            if (resourceBackend == ResourceBackend.Nx)
            {
                switch (node.NodeType)
                {
                    case NodeType.Double:
                        variant["info"][node.Name] = node.To<NxDoubleNode>().GetDouble();
                        break;
                    case NodeType.Int64:
                        variant["info"][node.Name] = node.To<NxIntNode>().GetInt();
                        break;
                    case NodeType.String:
                        variant["info"][node.Name] = node.To<NxStringNode>().GetString();
                        break;
                    case NodeType.Vector:
                        variant["info"][node.Name] = node.To<NxVectorNode>().GetVector();
                        break;
                    case NodeType.NoData:
                        break;
                }
            }
        }

        return variant;
    }

    public int GetBackgroundCount(string path)
    {
        switch (resourceBackend)
        {
            case ResourceBackend.Nx:
                SearchNxPath($"Map/Map/Map{path[0]}/{path}/back", out _, out var count);
                return count;
        }

        return -1;
    }


    public int GetObjectCount(string path)
    {
        switch (resourceBackend)
        {
            case ResourceBackend.Nx:
                SearchNxPath($"Map/Map/Map{path[0]}/{path}/obj", out _, out var count);
                return count;
        }
        return -1;
    }

    public int GetTileCount(string path)
    {
        switch (resourceBackend)
        {
            case ResourceBackend.Nx:
                SearchNxPath($"Map/Map/Map{path[0]}/{path}/tile", out _, out var count);
                return count;
        }

        return -1;
    }

    public Vector2? GetOrigin(string fullPath)
    {
        SearchNxPath($"{fullPath}/origin", out var origin, out _);

        return (Vector2)origin;
    }
}