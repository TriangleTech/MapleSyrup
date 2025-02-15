using System.Collections.ObjectModel;
using System.Numerics;
using CommunityToolkit.HighPerformance;
using K4os.Compression.LZ4;
using ZeroElectric.Vinculum;

namespace MapleSyrup.NX;

public record NXNode
{
    public required string NodePath { get; init; }
    public required string Name { get; init; }
    public required uint FirstChildId { get; init; }
    public required ushort ChildCount { get; init; }
    public required NodeType Type { get; init; }
    public required ulong Offset { get; init; }
    public required NXBuffer Buffer { get; init; }
    
    ~NXNode()
    {
        Buffer.Dispose();
    }

    /// <summary>
    /// Gets the children of the node
    /// </summary>
    /// <returns>A dictionary of <see cref="NXNode"/></returns>
    public ReadOnlyDictionary<string, NXNode> GetChildren()
    {
        var nodes = new Dictionary<string, NXNode>(ChildCount);
        if (ChildCount == 0) return nodes.AsReadOnly();

        for (var i = FirstChildId; i < FirstChildId + ChildCount; i++)
        {
            var offset = Buffer.NodeBlock + 20 * i;
            Buffer.Seek((long)offset);
            var nameOffset = Buffer.ReadUInt32();
            var firstChildId = Buffer.ReadUInt32();
            var childCount = Buffer.ReadUInt16();
            var nodeType = (NodeType)Buffer.ReadUInt16();

            Buffer.Seek((long)(Buffer.StringBlock + 8 * nameOffset));
            var stringOffset = Buffer.ReadUInt64();

            Buffer.Seek((long)(stringOffset));
            var nodeName = Buffer.ReadString();
            
            nodes.Add(nodeName, new NXNode
            {
                NodePath = string.Concat(NodePath, $"/{nodeName}"),
                Name = nodeName,
                FirstChildId = firstChildId,
                ChildCount = childCount,
                Type = nodeType,
                Offset = offset,
                Buffer = new NXBuffer(Buffer.MemoryMappedFile)
                {
                    NodeBlock = Buffer.NodeBlock,
                    StringBlock = Buffer.StringBlock,
                    BitmapBlock = Buffer.BitmapBlock,
                    AudioBlock = Buffer.AudioBlock,
                },
            });
        }

        return nodes.AsReadOnly();
    }
    
    /// <summary>
    /// Gets the names of the children contained in the <see cref="NXNode"/>
    /// </summary>
    /// <returns>A span of strings containing the names.</returns>
    public Span<string> GetChildrenNames()
    {
        if (ChildCount == 0) return Array.Empty<string>();
        var nodes = new List<string>();
        
        for (var i = FirstChildId; i < FirstChildId + ChildCount; i++)
        {
            var offset = Buffer.NodeBlock + 20 * i;
            Buffer.Seek((long)offset);
            var nameOffset = Buffer.ReadUInt32();

            Buffer.Seek((long)(Buffer.StringBlock + 8 * nameOffset));
            var stringOffset = Buffer.ReadUInt64();

            Buffer.Seek((long)(stringOffset));
            var nodeName = Buffer.ReadString();
            nodes.Add(nodeName);
        }
        
        return nodes.AsSpan();
    }
    
    public bool HasNode(string name)
    {
        if (ChildCount == 0) return false;
        
        for (var i = FirstChildId; i < FirstChildId + ChildCount; i++)
        {
            var offset = Buffer.NodeBlock + 20 * i;
            Buffer.Seek((long)offset);
            var nameOffset = Buffer.ReadUInt32();

            Buffer.Seek((long)(Buffer.StringBlock + 8 * nameOffset));
            var stringOffset = Buffer.ReadUInt64();

            Buffer.Seek((long)(stringOffset));
            var nodeName = Buffer.ReadString();
            
            if (nodeName == name) return true;
        }

        return false;
    }

    public int GetInt()
    {
        lock (Buffer)
        {
            Buffer.Seek((long)Offset + 12);
            var data = Buffer.ReadUInt64();

            return (int)data;
        }
    }

    public double GetDouble()
    {
        lock (Buffer)
        {
            Buffer.Seek((long)Offset + 12);
            var data = Buffer.ReadUInt64();

            return data;
        }
    }

    public string GetString()
    {
        if (Type != NodeType.String) throw new Exception("Not a string node");
        lock (Buffer)
        {
            Buffer.Seek((long)Offset + 12);
            var stringId = Buffer.ReadUInt32();

            Buffer.Seek((long)Buffer.StringBlock + 8 * stringId);
            var stringOffset = Buffer.ReadUInt64();

            Buffer.Seek((long)(stringOffset)); // taking a guess here. Worst case increase to 1024
            var nodeName = Buffer.ReadString();

            return nodeName;
        }
    }

    public Vector2 GetVector()
    {
        if (Type != NodeType.Vector) throw new Exception("Not a vector node");
        Buffer.Seek((long)Offset + 12);
        var vector = new Vector2(Buffer.ReadInt32(), Buffer.ReadInt32()); // if you read these with uint it will give you an 4.8e^23 number.
        
        if (vector.X > ushort.MaxValue || vector.Y > ushort.MaxValue) 
            throw new Exception("Vector is too big");

        return vector;
    }

    public unsafe Texture GetTexture()
    {
        if (Type != NodeType.Bitmap) throw new Exception("Not a bitmap node");
        Buffer.Seek((long)Offset + 12);
        var bitmapId = Buffer.ReadUInt32();
        var width = Buffer.ReadUInt16();
        var height = Buffer.ReadUInt16();

        Buffer.Seek((long)(Buffer.BitmapBlock + 8 * bitmapId));
        var bitmapOffset = Buffer.ReadUInt64();

        Buffer.Seek((long)(bitmapOffset));
        var dataLength = Buffer.ReadUInt32();
        var compressedData = Buffer.ReadBytes((int)dataLength).ToArray();
        var decompressedData = new byte[width * height * 4];
        var decompressedSize = LZ4Codec.Decode(compressedData, 0, compressedData.Length,
            decompressedData, 0, decompressedData.Length);
        if (decompressedSize <= 0) throw new Exception("Failed to decompress texture data");

        var rayImage = new Image();
        var count = 0; // just in queso
        fixed (byte* data = decompressedData)
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
            lock (decompressedData)
            {
                var tex = Raylib.LoadTextureFromImage(rayImage);
                return tex;
            }
        }
    }
}