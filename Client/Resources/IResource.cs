using ZeroElectric.Vinculum;

namespace Client.Resources;

public interface IResource
{
    public ResourceType ResourceType { get; init; }
    public string Name { get; init; }

    public  void Destroy();
}