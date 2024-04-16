using System.Text;
using K4os.Compression.LZ4;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace MapleSyrup.Nx;

public class NxNode : IDisposable
{
    private NodeType _type;
    private string _name;
    private List<NxNode> _children;
    private long _offset;
    private NxReader _reader;
    
    public NodeType Type
    {
        get => _type;
        set => _type = value;
    }
    
    public string Name
    {
        get => _name;
        set => _name = value;
    }
    
    public List<NxNode> Children
    {
        get => _children;
        set => _children = value;
    }

    public NxNode()
    {
        _children = new();
    }
    
    public NxNode(ref NxReader reader, NodeType type, string name, long offset)
    {
        _reader = reader;
        _type = type;
        _name = name;
        _children = new();
        _offset = offset;
    }

    public NxNode? this[string name]
    {
        get
        {
            return _children.FirstOrDefault(c => c.Name == name) ?? null;
        }
    }

    public bool Has(string node, out NxNode child)
    {
        for (var i = 0; i < _children.Count; i++)
        {
            if (_children[i].Name != node) continue;
            
            child = _children[i];
            return true;
        }

        child = default;
        return false;
    }

    public int GetInt()
    {
        if (_type != NodeType.Int64)
            return -9999;
        var value = _reader.ReadLong(_offset + 12);
        return (int)value;
    }
    
    public double GetDouble()
    {
        if (_type != NodeType.Double)
            return -9999.999;
        var value = _reader.ReadLong(_offset + 12);
        return value;
    }
    
    public Vector2 GetVector()
    {
        if (_type != NodeType.Vector)
            return Vector2.Zero;
        var x = _reader.ReadInt(_offset + 12);
        var y = _reader.ReadInt(_offset + 16);
        return new Vector2(x, y);
    }

    public string GetString()
    {
        if (_type != NodeType.String)
            return string.Empty;
        var nameOffset = _reader.ReadInt(_offset + 12);
        var stringOffset = _reader.ReadLong(_reader.StringBlockOffset + (sizeof(long) * nameOffset));
        var name = _reader.ReadString((int)stringOffset);

        return name;
    }

    public Texture2D GetTexture(GraphicsDevice device)
    {
        if (_type != NodeType.Bitmap)
            return default;
        
        var bitmapId = _reader.ReadInt(_offset + 12);
        var width = _reader.ReadShort(_offset + 16);
        var height = _reader.ReadShort(_offset + 18);
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

        return tex;
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
        _children.ForEach(node => node.Dispose());
        _children.Clear();
    }
}