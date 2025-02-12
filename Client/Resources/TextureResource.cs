using System.Numerics;
using ZeroElectric.Vinculum;

namespace Client.Resources;

/// <summary>
/// The <c>TextureResource</c> class encapsulates a texture and its underlining information.
/// Primarily used for static and animating textures, <c>TextureResource</c> makes it easy
/// to obtain the information of the texture.
/// </summary>
public class TextureResource : IResource
{
    public string Name { get; init; }
    public ResourceType ResourceType { get; init; } = ResourceType.Texture;

    /// <summary>
    /// Gets the texture information associated with the resource.
    /// </summary>
    public required Texture Texture { get; init; }
    
    /// <summary>
    /// Gets the origin of the associated <see cref="Texture"/>
    /// </summary>
    public required Vector2 Origin { get; init; }

    /// <summary>
    /// Gets the frame delay of the assocated <see cref="Texture"/>.
    /// May be ignored if the resource is not part of an animation.
    /// </summary>
    public float Delay { get; init; } = 0f;

    public Color Color { get; set; } = Raylib.WHITE;
    
    /// <summary>
    /// Gets the width of the associated <see cref="Texture"/>
    /// </summary>
    public int Width { get; init; }
    
    /// <summary>
    /// Gets the height of the associated <see cref="Texture"/>
    /// </summary>
    public int Height { get; init; }
    
    /// <summary>
    /// Gets the boundary of the associated <see cref="Texture"/>
    /// </summary>
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