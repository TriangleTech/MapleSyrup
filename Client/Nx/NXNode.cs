using System.Numerics;
using K4os.Compression.LZ4;
using ZeroElectric.Vinculum;

namespace Client.Nx;

public readonly struct NXNode
{
    public string NodePath {get; init;}
    public string Name { get; init; }
    public uint FirstChildId { get; init; }
    public ushort ChildCount { get; init; }
    public NodeType Type { get; init; }
    public ulong Offset { get; init; }
    public Memory<byte> Data { get; init; }

    public int GetInt(NXFile file)
    {
        file.Buffer.Seek((long)Offset + 12);
        var data = file.Buffer.ReadUInt64();

        return (int)data;
    }
    
    public double GetDouble(NXFile file)
    {
        file.Buffer.Seek((long)Offset + 12);
        var data = file.Buffer.ReadUInt64();

        return data;
    }
    
    public string GetString(NXFile file)
    {
        if (Type != NodeType.String) throw new Exception("Not a string node");
        file.Buffer.Seek((long)Offset + 12);
        var stringId = file.Buffer.ReadUInt32();
        
        file.Buffer.Seek((long)file.StringBlock + 8 * stringId);
        var stringOffset = file.Buffer.ReadUInt64();

        file.Buffer.Seek((long)(stringOffset)); // taking a guess here. Worst case increase to 1024
        var nodeName = file.Buffer.ReadString();
        
        return nodeName;
    }

    public Vector2 GetVector(NXFile file)
    {
        if (Type != NodeType.Vector) throw new Exception("Not a vector node");
        file.Buffer.Seek((long)Offset + 12);
        var vector = new Vector2(file.Buffer.ReadUInt32(), file.Buffer.ReadUInt32());
        
        return vector;
    }

    public unsafe Texture GetTexture(NXFile file)
    {
        if (Type != NodeType.Bitmap) throw new Exception("Not a bitmap node");
        file.Buffer.Seek((long)Offset + 12);
        var bitmapId = file.Buffer.ReadUInt32();
        var width = file.Buffer.ReadUInt16();
        var height = file.Buffer.ReadUInt16();
        
        file.Buffer.Seek((long)(file.BitmapBlock + 8 * bitmapId));
        var bitmapOffset = file.Buffer.ReadUInt64();
        
        file.Buffer.Seek((long)(bitmapOffset));
        var dataLength = file.Buffer.ReadUInt32();
        var compressedData = file.Buffer.ReadBytes((int)dataLength).ToArray();
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
        }

        var tex = Raylib.LoadTextureFromImage(rayImage);
        return tex;
    }
}