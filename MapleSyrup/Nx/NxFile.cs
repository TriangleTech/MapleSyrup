using System.Runtime.CompilerServices;
using K4os.Compression.LZ4;
using MapleSyrup.GameObjects.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace MapleSyrup.Nx;

public unsafe class NxFile : IDisposable
{
    private NxReader _reader;
    private int _nodeCount, _stringCount, _bitmapCount, _audioCount;
    private long _nodeBlockOffset, _stringBlockOffset, _bitmapBlockOffset, _audioBlockOffset;
    private int _currentNodeOffset = 0, _pathCheck = 0, _currentChildCount = 0;
    private int _directoryOffset = 0;
    private string _currentName;
    private NodeType _currentNodeType;
    private string _fileName;
    private Dictionary<string, int> _stringPool;
    private bool _restored;

    public int ChildCount => _currentChildCount;
    public string Name => _currentName;
    public NodeType NodeType => _currentNodeType;
    public int Offset => _currentNodeOffset;
    public Dictionary<string, int> StringPool => _stringPool;
    private GraphicsDevice _graphics;
    
    public NxFile(string path, GraphicsDevice device)
    {
        _reader = new NxReader(File.ReadAllBytes(path));
        _graphics = device;
        _stringPool = new();
        _fileName = Path.GetFileNameWithoutExtension(path);
        _restored = true;
        ParseFile(path);
    }
    
    public NxFile this[string name]
    {
        get
        {
            if (_currentNodeOffset == 0)
                _currentNodeOffset = (int)_nodeBlockOffset;
            return ResolveFromOffset(name, _currentNodeOffset);
        }
    }
    

    private void ParseFile(string path)
    {
        var magic = _reader.ReadInt();
        if (magic != 0x34474B50)
            throw new Exception("Nope");
        
        // Parse Header
        _nodeCount = _reader.ReadInt();
        _nodeBlockOffset = _reader.ReadLong();
        _stringCount = _reader.ReadInt();
        _stringBlockOffset = _reader.ReadLong();
        _bitmapCount = _reader.ReadInt();
        _bitmapBlockOffset = _bitmapCount > 0 ? _reader.ReadLong() : 0;
        _audioCount = _reader.ReadInt();
        _audioBlockOffset = _audioCount > 0 ? _reader.ReadLong() : 0;

        GenerateStringPool(_fileName == "Map");
    }

    #region StringPool

    private void GenerateStringPool(bool isMap)
    {
        if (isMap)
        { // fucking maps wanting to be special...
            fixed (byte* data = _reader.Memory.Span)
            {
                var nodeOffset = _nodeBlockOffset;
                var nameOffset = _reader.ReadInt((int)nodeOffset);
                var firstChildId = _reader.ReadInt((int)nodeOffset + 4);
                var childCount = _reader.ReadShort((int)nodeOffset + 8);

                for (int i = firstChildId; i < firstChildId + childCount; i++)
                {
                    var childOffset = (int)_nodeBlockOffset + 20 * i;
                    var childNameOffset = _reader.ReadInt(childOffset);
                    var nodeChildId = _reader.ReadInt(childOffset + 4);
                    var nodeChildCount = _reader.ReadShort(childOffset + 8);
                    var stringOffset = Unsafe.Read<long>(data + _stringBlockOffset + (sizeof(long) * childNameOffset));
                    var childName = GetNodeName((int)stringOffset);

                    switch (childName)
                    {
                        case "Map":
                            for (var j = nodeChildId; j < nodeChildId + nodeChildCount; j++)
                            {
                                var dirOffset = (int)_nodeBlockOffset + 20 * j;
                                var dirChildId = _reader.ReadInt(dirOffset + 4);
                                var dirChildCount = _reader.ReadShort(dirOffset + 8);

                                // Eventually I wanna get rid of this shit but fuck it works so leave it
                                for (var k = dirChildId; k < dirChildId + dirChildCount; k++)
                                {
                                    var mapOffset = (int)_nodeBlockOffset + 20 * k;
                                    var mapNameOffset = _reader.ReadInt(mapOffset);
                                    var mapStringOffset = Unsafe.Read<long>(data + _stringBlockOffset + (sizeof(long) * mapNameOffset));
                                    var mapName = GetNodeName((int)mapStringOffset);
                                    _stringPool.Add($"{_fileName}/{mapName}", mapOffset);
                                }
                                
                            }
                            break;
                        case "Tile":
                            for (var j = nodeChildId; j < nodeChildId + nodeChildCount; j++)
                            {
                                var offset = (int)_nodeBlockOffset + 20 * j;
                                var mapNameOffset = _reader.ReadInt(offset);
                                var mapNameStringOffset =
                                    Unsafe.Read<long>(data + _stringBlockOffset + (sizeof(long) * mapNameOffset));
                                var mapName = GetNodeName((int)mapNameStringOffset);
                                _stringPool.Add($"{_fileName}/Tile/{mapName}", offset);
                            }

                            break;
                        case "Obj":
                            for (var j = nodeChildId; j < nodeChildId + nodeChildCount; j++)
                            {
                                var offset = (int)_nodeBlockOffset + 20 * j;
                                var mapNameOffset = _reader.ReadInt(offset);
                                var mapNameStringOffset =
                                    Unsafe.Read<long>(data + _stringBlockOffset + (sizeof(long) * mapNameOffset));
                                var mapName = GetNodeName((int)mapNameStringOffset);
                                    _stringPool.Add($"{_fileName}/Obj/{mapName}", offset);
                            }
                            break;
                        case "WorldMap":
                            for (var j = nodeChildId; j < nodeChildId + nodeChildCount; j++)
                            {
                                var offset = (int)_nodeBlockOffset + 20 * j;
                                var mapNameOffset = _reader.ReadInt(offset);
                                var mapNameStringOffset =
                                    Unsafe.Read<long>(data + _stringBlockOffset + (sizeof(long) * mapNameOffset));
                                var mapName = GetNodeName((int)mapNameStringOffset);

                                    _stringPool.Add($"{_fileName}/{mapName}", offset);
                            }

                            break;
                        case "MapHelper.img":
                            _stringPool.Add($"{_fileName}/{childName}", childOffset);
                            break;
                        case "Effect.img":
                            _stringPool.Add($"{_fileName}/{childName}", childOffset);
                            break;
                        case "Physics.img":
                            _stringPool.Add($"{_fileName}/{childName}", childOffset);
                            break;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < _nodeCount; i++)
            {
                fixed (byte* data = _reader.Memory.Span)
                {
                    var nodeOffset = _nodeBlockOffset + 20 * i;
                    var nameOffset = _reader.ReadInt((int)nodeOffset);
                    var rootStringOffset = Unsafe.Read<long>(data + _stringBlockOffset + (sizeof(long) * nameOffset));
                    var name = GetNodeName((int)rootStringOffset);

                    if (name.Contains(".img"))
                    {
                        _stringPool.Add($"{_fileName}/{name}", (int)nodeOffset);
                    }
                }
            }
        }
    }
    
    #endregion

    public NxFile GetImage(string path)
    {
        if (!_restored)
            throw new Exception("You must call Restore() when you're done obtaining values from the NX File.");
        if (_stringPool.TryGetValue(path, out var offset))
        {
            // we don't care about the rest because it's almost always going to be an image.
            _currentNodeOffset = offset;
            _currentName = path;
        }

        if (_currentNodeOffset <= 0)
            throw new Exception("Path Requested is not within the string pool");
        _restored = false;
        
        return this;
    }

    public NxFile GetDirectory(string path, bool restore = true)
    {
        if (restore)
            _directoryOffset = 0;
        
        fixed (byte* data = _reader.Memory.Span)
        {
            var offset = _directoryOffset != 0 ? _directoryOffset : _currentNodeOffset;
            var firstChildId = _reader.ReadInt((int)offset + 4);
            var childCount = _reader.ReadShort((int)offset + 8);

            for (var i = firstChildId; i < firstChildId + childCount; i++)
            {
                var nodeOffset = (int)_nodeBlockOffset + 20 * i;
                var nameOffset = _reader.ReadInt(nodeOffset);
                //var nodeChildId = _reader.ReadInt(nodeOffset + 4);
                //var nodeChildCount = _reader.ReadShort(nodeOffset + 8);
                var nodeType = _reader.ReadShort(nodeOffset + 10);
                var stringOffset = Unsafe.Read<long>(data + _stringBlockOffset + (sizeof(long) * nameOffset));
                var name = GetNodeName((int)stringOffset);
                
                if (path != name) continue;
                _directoryOffset = nodeOffset;
                _currentNodeType = (NodeType)nodeType;
                break;
            }
        }

        return this;
    }

    /*public Variant GetProperty(string propertyName)
    {
        fixed (byte* data = _reader.Memory.Span)
        {
            var offset = _directoryOffset != 0 ? _directoryOffset : _currentNodeOffset;
            var firstChildId = _reader.ReadInt((int)offset + 4);
            var childCount = _reader.ReadShort((int)offset + 8);

            for (var i = firstChildId; i < firstChildId + childCount; i++)
            {
                var nodeOffset = (int)_nodeBlockOffset + 20 * i;
                var nameOffset = _reader.ReadInt(nodeOffset);
                //var nodeChildId = _reader.ReadInt(nodeOffset + 4);
                //var nodeChildCount = _reader.ReadShort(nodeOffset + 8);
                var nodeType = _reader.ReadShort(nodeOffset + 10);
                var stringOffset = Unsafe.Read<long>(data + _stringBlockOffset + (sizeof(long) * nameOffset));
                var name = GetNodeName((int)stringOffset);
                
                if (propertyName != name) continue;
                
                switch ((NodeType)nodeType)
                {
                    case NodeType.Audio:
                        throw new NotImplementedException();
                    case NodeType.Bitmap:
                        return new Variant(VariantType.Texture, GetTexture(nodeOffset, _graphics));
                    case NodeType.Double:
                        return new Variant(VariantType.Double, GetDouble(nodeOffset));
                    case NodeType.Int64:
                        return new Variant(VariantType.Double, GetInt(nodeOffset));
                    case NodeType.String:
                        return new Variant(VariantType.Double, GetString(nodeOffset));
                    case NodeType.Vector:
                        return new Variant(VariantType.Double, GetVector(nodeOffset));
                    case NodeType.NoData:
                        return new Variant(VariantType.Unknown, null);
                }
            }
        }

        return new Variant(VariantType.Unknown, null);
    }*/
    
    public NxFile ResolvePath(string path)
    {
        if (_stringPool.TryGetValue(path, out var offset))
        {
            _currentNodeOffset = offset;
        }
        else
        {
            _currentNodeOffset = (int)_nodeBlockOffset;
        }
        
        fixed (byte* data = _reader.Memory.Span)
        {
            var splitPath = path.Split("/");
            var rootOffset = _currentNodeOffset;
            var rootNameOffset = _reader.ReadInt((int)rootOffset);
            var rootFirstChild = _reader.ReadInt((int)rootOffset + 4);
            var rootChildCount = _reader.ReadShort((int)rootOffset + 8);
            var rootNodeType = _reader.ReadShort((int)rootOffset + 10);
            var rootStringOffset = Unsafe.Read<long>(data + _stringBlockOffset + (sizeof(long) * rootNameOffset));
            _currentNodeOffset = (int)rootOffset;
            _currentName = GetNodeName((int)rootStringOffset);
            _currentNodeType = (NodeType)rootNodeType;
            CheckChildren(rootFirstChild, rootChildCount, splitPath);
        }

        return this;
    }

    private NxFile ResolveFromOffset(string path, int offset)
    {
        fixed (byte* data = _reader.Memory.Span)
        {
            var splitPath = path.Split("/");
            var rootOffset = offset;
            var rootNameOffset = _reader.ReadInt(rootOffset);
            var rootFirstChild = _reader.ReadInt(rootOffset + 4);
            var rootChildCount = _reader.ReadShort(rootOffset + 8);
            var rootNodeType = _reader.ReadShort(rootOffset + 10);
            var rootStringOffset = Unsafe.Read<long>(data + _stringBlockOffset + (sizeof(long) * rootNameOffset));
            _currentNodeOffset = rootOffset;
            _currentName = GetNodeName((int)rootStringOffset);
            _currentNodeType = (NodeType)rootNodeType;
            _currentChildCount = rootChildCount;
            CheckChildren(rootFirstChild, rootChildCount, splitPath);
        }

        _pathCheck = 0;
        
        return this;
    }
    
    private void CheckChildren(int firstChildId, int childCount, string[] path)
    {
        var maxCheck = path.Length;
        var nodeOffset = 0;
        var nameOffset = 0;
        var nodeChildId = 0;
        var nodeChildCount = 0;
        var nodeType = 0;
        long stringOffset = 0;
        
        fixed (byte* data = _reader.Memory.Span)
        {
            for (int i = firstChildId; i < firstChildId + childCount; i++)
            {
                if (_pathCheck > maxCheck)
                    break;
                
                nodeOffset = (int)_nodeBlockOffset + 20 * i;
                nameOffset = _reader.ReadInt(nodeOffset);
                nodeChildId = _reader.ReadInt(nodeOffset + 4);
                nodeChildCount = _reader.ReadShort(nodeOffset + 8);
                nodeType = _reader.ReadShort(nodeOffset + 10);
                stringOffset = Unsafe.Read<long>(data + _stringBlockOffset + (sizeof(long) * nameOffset));
                var name = GetNodeName((int)stringOffset);

                if (name == path[_pathCheck])
                {
                    _currentNodeOffset = nodeOffset;
                    _currentName = name;
                    _currentChildCount = nodeChildCount;
                    _currentNodeType = (NodeType)nodeType;
                    _pathCheck++;
                    
                    if (nodeChildCount > 0 && _pathCheck != maxCheck)
                        CheckChildren(nodeChildId, nodeChildCount, path);
                    break;
                }
            }
        }

        _pathCheck = 0;
    }

    public Dictionary<string, int> GetChildrenNames()
    {
        var list = new Dictionary<string, int>();
        var offset = _directoryOffset != 0 ? _directoryOffset : _currentNodeOffset;
        
        fixed (byte* data = _reader.Memory.Span)
        {
            var firstChildId = _reader.ReadInt(offset + 4);
            var childCount = _reader.ReadShort(offset + 8);

            for (int i = firstChildId; i < firstChildId + childCount; i++)
            {
                var nodeOffset = (int)_nodeBlockOffset + 20 * i;
                var nameOffset = _reader.ReadInt(nodeOffset);
                var stringOffset = Unsafe.Read<long>(data + _stringBlockOffset + (sizeof(long) * nameOffset));
                
                list.Add(GetNodeName((int)stringOffset), nodeOffset);
            }
        }
        //Restore();

        return list;
    }

    public void Restore()
    {
        _pathCheck = 0;
        _currentChildCount = 0;
        _currentName = "";
        _currentNodeOffset = (int)_nodeBlockOffset;
        _directoryOffset = 0;
        _currentNodeType = NodeType.NoData;
        _restored = true;
    }
    
    private string GetNodeName(int offset)
    {
        return _reader.ReadString(offset);
    }

    public int GetInt()
    {
        if (_currentNodeType != NodeType.Int64)
            return -1;
        fixed (byte* data = _reader.Memory.Span)
        {
            var val = Unsafe.Read<int>(data + _currentNodeOffset + 12);
            Restore();
            return val;
        }
    }
    
    public int GetInt(int offset)
    {
        fixed (byte* data = _reader.Memory.Span)
        {
            var val = Unsafe.Read<int>(data + offset + 12);
            return val;
        }
    }

    public Vector2 GetVector()
    {
        if (_currentNodeType != NodeType.Vector)
            return Vector2.Zero;
        fixed (byte* data = _reader.Memory.Span)
        {
            var x = Unsafe.Read<int>(data + _currentNodeOffset + 12);
            var y = Unsafe.Read<int>(data + _currentNodeOffset + 16);
            Restore();

            return new Vector2(x, y);
        }
    }
    
    public Vector2 GetVector(int offset)
    {
        fixed (byte* data = _reader.Memory.Span)
        {
            var x = Unsafe.Read<int>(data + offset + 12);
            var y = Unsafe.Read<int>(data + offset + 16);
            return new Vector2(x, y);
        }
    }

    public double GetDouble()
    {
        if (_currentNodeType != NodeType.Int64)
            return -1;
        fixed (byte* data = _reader.Memory.Span)
        {
            var val = Unsafe.Read<double>(data + _currentNodeOffset + 12);
            Restore();

            return val;
        }
    }
    
    public double GetDouble(int offset)
    {
        fixed (byte* data = _reader.Memory.Span)
        {
            var val = Unsafe.Read<double>(data + offset + 12);
            return val;
        }
    }

    public string GetString()
    {
        if (_currentNodeType != NodeType.String)
            return string.Empty;
        fixed (byte* data = _reader.Memory.Span)
        {
            var val = Unsafe.Read<int>(data + _currentNodeOffset + 12);
            var stringOffset = Unsafe.Read<long>(data + _stringBlockOffset + (sizeof(long) * val));
            var str = GetNodeName((int)stringOffset);
            Restore();

            return str;
        }
    }
    
    public string GetString(int offset)
    {
        fixed (byte* data = _reader.Memory.Span)
        {
            var val = Unsafe.Read<int>(data + offset + 12);
            var stringOffset = Unsafe.Read<long>(data + _stringBlockOffset + (sizeof(long) * val));
            var str = GetNodeName((int)stringOffset);
            return str;
        }
    }

    public Texture2D GetTexture(GraphicsDevice device)
    {
        if (_currentNodeType != NodeType.Bitmap)
            return null;

        var offset = _directoryOffset != 0 ? _directoryOffset : _currentNodeOffset;
        fixed (byte* data = _reader.Memory.Span)
        {
            var bitmapId = Unsafe.Read<int>(data + offset + 12);
            var width = Unsafe.Read<short>(data + offset + 16);
            var height = Unsafe.Read<short>(data + offset + 18);
            var bitmapImgLoc = Unsafe.Read<long>(data + _bitmapBlockOffset + sizeof(long) * bitmapId);

            if (width == 0)
                width = 800;
            if (height == 0)
                height = 600;
            
            int length = _reader.ReadInt((int)bitmapImgLoc);
            byte[] compressedBitmap = _reader.ReadBytes((int)bitmapImgLoc + 4, length).ToArray();
            byte[] uncompressedBitmap = new byte[width * height * 4];
            LZ4Codec.Decode(compressedBitmap, 0, compressedBitmap.Length, 
                uncompressedBitmap, 0, uncompressedBitmap.Length);
            var image = Image.LoadPixelData<Bgra32>(uncompressedBitmap, width, height);
            
            using var stream = new MemoryStream();
            image.Save(stream, new PngEncoder());
            
            var tex = Texture2D.FromStream(device, stream);

            return tex;
        }
    }
    
    public Texture2D GetTexture(int offset, GraphicsDevice device)
    {
        fixed (byte* data = _reader.Memory.Span)
        {
            var bitmapId = Unsafe.Read<int>(data + offset + 12);
            var width = Unsafe.Read<short>(data + offset + 16);
            var height = Unsafe.Read<short>(data + offset + 18);
            var bitmapImgLoc = Unsafe.Read<long>(data + _bitmapBlockOffset + sizeof(long) * bitmapId);

            // uhhh..I guess? idc honestly
            if (width == 0)
                width = 800;
            if (height == 0)
                height = 600;
            
            int length = _reader.ReadInt((int)bitmapImgLoc);
            byte[] compressedBitmap = _reader.ReadBytes((int)bitmapImgLoc + 4, length).ToArray();
            byte[] uncompressedBitmap = new byte[width * height * 4];
            LZ4Codec.Decode(compressedBitmap, 0, compressedBitmap.Length, 
                uncompressedBitmap, 0, uncompressedBitmap.Length);
            var image = Image.LoadPixelData<Bgra32>(uncompressedBitmap, width, height);
            
            using var stream = new MemoryStream();
            image.Save(stream, new PngEncoder());
            
            var tex = Texture2D.FromStream(device, stream);
            return tex;
        }
    }

    public void Dispose()
    {
        _reader.Dispose();
        _stringPool.Clear();
    }
}