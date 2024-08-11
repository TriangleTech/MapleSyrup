using System.Numerics;
using K4os.Compression.LZ4;
using Raylib_CsLo;

namespace Client.NX;

public class NxNode : IDisposable
{
    private NxReader _reader;
    private Dictionary<string, long> _children;

    public string Name { get; }
    public long Offset { get; }
    public int FirstChildId { get; }
    public short ChildCount { get; }
    public Dictionary<string, long> Children => _children;
    public NodeType NodeType { get; }
    public NxSaveMode SaveMode { get; }
    public string FullPath { get; init; }

    public NxNode(NxReader reader, long offset, string name, NxSaveMode mode = NxSaveMode.None)
    {
        _reader = reader;
        Offset = offset;
        Name = name;
        SaveMode = mode;
        _reader.Seek(offset);
        _reader.Skip(4); // we don't need the name;
        FirstChildId = _reader.ReadInt();
        ChildCount = _reader.ReadShort();
        NodeType = (NodeType)_reader.ReadShort();
        _children = mode == NxSaveMode.Save ? new(ChildCount) : new(1);
        ParseChildren();
    }

    ~NxNode()
    {
        _children.Clear();
    }

    public NxNode? this[string name] => GetChildNode(name);

    private void ParseChildren()
    {
        if (SaveMode != NxSaveMode.Save) return;
        for (var i = FirstChildId; i < FirstChildId + ChildCount; i++)
        {
            var childOffset = _reader.NodeBlockOffset + 20 * i;
            _reader.Seek(childOffset);
            var nameOffset = _reader.ReadInt();

            _reader.Seek(_reader.StringBlockOffset + 8 * nameOffset);
            var stringOffset = _reader.ReadLong();
            var childName = _reader.ReadString(stringOffset);
            _children?.Add(childName, childOffset);
        }
    }

    public NxNode? GetChildNode(string name)
    {
        if (SaveMode == NxSaveMode.Save)
        {
            _ = _children.TryGetValue(name, out var offset);
            return offset > 0
                ? new NxNode(_reader, offset, name, SaveMode){ FullPath = string.Concat(FullPath, $"/{name}") }
                : throw new Exception($"[NX] Parent: {Name} does not contain child {name}");
        }

        for (var i = FirstChildId; i < FirstChildId + ChildCount; i++)
        {
            var childOffset = _reader.NodeBlockOffset + 20 * i;
            _reader.Seek(childOffset);
            var nameOffset = _reader.ReadInt();

            _reader.Seek(_reader.StringBlockOffset + 8 * nameOffset);
            var stringOffset = _reader.ReadLong();
            var childName = _reader.ReadString(stringOffset);
            if (childName == name)
            {
                return new NxNode(_reader, childOffset, childName)
                    { FullPath = string.Concat(FullPath, $"/{childName}") };
            }
        }

        return null;
    }

    public bool Has(string node)
    {
        if (SaveMode == NxSaveMode.Save)
        {
            return _children.TryGetValue(node, out _);
        }

        for (var i = FirstChildId; i < FirstChildId + ChildCount; i++)
        {
            var childOffset = _reader.NodeBlockOffset + 20 * i;
            _reader.Seek(childOffset);
            var nameOffset = _reader.ReadInt();

            _reader.Seek(_reader.StringBlockOffset + 8 * nameOffset);
            var stringOffset = _reader.ReadLong();
            var childName = _reader.ReadString(stringOffset);

            if (childName == node)
            {
                return true;
            }
        }

        return false;
    }

    public bool Has(string node, out NxNode? child)
    {
        if (SaveMode == NxSaveMode.Save)
        {
            if (!_children.TryGetValue(node, out var offset))
            {
                child = null;
                return false;
            }

            child = new NxNode(_reader, offset, node, SaveMode);
            return true;
        }

        for (var i = FirstChildId; i < FirstChildId + ChildCount; i++)
        {
            var childOffset = _reader.NodeBlockOffset + 20 * i;
            _reader.Seek(childOffset);
            var nameOffset = _reader.ReadInt();

            _reader.Seek(_reader.StringBlockOffset + 8 * nameOffset);
            var stringOffset = _reader.ReadLong();
            var childName = _reader.ReadString(stringOffset);

            if (childName != node) continue;
            child = new NxNode(_reader, childOffset, childName);
            return true;
        }

        child = null;
        return false;
    }

    public int GetInt(string node)
    {
        if (SaveMode == NxSaveMode.Save)
        {
            if (!_children.TryGetValue(node, out var offset)) return 0;
            _reader.Seek(offset);
            _reader.Skip(10);
            var type = (NodeType)_reader.ReadShort();
            if (type != NodeType.Int64) throw new Exception($"[NX] Node: {node} is not type of {NodeType.Int64}");
            var val = (int)_reader.ReadLong();
            return val;
        }

        for (var i = FirstChildId; i < FirstChildId + ChildCount; i++)
        {
            var childOffset = _reader.NodeBlockOffset + 20 * i;
            _reader.Seek(childOffset);
            var nameOffset = _reader.ReadInt();
            _reader.Skip(6);
            var type = (NodeType)_reader.ReadShort();

            _reader.Seek(_reader.StringBlockOffset + 8 * nameOffset);
            var stringOffset = _reader.ReadLong();
            var childName = _reader.ReadString(stringOffset);
            if (childName != node || type != NodeType.Int64) continue;

            _reader.Seek(childOffset);
            _reader.Skip(12);
            var val = (int)_reader.ReadLong();
            return val;
        }

        throw new Exception($"[NX] Node: {node} is not type of {NodeType.Int64}");
    }

    public double GetDouble(string node)
    {
        if (SaveMode == NxSaveMode.Save)
        {
            if (!_children.TryGetValue(node, out var offset)) return 0;
            _reader.Seek(offset);
            _reader.Skip(10);
            var type = (NodeType)_reader.ReadShort();
            if (type != NodeType.Double) throw new Exception($"[NX] Node: {node} is not type of {type}");

            var val = (int)_reader.ReadLong();
            return val;
        }

        for (var i = FirstChildId; i < FirstChildId + ChildCount; i++)
        {
            var childOffset = _reader.NodeBlockOffset + 20 * i;
            _reader.Seek(childOffset);
            var nameOffset = _reader.ReadInt();
            _reader.Skip(6);
            var type = (NodeType)_reader.ReadShort();

            _reader.Seek(_reader.StringBlockOffset + 8 * nameOffset);
            var stringOffset = _reader.ReadLong();
            var childName = _reader.ReadString(stringOffset);
            if (childName != node || type != NodeType.Double) continue;
            _reader.Seek(childOffset);
            _reader.Skip(12);

            return _reader.ReadLong();
        }

        throw new Exception($"[NX] Node: {node} is not type of {NodeType.Double}");
    }

    public Vector2 GetVector(string node)
    {
        if (SaveMode == NxSaveMode.Save)
        {
            if (!_children.TryGetValue(node, out var offset)) return Vector2.Zero;
            _reader.Seek(offset);
            _reader.Skip(10);
            var type = (NodeType)_reader.ReadShort();
            if (type != NodeType.Vector) throw new Exception($"[NX] Node: {node} is not type of {type}");

            return new Vector2(_reader.ReadInt(), _reader.ReadInt());

        }

        for (var i = FirstChildId; i < FirstChildId + ChildCount; i++)
        {
            var childOffset = _reader.NodeBlockOffset + 20 * i;
            _reader.Seek(childOffset);
            var nameOffset = _reader.ReadInt();
            _reader.Skip(6);
            var type = (NodeType)_reader.ReadShort();

            _reader.Seek(_reader.StringBlockOffset + 8 * nameOffset);
            var stringOffset = _reader.ReadLong();
            var childName = _reader.ReadString(stringOffset);
            if (childName != node || type != NodeType.Vector) continue;
            _reader.Seek(childOffset);
            _reader.Skip(12);

            return new Vector2(_reader.ReadInt(), _reader.ReadInt());
        }

        throw new Exception($"[NX] Node: {node} is not type of {NodeType.Vector}");
    }

    public string GetString(string node)
    {
        if (SaveMode == NxSaveMode.Save)
        {
            if (!_children.TryGetValue(node, out var offset)) return "";
            _reader.Seek(offset);
            _reader.Skip(10);
            var type = (NodeType)_reader.ReadShort();
            if (type != NodeType.String) throw new Exception($"[NX] Node: {node} is not type of {type}");

            var nodeNameOffset = _reader.ReadInt();
            _reader.Seek(_reader.StringBlockOffset + 8 * nodeNameOffset);
            var nodeStringOffset = _reader.ReadLong();
            var readString = _reader.ReadString(nodeStringOffset);

            return readString;

        }

        for (var i = FirstChildId; i < FirstChildId + ChildCount; i++)
        {
            var childOffset = _reader.NodeBlockOffset + 20 * i;
            _reader.Seek(childOffset);
            var nameOffset = _reader.ReadInt();
            _reader.Skip(6);
            var type = (NodeType)_reader.ReadShort();

            _reader.Seek(_reader.StringBlockOffset + 8 * nameOffset);
            var stringOffset = _reader.ReadLong();
            var childName = _reader.ReadString(stringOffset);

            if (childName != node || type != NodeType.String) continue;

            _reader.Seek(childOffset);
            _reader.Skip(12);
            var nodeNameOffset = _reader.ReadInt();
            _reader.Seek(_reader.StringBlockOffset + 8 * nodeNameOffset);
            var nodeStringOffset = _reader.ReadLong();
            var readString = _reader.ReadString(nodeStringOffset);

            return readString;
        }

        throw new Exception($"[NX] Node: {node} is not type of {NodeType.String}");
    }

    public unsafe Texture GetTexture(string node)
    {
        lock (_children)
        {
            if (SaveMode == NxSaveMode.Save)
            {
                if (!_children.TryGetValue(node, out var offset)) return new Texture();
                _reader.Seek(offset);
                _reader.Skip(10);
                var type = (NodeType)_reader.ReadShort();
                if (type != NodeType.Bitmap) throw new Exception($"[NX] Node: {node} is not type of Bitmap");

                var bitmapId = _reader.ReadInt();
                var width = _reader.ReadShort();
                var height = _reader.ReadShort();
                _reader.Seek(_reader.BitmapBlockOffset + 8 * bitmapId);
                var bitmapImageLocation = _reader.ReadLong();
                _reader.Seek(bitmapImageLocation);
                var length = _reader.ReadInt();
                var compressedBitmap = _reader.ReadBytes(length).ToArray();
                var uncompressedBitmap = new byte[width * height * 4];
                var decompressedSize = LZ4Codec.Decode(compressedBitmap, 0, compressedBitmap.Length,
                    uncompressedBitmap, 0, uncompressedBitmap.Length);

                if (decompressedSize <= 0)
                    throw new Exception("Failed to decompress texture data");

                var rayImage = new Raylib_CsLo.Image();
                var count = 0; // just in queso
                fixed (byte* data = uncompressedBitmap)
                {
                    // Convert it from BGRA32 to RGBA32
                    // Raylib doesn't support anything but RGB channels.
                    for (var j = 0; j < width * height; j++)
                    {
                        var b = data[count];
                        var g = data[count + 1];
                        var r = data[count + 2];
                        var a = data[count + 3];

                        data[count] = r;
                        data[count + 1] = g;
                        data[count + 2] = b;
                        data[count + 3] = a;

                        count += 4;
                    }
                    
                    rayImage.data = data;
                    rayImage.format = (int)PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8;
                    rayImage.width = width;
                    rayImage.height = height;
                    rayImage.mipmaps = 1;
                }

                var tex = Raylib.LoadTextureFromImage(rayImage);
                return tex;
            }

            for (var i = FirstChildId; i < FirstChildId + ChildCount; i++)
            {
                var childOffset = _reader.NodeBlockOffset + 20 * i;
                _reader.Seek(childOffset);
                var nameOffset = _reader.ReadInt();
                _reader.Skip(6);
                var type = (NodeType)_reader.ReadShort();

                _reader.Seek(_reader.StringBlockOffset + 8 * nameOffset);
                var stringOffset = _reader.ReadLong();
                var childName = _reader.ReadString(stringOffset);

                if (childName != node || type != NodeType.Bitmap) continue;

                _reader.Seek(childOffset);
                _reader.Skip(12);
                var bitmapId = _reader.ReadInt();
                var width = _reader.ReadShort();
                var height = _reader.ReadShort();
                _reader.Seek(_reader.BitmapBlockOffset + 8 * bitmapId);
                var bitmapImageLocation = _reader.ReadLong();
                _reader.Seek(bitmapImageLocation);
                var length = _reader.ReadInt();
                var compressedBitmap = _reader.ReadBytes(length).ToArray();
                var uncompressedBitmap = new byte[width * height * 4];
                var decompressedSize = LZ4Codec.Decode(compressedBitmap, 0, compressedBitmap.Length,
                    uncompressedBitmap, 0, uncompressedBitmap.Length);

                if (decompressedSize <= 0)
                    throw new Exception("Failed to decompress texture data");

                var rayImage = new Raylib_CsLo.Image();
                var count = 0; // just in queso
                fixed (byte* data = uncompressedBitmap)
                {
                    // Convert it from BGRA32 to RGBA32
                    // Raylib doesn't support anything but RGB channels.
                    for (var j = 0; j < width * height; j++)
                    {
                        var b = data[count];
                        var g = data[count + 1];
                        var r = data[count + 2];
                        var a = data[count + 3];

                        data[count] = r;
                        data[count + 1] = g;
                        data[count + 2] = b;
                        data[count + 3] = a;

                        count += 4;
                    }
                    
                    rayImage.data = data;
                    rayImage.format = (int)PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8;
                    rayImage.width = width;
                    rayImage.height = height;
                    rayImage.mipmaps = 1;
                }

                var tex = Raylib.LoadTextureFromImage(rayImage);
                return tex;
            }

            throw new Exception($"[NX] Node: {node} is not type of {NodeType.Bitmap}");
        }
    }

    public void Dispose()
    {
        _reader.Dispose();
        _children.Clear();
    }
}

public enum NxSaveMode
{
    None,
    Save
}