using ZeroElectric.Vinculum;

namespace Client.ECS.Components.Common;

public class RenderSprite
    : IComponent
{
    public required int Owner { get; init; }
    public required Texture Texture;
}