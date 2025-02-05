using System.Numerics;

namespace Client.Resources;

public class MappedResource : IResource
{
    private readonly Dictionary<string, Vector2> _mappings;

    public string Name { get; init; } = string.Empty;
    public ResourceType ResourceType { get; init; } = ResourceType.Mapped;

    public MappedResource(string name)
    {
        _mappings = new Dictionary<string, Vector2>(10);
    }

    public Vector2 this[string key] => _mappings[key];

    public void Destroy()
    {
        throw new NotImplementedException();
    }
}