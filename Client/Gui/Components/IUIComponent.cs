using System.Numerics;
using Client.Gui.Panels;
using Client.NX;
using Raylib_CsLo;

namespace Client.Gui.Components;

public interface IUIComponent
{
    public IUIPanel Parent { get; init; }
    public int ID { get; init; }
    /// <summary>
    /// Gets or sets the name of the UI Node, this can be used to retrieve it later on.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Gets or sets the position of the UI based on the parent container's position.
    /// </summary>
    public Vector2 Position { get; set; }
    public Vector2 ScreenOffset { get; internal set; }
    
    /// <summary>
    /// Gets the bounds of the UI, set internally only.
    /// </summary>
    public Rectangle Bounds { get; internal set; }
    public Rectangle DstRectangle { get; internal set; }
    
    /// <summary>
    /// Gets the Node used to retrieve data, if it has one, not required.
    /// </summary>
    public NxNode? Node { get; init; }
    public bool Active { get; set; }
    public string TexturePath { get; set; }
    
    /// <summary>
    /// Draws the UI Node at a specific interval, referencing the parents position.
    /// </summary>
    /// <param name="parentPos">Position of the container</param>
    /// <param name="frameTime">Time since laster frame.</param>
    void Draw(Vector2 parentPos, float frameTime);
    
    /// <summary>
    /// Updates the UI Node at a specific interval, referencing the parents position.
    /// </summary>
    /// <param name="parentPos">Position of the container</param>
    /// <param name="frameTime">Time since laster frame.</param>
    void Update(Vector2 parentPos, float frameTime);
    
    /// <summary>
    /// Clears any assets used within the UI Node.
    /// </summary>
    public void Clear();
}