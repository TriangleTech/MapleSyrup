using System.Runtime.CompilerServices;
using K4os.Compression.LZ4;
using K4os.Compression.LZ4.Encoders;
using Microsoft.Win32.SafeHandles;
using Microsoft.Xna.Framework.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace MapleSyrup.Resources.Nx;

/// <summary>
/// Container for Bitmap Node Type
/// </summary>
public class NxBitmapNode : NxNode
{
    private int width, height;
    private long bitmapOffset;
    private NxBuffer? reader;
    
    /// <summary>
    /// Initializes the Bitmap Node
    /// </summary>
    /// <param name="name">Name of the Node</param>
    /// <param name="childId">ID of the first child node</param>
    /// <param name="count">Number of child nodes</param>
    /// <param name="nType">Type of Node</param>
    /// <param name="offset"></param>
    /// <param name="w">Width of the Bitmap</param>
    /// <param name="h">Height of the Bitmap</param>
    /// <param name="buffer">The buffer that contains the data of the file</param>
    public NxBitmapNode(string name, int childId, int count, NodeType nType, long offset, int w, int h, NxBuffer buffer)
        : base(name, childId, count, nType)
    {
        width = w;
        height = h;
        bitmapOffset = offset;
        reader = buffer;
    }

    /// <summary>
    /// Gets the Bitmap using the Offset from the table and decompresses it to produce a Bgra32 Image.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Image<Bgra32> GetBitmap()
    {
        if (reader == null || bitmapOffset == 0)
            throw new Exception("[NxBitmapNode] Attempted to retrieve a bitmap with no data");
        
        reader.SetIndex((int)bitmapOffset);
        int length = reader.ReadInt();
        byte[] compressedBitmap = reader.ReadBytes(length, (int)bitmapOffset + 4).ToArray();
        byte[] uncompressedBitmap = new byte[width * height * 4];
        LZ4Codec.Decode(compressedBitmap, 0, compressedBitmap.Length, 
            uncompressedBitmap, 0, uncompressedBitmap.Length);
        var image = Image.LoadPixelData<Bgra32>(uncompressedBitmap, width, height);
        
        return image;
    }

    public Texture2D GetTexture(GraphicsDevice device)
    {
        using var img = GetBitmap();
        using var stream = new MemoryStream();
        img.Save(stream, new PngEncoder());

        var tex = Texture2D.FromStream(device, stream);
        return tex;
    }

    public void Save()
    {
        var img = GetBitmap();
        img.SaveAsPng(new FileStream("/home/beray/mapledev/banana.png", FileMode.CreateNew));
    }
}