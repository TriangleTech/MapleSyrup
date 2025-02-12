using ZeroElectric.Vinculum;

namespace Client.Resources;

public interface IResource
{
    /// <summary>
    /// Gets the name of the resource. This value is used to
    /// obtain the resource through the <see cref="ResourceFactory"/>
    /// </summary>
    public string Name { get; init; }
    
    /// <summary>
    /// NOT USED - YET
    /// </summary>
    public ResourceType ResourceType { get; init; }
    
    /// <summary>
    /// Releases any resources allocated by the inheriting class.
    /// </summary>
    public  void Destroy();
}