using ZeroElectric.Vinculum;

namespace Client.Resources;

public interface IResource
{
    public string Name { get; init; }
    public ResourceType ResourceType { get; init; }

    public  void Destroy();
}