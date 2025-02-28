using System.Numerics;
using ZeroElectric.Vinculum;

namespace MapleSyrup.Resources;

/// <summary>
/// The <c>MappedResource</c> class encapsulates the data associated with
/// a resource which contains various coordinates pointing within itself.
/// A <c>MappedResource</c> can be considered a combination of a texture
/// and a series of vectors.
/// Examples of this may include equipment, body parts, and so on.
/// </summary>
public class MappedResource : ResourceBase
{
    public required Dictionary<string, Vector2> Map { get; init; } = new();

    public MappedResource(string directory, string name)
     : base(directory, name)
    {
        ResourceType = ResourceType.Mapped;
    }

    public Vector2 this[string key] => Map[key];

    public override void Destroy()
    {
        base.Destroy();
        Map.Clear();
    }
}