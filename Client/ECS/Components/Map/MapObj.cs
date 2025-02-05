using System.Numerics;
using ZeroElectric.Vinculum;

namespace Client.ECS.Components.Map;

public class MapObj
    : IComponent
{
    /// <summary>
    /// Returns the owning entity id.
    /// </summary>
    public required int Owner { get; init; }
    
    /// <summary>
    /// Returns the textures.
    /// </summary>
    public required List<string> Textures { get; init; }
    
    /// <summary>
    /// Returns the current value of the delay of the frame.
    /// </summary>
    public float FrameDelay { get; set; } = 0;
    
    /// <summary>
    /// Returns the current frame of the animation.
    /// </summary>
    public int Frame { get; set; } = 0;
    
    /// <summary>
    /// Returns the amount of frames in the animation.
    /// </summary>
    public int FrameCount => Textures.Count;
    
    /// <summary>
    /// Returns whether the animation is continuous.
    /// </summary>
    public bool Loop { get; set; } = true;
    
    /// <summary>
    /// Returns whether the animation is complete.
    /// </summary>
    public bool Finished => Frame >= FrameCount;
    
    /// <summary>
    /// Returns whether the animation is animation via alpha blending.
    /// </summary>
    public bool Blend { get; init; }
}