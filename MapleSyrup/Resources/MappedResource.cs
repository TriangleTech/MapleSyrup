using System.Numerics;
using MapleSyrup.Nx;
using ZeroElectric.Vinculum;

namespace MapleSyrup.Resources;

/// <summary>
/// The <c>MappedResource</c> class encapsulates the data associated with
/// a resource which contains various coordinates pointing within itself.
/// A <c>MappedResource</c> can be considered a combination of a texture
/// and a series of vectors.
/// Examples of this may include equipment, body parts, and so on.
/// </summary>
public class MappedResource : IResource
{
    public string Name { get; init; }
    public MapleFile MainFile { get; init; }
    public ResourceType ResourceType { get; init; } = ResourceType.Mapped;
    
    public Texture Texture { get; set; }
    public required Vector2 Origin { get; init; }
    public required Dictionary<string, Vector2> Map { get; init; } = new();
    public int Width { get; init; }
    public int Height { get; init; }
    public Rectangle Bounds { get; init; }

    public MappedResource(string name)
    {
        Name = name;
        Width = Texture.width;
        Height = Texture.height;
        Bounds = new Rectangle(0, 0, Texture.width, Texture.height);
    }

    public Vector2 this[string key] => Map[key];

    public void Destroy()
    {
        throw new NotImplementedException();
    }
}