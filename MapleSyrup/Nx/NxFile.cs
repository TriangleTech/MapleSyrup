using System.Runtime.CompilerServices;
using K4os.Compression.LZ4;
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
    private string _currentName;
    private NodeType _currentNodeType;
    private bool childFound = false;

    public int ChildCount => _currentChildCount;
    public string Name => _currentName;
    public NodeType NodeType => _currentNodeType;
    public int Offset => _currentNodeOffset;
    
    public NxFile(string path)
    {
        _reader = new NxReader(File.ReadAllBytes(path));
        ParseFile(path);
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
    }
    
    public NxFile ResolvePath(string path)
    {
        fixed (byte* data = _reader.Memory.Span)
        {
            var splitPath = path.Split("/");
            var rootOffset = _nodeBlockOffset;
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

    public NxFile this[string name]
    {
        get
        {
            if (_currentNodeOffset == 0)
                _currentNodeOffset = (int)_nodeBlockOffset;
            return ResolveFromOffset(name, _currentNodeOffset);
        }
    }

    public List<string> GetChildrenNames()
    {
        var list = new List<string>();
        fixed (byte* data = _reader.Memory.Span)
        {
            var firstChildId = _reader.ReadInt(_currentNodeOffset + 4);
            var childCount = _reader.ReadShort(_currentNodeOffset + 8);

            for (int i = firstChildId; i < firstChildId + childCount; i++)
            {
                var nodeOffset = (int)_nodeBlockOffset + 20 * i;
                var nameOffset = _reader.ReadInt(nodeOffset);
                var stringOffset = Unsafe.Read<long>(data + _stringBlockOffset + (sizeof(long) * nameOffset));
                
                list.Add(GetNodeName((int)stringOffset));
            }
        }
        Restore();

        return list;
    }

    public void Restore()
    {
        _pathCheck = 0;
        _currentChildCount = 0;
        _currentName = "";
        _currentNodeOffset = (int)_nodeBlockOffset;
        _currentNodeType = NodeType.NoData;
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

    public Texture2D GetTexture(GraphicsDevice device)
    {
        if (_currentNodeType != NodeType.Bitmap)
            return null;
        fixed (byte* data = _reader.Memory.Span)
        {
            var bitmapId = Unsafe.Read<int>(data + _currentNodeOffset + 12);
            var width = Unsafe.Read<short>(data + _currentNodeOffset + 16);
            var height = Unsafe.Read<short>(data + _currentNodeOffset + 18);
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
            Restore();

            return tex;
        }
    }

    public void Dispose()
    {
        _reader.Dispose();
    }
}