using System.Text;
using K4os.Compression.LZ4;
using MapleSyrup.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace MapleSyrup.Nx;

public class NxNode(ref NxReader reader, NodeType type, string nodeName, long offset)
    : IDisposable
{
    private List<NxNode> _children = new();
    private readonly NxReader _reader = reader;
    private RefCounted<Texture2D> _texture;

    public NodeType Type
    {
        get => type;
        set => type = value;
    }

    public string Name
    {
        get => nodeName;
        set => nodeName = value;
    }

    public List<NxNode> Children
    {
        get => _children;
        set => _children = value;
    }

    public NxNode this[string node]
    {
        get { return _children.FirstOrDefault(c => c.Name == node) ?? throw new NullReferenceException(); }
    }

    public bool Has(string node, out NxNode? child)
    {
        foreach (var t in _children.Where(t => t.Name == node))
        {
            child = t;
            return true;
        }

        child = null;
        return false;
    }

    public int GetInt()
    {
        if (type != NodeType.Int64)
            return -9999;
        var value = _reader.ReadLong(offset + 12);
        return (int)value;
    }

    public double GetDouble()
    {
        if (type != NodeType.Double)
            return -9999.999;
        var value = _reader.ReadLong(offset + 12);
        return value;
    }

    public Vector2 GetVector()
    {
        if (type != NodeType.Vector)
            return Vector2.Zero;
        var x = _reader.ReadInt(offset + 12);
        var y = _reader.ReadInt(offset + 16);
        return new Vector2(x, y);
    }

    public string GetString()
    {
        if (type != NodeType.String)
            return string.Empty;
        var nameOffset = _reader.ReadInt(offset + 12);
        var stringOffset = _reader.ReadLong(_reader.StringBlockOffset + (sizeof(long) * nameOffset));
        var readString = _reader.ReadString((int)stringOffset);

        return readString;
    }

    public RefCounted<Texture2D> GetTexture(GraphicsDevice device)
    {
        if (type != NodeType.Bitmap)
            return null;

        var bitmapId = _reader.ReadInt(offset + 12);
        var width = _reader.ReadShort(offset + 16);
        var height = _reader.ReadShort(offset + 18);
        var bitmapImageLocation = _reader.ReadLong(_reader.BitmapBlockOffset + sizeof(long) * bitmapId);

        int length = _reader.ReadInt((int)bitmapImageLocation);
        byte[] compressedBitmap = _reader.ReadBytes((int)bitmapImageLocation + 4, length).ToArray();
        byte[] uncompressedBitmap = new byte[width * height * 4];
        LZ4Codec.Decode(compressedBitmap, 0, compressedBitmap.Length,
            uncompressedBitmap, 0, uncompressedBitmap.Length);
        using var image = Image.LoadPixelData<Bgra32>(uncompressedBitmap, width, height);
        using var stream = new MemoryStream();
        image.Save(stream, new PngEncoder());
        var tex = Texture2D.FromStream(device, stream);
        _texture = new RefCounted<Texture2D>(tex);

        return _texture;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        foreach (var child in _children)
        {
            sb.Append($"Child: {child.Name}\n");
        }

        return sb.ToString();
    }

    public void Dispose()
    {
        _texture?.Release(true);
        _children.ForEach(node => node.Dispose());
        _children.Clear();
    }
}