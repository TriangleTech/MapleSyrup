using System.Numerics;
using ZeroElectric.Vinculum;

namespace Client.ECS.Components.Map;

public class BackgroundObj
    : IComponent
{
    public required int Owner { get; init; }
    public required List<string> Textures { get; init; }
    public int Type { get; init; } = 0;
    public int Cx { get; init; } = 0;
    public int Cy { get; init; } = 0;
    public int Rx { get; init; } = 0;
    public int Ry { get; init; } = 0;
    public int Alpha { get; init; } = 255;
    public bool Animated { get; init; } = false;
    
    /// <summary>
    /// Returns the current value of the delay of the frame.
    /// </summary>
    public float FrameDelay { get; set; } = 0;
    
    /// <summary>
    /// Returns the current frame of the animation.
    /// </summary>
    public int Frame { get; set; } = 0;
    public int FrameCount => Textures.Count;
}