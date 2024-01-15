using MapleSyrup.EC;
using MapleSyrup.EC.Components;
using MapleSyrup.Nx;
using MapleSyrup.Player;
using MapleSyrup.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Managers;

public class ResourceManager : IManager
{
    private ManagerLocator? _locator;
    private Dictionary<string, NxFile> _nxFiles;
    private Dictionary<string, Texture2D> _textureCache;
    private ResourceBackend _resourceBackend;

    public ResourceManager(ResourceBackend backend)
    {
        _resourceBackend = backend;
        _nxFiles = new();
        _textureCache = new Dictionary<string, Texture2D>();
    }

    public void Initialize(ManagerLocator locator)
    {
        _locator = locator;
        _nxFiles["Map"] = new NxFile("D:/v41/Map.nx"); // Couldn't find v40 :(
        _nxFiles["Character"] = new NxFile("D:/v41/Character.nx");
        //LoadNxFiles();
    }

    private void LoadNxFiles()
    {
        var files = Directory.GetFiles("D:/v41/", "*.nx");
        foreach (var file in files)
            _nxFiles[Path.GetFileNameWithoutExtension(file)] = new NxFile(file);
    }
    
    public void Shutdown()
    {
        foreach (var (_, texture) in _textureCache)
        {
            texture.Dispose();
        }

        _textureCache.Clear();
        switch (_resourceBackend)
        {
            case ResourceBackend.Nx:
                foreach (var (_, nxFile) in _nxFiles)
                {
                    nxFile.Dispose();
                }

                _nxFiles.Clear();
                break;
        }
    }
    
    #region Items Search & Obtain

    /// <summary>
    /// Checks if the specified path contains the node, and returns the data if it does.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="Exception"></exception>
    public bool Contains(string path, out object? data)
    {
        switch (_resourceBackend)
        {
            case ResourceBackend.Nx:
                return SearchNxPath(path, out data, out _);
            case ResourceBackend.Wz:
                throw new NotImplementedException();
        }

        throw new Exception("Resource Backend not set");
    }

    /// <summary>
    /// Searches the NX File for the specified path.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="data"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private bool SearchNxPath(string path, out object? data, out int count)
    {
        var split = path.Split("/");
        var nxFile = _nxFiles[split[0]];
        var root = nxFile.BaseNode;

        for (int i = 1; i < split.Length; i++)
        {
            if (!root.HasChild(split[i]))
            {
                data = null;
                count = -1;
                return false;
            }

            root = root[split[i]];
        }

        switch (root.NodeType)
        {
            case NodeType.Audio:
                throw new NotImplementedException();
            case NodeType.Bitmap:
                data = root.To<NxBitmapNode>().GetTexture(_locator.Scene.GraphicsDevice);
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
                return true;
            default:
                data = null;
                count = -1;
                return false;
        }

        return false;
    }

    /// <summary>
    /// Retrieves a texture from the cache or loads it from memory.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="Exception"></exception>
    private Texture2D? GetTexture(string path)
    {
        if (_textureCache.TryGetValue(path, out var tex))
            return tex;

        switch (_resourceBackend)
        {
            case ResourceBackend.Nx:
                SearchNxPath(path, out var texture, out _);
                if (texture is not Texture2D data)
                    return null;
                return data;
            case ResourceBackend.Wz:
                throw new NotImplementedException();
        }

        throw new Exception("Failed to retrieve texture");
    }

    /// <summary>
    /// Gets a background texture
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public Texture2D? GetBackground(string path)
    {
        return GetTexture($"Map/Back/{path}");
    }

    /// <summary>
    /// Gets a Map Object texture
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public Texture2D? GetMapObject(string path)
    {
        return GetTexture($"Map/Obj/{path}");
    }

    /// <summary>
    /// Gets a tile texture
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public Texture2D? GetTile(string path)
    {
        return GetTexture($"Map/Tile/{path}");
    }

    /// <summary>
    /// Gets a portal texture
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public Texture2D GetPortal(string path)
    {
        return GetTexture($"Map/MapHelper.img/portal/game/{path}");
    }

    /// <summary>
    /// Loads all the map data (i.e. tileSets, x, y) within a Map Image.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public VariantMap<string, string, object> LoadMapData(string path)
    {
        var truePath = $"Map/Map/Map{path[0]}/{path}";
        var nxFile = _nxFiles["Map"];
        var root = nxFile.BaseNode;
        var split = truePath.Split("/");
        var variant = new VariantMap<string, string, object>();

        for (int i = 1; i < split.Length; i++)
        {
            root = root[split[i]];
        }

        foreach (var nextChild in root.Children.Values)
        foreach (var lastChild in nextChild.Children.Values)
        {
            if (_resourceBackend == ResourceBackend.Nx)
            {
                switch (lastChild.NodeType)
                {
                    case NodeType.Double:
                        variant[nextChild.Name, lastChild.Name] = lastChild.To<NxDoubleNode>().GetDouble();
                        break;
                    case NodeType.Int64:
                        variant[nextChild.Name, lastChild.Name] = lastChild.To<NxIntNode>().GetInt();
                        break;
                    case NodeType.String:
                        variant[nextChild.Name, lastChild.Name] = lastChild.To<NxStringNode>().GetString();
                        break;
                    case NodeType.Vector:
                        variant[nextChild.Name, lastChild.Name] = lastChild.To<NxVectorNode>().GetVector();
                        break;
                    case NodeType.NoData:
                        break;
                }
            }
        }

        return variant;
    }

    /// <summary>
    /// Loads the Map information in the "info" node of the Map Image.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public VariantMap<string, string, object> LoadMapInfo(string path)
    {
        var truePath = $"Map/Map/Map{path[0]}/{path}/info";
        var nxFile = _nxFiles["Map"];
        var root = nxFile.BaseNode;
        var split = truePath.Split("/");
        var variant = new VariantMap<string, string, object>();

        for (int i = 1; i < split.Length; i++)
        {
            root = root[split[i]];
        }

        foreach (var node in root.Children.Values)
        {
            if (_resourceBackend == ResourceBackend.Nx)
            {
                switch (node.NodeType)
                {
                    case NodeType.Double:
                        variant["info", node.Name] = node.To<NxDoubleNode>().GetDouble();
                        break;
                    case NodeType.Int64:
                        variant["info", node.Name] = node.To<NxIntNode>().GetInt();
                        break;
                    case NodeType.String:
                        variant["info", node.Name] = node.To<NxStringNode>().GetString();
                        break;
                    case NodeType.Vector:
                        variant["info", node.Name] = node.To<NxVectorNode>().GetVector();
                        break;
                    case NodeType.NoData:
                        break;
                }
            }
        }

        return variant;
    }

    /// <summary>
    /// Gets the node count for the specified path.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public int GetNodeCount(string path)
    {
        switch (_resourceBackend)
        {
            case ResourceBackend.Nx:
                SearchNxPath(path, out _, out var count);
                return count;
        }

        return -1;
    }

    /// <summary>
    /// Returns the amount of backgrounds in an Map Image.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public int GetBackgroundCount(string path)
    {
        switch (_resourceBackend)
        {
            case ResourceBackend.Nx:
                SearchNxPath($"Map/Map/Map{path[0]}/{path}/back", out _, out var count);
                return count;
        }

        return -1;
    }

    /// <summary>
    /// Returns the amount of objects in a Map Image.
    /// </summary>
    /// <param name="img">Image</param>
    /// <returns></returns>
    public int GetObjectCount(string img)
    {
        switch (_resourceBackend)
        {
            case ResourceBackend.Nx:
                SearchNxPath($"Map/Map/Map{img[0]}/{img}/obj", out _, out var count);
                return count;
        }

        return -1;
    }

    /// <summary>
    /// Returns the amount of tiles within a Map Image.
    /// </summary>
    /// <param name="img">Image path</param>
    /// <returns></returns>
    public int GetTileCount(string img)
    {
        switch (_resourceBackend)
        {
            case ResourceBackend.Nx:
                SearchNxPath($"Map/Map/Map{img[0]}/{img}/tile", out _, out var count);
                return count;
        }

        return -1;
    }

    /// <summary>
    /// Searches and obtains the origin of the item.
    /// </summary>
    /// <param name="fullPath">Full path to the origin. Must include Wz/Nx file.</param>
    /// <returns></returns>
    public Vector2? GetOrigin(string fullPath)
    {
        switch (_resourceBackend)
        {
            case ResourceBackend.Nx:
                SearchNxPath($"{fullPath}/origin", out var origin, out _);
                return (Vector2)origin;
        }

        return Vector2.Zero;
    }

    public BodyPart LoadBodyPart(IEntity entity, AvatarState state, BodyType type)
    {
        var player = entity as Avatar;
        var part = new BodyPart(player, type);
        var stateName = state.ToString();

        switch (type)
        {
            case BodyType.Arm:
                var armNode = _nxFiles["Character"][$"00002{player.SkinId}.img"][stateName];
                part.Texture = armNode["0"]["arm"].To<NxBitmapNode>().GetTexture(_locator.GraphicsDevice);
                
                foreach (var frameNode in armNode)
                {
                    var frame = Convert.ToInt32(frameNode.Name);
                    part.AddFrame(frame, 
                        armNode[frameNode.Name]["arm"]["origin"].To<NxVectorNode>().GetVector(), 
                        armNode[frameNode.Name]["arm"].To<NxBitmapNode>().GetTexture(_locator.GraphicsDevice));
                    part.AddBodyMap(frame, 
                        BodyMap.Navel, 
                        armNode[frameNode.Name]["arm"]["map"]["navel"].To<NxVectorNode>().GetVector());
                    part.AddBodyMap(frame,
                        BodyMap.Hand,
                        armNode[frameNode.Name]["arm"]["map"]["hand"].To<NxVectorNode>().GetVector());
                    if (frameNode.HasChild("delay"))
                        part.AddDelay(frameNode["delay"].To<NxIntNode>().GetInt());
                    else
                    {
                        part.AddDelay(150);
                    }
                }
                break;
            case BodyType.Body:
                var bodyNode = _nxFiles["Character"][$"00002{player.SkinId}.img"][stateName];
                part.Texture = bodyNode["0"]["body"].To<NxBitmapNode>().GetTexture(_locator.GraphicsDevice);

                foreach (var frameNode in bodyNode)
                {
                    var frame = Convert.ToInt32(frameNode.Name);
                    part.AddFrame(frame, 
                        bodyNode[frameNode.Name]["body"]["origin"].To<NxVectorNode>().GetVector(), 
                        bodyNode[frameNode.Name]["body"].To<NxBitmapNode>().GetTexture(_locator.GraphicsDevice));
                    part.AddBodyMap(frame, 
                        BodyMap.Navel, 
                        bodyNode[frameNode.Name]["body"]["map"]["navel"].To<NxVectorNode>().GetVector());
                    part.AddBodyMap(frame, 
                        BodyMap.Neck, 
                        bodyNode[frameNode.Name]["body"]["map"]["neck"].To<NxVectorNode>().GetVector());
                    
                    if (frameNode.HasChild("delay"))
                        part.AddDelay(frameNode["delay"].To<NxIntNode>().GetInt());
                    else
                    {
                        part.AddDelay(500);
                    }
                }
                break;
            case BodyType.Head:
                var frontNode = _nxFiles["Character"][$"00012{player.SkinId}.img"]["front"];
                var headNode = _nxFiles["Character"][$"00012{player.SkinId}.img"][stateName];
                var body = _nxFiles["Character"][$"00002{player.SkinId}.img"][stateName];
                part.Texture = headNode["0"]["head"].To<NxBitmapNode>().GetTexture(_locator.GraphicsDevice);
                
                foreach (var frameNode in headNode)
                {
                    var frame = Convert.ToInt32(frameNode.Name);
                    part.AddFrame(frame, 
                        headNode[frameNode.Name]["head"]["origin"].To<NxVectorNode>().GetVector(), 
                        headNode[frameNode.Name]["head"].To<NxBitmapNode>().GetTexture(_locator.GraphicsDevice));
                    part.AddBodyMap(frame,
                        BodyMap.NeckBody,
                        body[frameNode.Name]["body"]["map"]["neck"].To<NxVectorNode>().GetVector());
                    part.AddBodyMap(frame, 
                        BodyMap.Neck, 
                        frontNode["head"]["map"]["neck"].To<NxVectorNode>().GetVector());
                    part.AddBodyMap(frame, 
                        BodyMap.EarOverHead, 
                        frontNode["head"]["map"]["earOverHead"].To<NxVectorNode>().GetVector());
                    part.AddBodyMap(frame, 
                        BodyMap.EarBelowHead, 
                        frontNode["head"]["map"]["earBelowHead"].To<NxVectorNode>().GetVector());
                    part.AddBodyMap(frame, 
                        BodyMap.Brow, 
                        frontNode["head"]["map"]["brow"].To<NxVectorNode>().GetVector());
                    
                    if (frameNode.HasChild("delay"))
                        part.AddDelay(frameNode["delay"].To<NxIntNode>().GetInt());
                    else
                    {
                        part.AddDelay(500);
                    }
                }
                break;
        }

        return part;
    }

    #endregion
}

public enum ResourceBackend
{
    Nx,
    Wz
}