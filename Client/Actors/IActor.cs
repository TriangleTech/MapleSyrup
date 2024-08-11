using System.Numerics;
using Client.Net;
using Client.NX;
using Raylib_CsLo;

namespace Client.Actors;

public interface IActor
{
    /// <summary>
    /// Gets the id of the actor, assigned upon initialization.
    /// </summary>
    public int ID { get; init; }
    
    /// <summary>
    /// Gets or sets the name of the actor, can be used to find the actor later one.
    /// Obituary objects, such as tiles, don't need name.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Gets or sets the depth of the actor on the layer the actor is in.
    /// </summary>
    public int Z { get; set; }


    /// <summary>
    /// Gets or sets the visibility of the actor.
    /// </summary>
    public bool Visible { get; set; } 
    
    /// <summary>
    /// Gets or sets the position of the actor.
    /// </summary>
    public Vector2 Position { get; set; }
    
    /// <summary>
    /// Gets or sets the origin of the actor.
    /// </summary>
    public Vector2 Origin { get; set; }
    
    public Vector2 ScreenOffset { get; internal set; }
    
    /// <summary>
    /// Gets or sets the layer of the actor, please don't do this carelessly.
    /// </summary>
    public ActorLayer Layer { get; set; }
    
    /// <summary>
    /// Gets the actor type, set upon initialization of the actor.
    /// </summary>
    public ActorType ActorType { get; init; }
    
    /// <summary>
    /// Gets or sets the bounds of the actor...might change this later.
    /// </summary>
    public Rectangle Bounds { get; internal set; }
    
    public Rectangle DstRectangle { get; internal set; }
    
    /// <summary>
    /// Gets the node the actor will be using for data acquisition. Sets the node internally
    /// </summary>
    public NxNode? Node { get; internal set; }
    
    /// <summary>
    /// Draws the actor at a fixed interval.
    /// </summary>
    /// <param name="frameTime">Time since last frame.</param>
    void Draw(float frameTime);
    
    /// <summary>
    /// Updates the actor at a fixed interval.
    /// </summary>
    /// <param name="frameTime">Time since last frame.</param>
    void Update(float frameTime);
    
    /// <summary>
    /// Clears all assets used within the actor.
    /// </summary>
    void Clear();
}