using System.Diagnostics.CodeAnalysis;
using MSClient.Actors;
using MSClient.Net;

namespace MSClient.Managers;

public class ActorManager : IManager
{
    private readonly SortedSet<Actor> _actors;
    private readonly Queue<Actor> _actorsToRemove, _actorsToAdd;
    private readonly Queue<(Actor, ActorLayer)> _actorsToChange;
    private readonly object _threadLock;

    public ActorManager()
    {
        _actors = new();
        _actorsToRemove = new();
        _actorsToAdd = new();
        _actorsToChange = new();
        _threadLock = new();
    }

    public void Initialize()
    {
        
    }

    public void Shutdown()
    {
        ClearActors();
    }

    public void Create<T>(T actor) where T : Actor
    {
        lock (_threadLock)
        {
            _actors.Add((T)actor);
        }
    }

    public void AddActor(Actor actor)
    {
        lock (_threadLock)
        {
            _actorsToAdd.Enqueue(actor);
        }
    }

    public void RemoveActor(Actor actor)
    {
        lock (_threadLock)
            _actorsToRemove.Enqueue(actor);
    }

    public void ChangeActorLayer(Actor actor, ActorLayer layer)
    {
        lock (_threadLock)
            _actorsToChange.Enqueue((actor, layer));
    }

    public void ClearActors()
    {
        lock (_threadLock)
        {
            foreach (var actor in _actors)
                actor.Clear();
            _actors.Clear();
        }
    }

    public void DispatchResponse(PacketResponse response)
    {
        lock (_threadLock)
        {
            foreach (var actor in _actors)
            {
                actor.ProcessPacket(response);
            }
        }
    }

    public void Update(float frameTime)
    {
        lock (_threadLock)
        {
            foreach (var actor in _actors)
            {
                actor.Update(frameTime);
            }
        }
    }

    public void Draw(float frameTime)
    {
        lock (_threadLock)
        {
            foreach (var actor in _actors)
            {
                actor.Draw(frameTime);
            }
        }
    }

    public void ValidateActors()
    {
        lock (_threadLock)
        {
            // Process actors to be removed
            while (_actorsToRemove.Count > 0)
            {
                var actorToRemove = _actorsToRemove.Dequeue();
                _actors.Remove(actorToRemove);
            }

            // Process actors to be added
            while (_actorsToAdd.Count > 0)
            {
                var actorToAdd = _actorsToAdd.Dequeue();
                _actors.Add(actorToAdd);
            }

            // Process actors whose layers need to be changed
            while (_actorsToChange.Count > 0)
            {
                var (actorToChange, newLayer) = _actorsToChange.Dequeue();
                _actors.Remove(actorToChange);
                actorToChange.Layer = newLayer;
                _actors.Add(actorToChange);
            }
        }
    }
}