using System.Collections.Concurrent;
using System.Numerics;
using Client.Actors;
using Client.Avatar;
using Client.Map;
using Client.Net;
using Raylib_CsLo;

namespace Client.Scene;

public interface IWorld
{
    public List<IActor> Actors { get; init; }
    public ConcurrentQueue<IActor> ToAdd { get; init; }
    public ConcurrentQueue<IActor> ToRemove { get; init; }
    public ConcurrentQueue<(IActor, ActorLayer)> ToShift { get; init; }
    
    public int ActorCount { get; internal set; }
    public bool Cleared { get; internal set; }
    
    /// <summary>
    /// Represents a camera used for rendering and viewing a scene.
    /// </summary>
    public Camera2D Camera { get; }

    /// <summary>
    /// Loads the content for the world.
    /// </summary>
    public void Load();

    /// <summary>
    /// Clears all assets used within the world.
    /// </summary>
    public void Clear();
    
    /// <summary>
    /// Draws the world assets at a fixed interval.
    /// </summary>
    /// <param name="frameTime">Time since the last frame.</param>
    public void Draw(float frameTime);
    
    /// <summary>
    /// Updates the world at a fixed interval.
    /// </summary>
    /// <param name="frameTime">Time since last frame.</param>
    public void Update(float frameTime);
    
    /// <summary>
    /// Processes any packets the world needs to reactor, otherwise they're ignored.
    /// </summary>
    /// <param name="packet">The inbound packet from the network.</param>
    public void ProcessPacket(InPacket packet);
    
    /// <summary>
    /// Updates the position of the camera based on a specified position.
    /// </summary>
    /// <param name="position">The position the camera needs to reference.</param>
    public void UpdateCamera(Vector2 position);

    public void UpdateZoom(float zoom);

    public int GenerateID()
    {
        return ActorCount++;
    }
    
    public void AddActor(IActor actor)
    {
        ToAdd.Enqueue(actor);
    }

    public void RemoveActor(IActor actor)
    {
        ToRemove.Enqueue(actor);
    }

    public void ShiftActor(IActor actor, ActorLayer layer)
    {
        ToShift.Enqueue((actor, layer));
    }

    public sealed void ProcessPending()
    {
        var _lock = new object();
        lock (_lock)
        {
            // Process actors to be added
            while (!ToAdd.IsEmpty)
            {
                ToAdd.TryDequeue(out var actorToAdd);
                Actors.Add(actorToAdd);
                Actors.Sort(new ActorCompare<IActor>());
                ActorCount++;
                if (actorToAdd is Player)
                {
                    Console.WriteLine($"Player spawn at {actorToAdd.Position} & Layer: {actorToAdd.Layer}");
                }
            }

            // Process actors whose layers need to be changed
            while (!ToShift.IsEmpty)
            {
                (IActor actor, ActorLayer layer) result;
                ToShift.TryDequeue(out result);
                result.actor.Layer = result.layer;
                Actors.Sort(new ActorCompare<IActor>());
            }

            // Process actors to be removed
            while (!ToRemove.IsEmpty)
            {
                ToRemove.TryDequeue(out var actorToRemove);
                Actors.Remove(actorToRemove);
                actorToRemove.Clear();
            }
        }
    }
    
    public sealed void SortLayers()
    {
        Actors.Sort(new ActorCompare<IActor>());
    }
}