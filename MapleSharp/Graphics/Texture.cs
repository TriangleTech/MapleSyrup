using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace MapleSharp.Graphics;

public class Texture : IDisposable
{
    private int textureId = -1;
    private Vector2 textureSize;
    private Image textureImage;
    
    public int TextureId
    {
        get => textureId;
    }
    
    public Vector2 TextureSize
    {
        get => textureSize;
    }

    private void Create()
    {
        byte[] data = new byte[(int)textureSize.X * (int)textureSize.Y * 4];
        var test = textureImage.CloneAs<Rgba32>();
        test.Mutate(x => x.Flip(FlipMode.Vertical));
        test.CopyPixelDataTo(data);
        
        GL.GenTextures(1, out textureId);
        GL.BindTexture(TextureTarget.Texture2D, textureId);
        
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, (int)textureSize.X, (int)textureSize.Y, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

         // we don't need this anymore.
    }
    
    protected void UpdateTexture(Texture other)
    {
        throw new NotImplementedException();
    }
    
    public void Dispose()
    {
        GL.DeleteTextures(1, ref textureId);
        textureImage.Dispose();
        GC.SuppressFinalize(this);
    }
}