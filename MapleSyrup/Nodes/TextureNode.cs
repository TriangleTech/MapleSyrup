using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace MapleSyrup.Nodes;

public class TextureNode : Node
{
    public enum FlipMode
    {
        None,
        Horizontal,
        Vertical
    }
    
    private int textureId = -1;
    private Image textureImage;
    private FlipMode flipMode = FlipMode.None;
    private TextureNode texture;
    private Vector2 textureSize;
    
    public TextureNode Texture
    {
        get => texture;
        set => texture = value;
    }
    
    public int TextureId
    {
        get => textureId;
    }
    
    public FlipMode Flip
    {
        get => flipMode;
        set => flipMode = value;
    }
    
    public Vector2 Size
    {
        get => textureSize;
        set => textureSize = value;
    }
    
    public TextureNode(Image image) 
    {
        textureImage = image;
        Size = new Vector2(image.Width, image.Height);
        Create();
        texture = this;
    }

    private void Create()
    {
        byte[] data = new byte[(int)Size.X * (int)Size.Y * 4];
        var image = textureImage.CloneAs<Rgba32>();
        //image.Mutate(x => x.Flip(SixLabors.ImageSharp.Processing.FlipMode.Vertical)); // this is a hack to fix the image being flipped vertically
        
        if (flipMode == FlipMode.Horizontal)
            image.Mutate(x => x.Flip(SixLabors.ImageSharp.Processing.FlipMode.Horizontal));
        else if (flipMode == FlipMode.Vertical)
            image.Mutate(x => x.Flip(SixLabors.ImageSharp.Processing.FlipMode.Vertical));
        image.CopyPixelDataTo(data);
        
        GL.GenTextures(1, out textureId);
        GL.BindTexture(TextureTarget.Texture2D, textureId);
        
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, (int)Size.X, (int)Size.Y, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
    }

    public void Use(TextureUnit textureUnit = TextureUnit.Texture0)
    {
        GL.ActiveTexture(textureUnit);
        GL.BindTexture(TextureTarget.Texture2D, textureId);
    }
    
    public override void Dispose()
    {
        GL.DeleteTextures(1, ref textureId);
        textureImage.Dispose();
        GC.SuppressFinalize(this);
        base.Dispose();
    }
}