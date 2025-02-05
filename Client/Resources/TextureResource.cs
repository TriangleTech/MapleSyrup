using System.Numerics;
using ZeroElectric.Vinculum;

namespace Client.Resources;

public class TextureResource : IResource
{
    public string Name { get; init; }
    public ResourceType ResourceType { get; init; } = ResourceType.Texture;
    
    public required Texture Texture { get; init; }
    public required Vector2 Origin { get; init; }
    public required float Delay { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }
    public Rectangle Bounds { get; init; }
    
    public TextureResource(string name)
    {
        Name = name;
        Width = Texture.width;
        Height = Texture.height;
        Bounds = new Rectangle(0, 0, Texture.width, Texture.height);
    }

    public void Destroy()
    {
        Raylib.UnloadTexture(Texture);
    }
}