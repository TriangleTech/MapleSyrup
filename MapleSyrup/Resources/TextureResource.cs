using System.Numerics;
using ZeroElectric.Vinculum;

namespace MapleSyrup.Resources;

/// <summary>
/// The <c>TextureResource</c> class encapsulates a texture and its underlining information.
/// Primarily used for static and animating textures, <c>TextureResource</c> makes it easy
/// to obtain the information of the texture.
/// </summary>
public class TextureResource : ResourceBase
{
    /// <summary>
    /// Gets the frame delay of the assocated <see cref="Texture"/>.
    /// May be ignored if the resource is not part of an animation.
    /// </summary>
    public float Delay { get; init; } = 0f;
    
    public TextureResource(string directory, string name)
    : base(directory, name)
    {
        ResourceType = ResourceType.Texture;
    }
}