using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using MSClient.Actors;
using MSClient.Avatar;
using MSClient.Net;
using Raylib_CsLo;

namespace MSClient.Managers;

public class ActorManager : IManager
{
    private readonly Dictionary<ActorLayer, List<Actor>> _actors;
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
        _actors[ActorLayer.Background] = new List<Actor>();
        _actors[ActorLayer.TileLayer0] = new List<Actor>();
        _actors[ActorLayer.TileLayer1] = new List<Actor>();
        _actors[ActorLayer.TileLayer2] = new List<Actor>();
        _actors[ActorLayer.TileLayer3] = new List<Actor>();
        _actors[ActorLayer.TileLayer4] = new List<Actor>();
        _actors[ActorLayer.TileLayer5] = new List<Actor>();
        _actors[ActorLayer.TileLayer6] = new List<Actor>();
        _actors[ActorLayer.TileLayer7] = new List<Actor>();
        _actors[ActorLayer.Effects] = new List<Actor>();
        _actors[ActorLayer.Foreground] = new List<Actor>();
    }

    public void Shutdown()
    {
        ClearActors();
    }

    public void Create<T>(T actor) where T : Actor
    {
        lock (_threadLock)
        {
            _actors[actor.Layer].Add(actor);
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
    
    #region Actor Locators

    public bool GetPlayer(out Player? found)
    {
        lock (_threadLock)
        {
            foreach (var actor in _actors[ActorLayer.TileLayer0].Where(actor => actor.ActorType == ActorType.Player))
            {
                found = actor as Player;
                return true;
            }
            foreach (var actor in _actors[ActorLayer.TileLayer1].Where(actor => actor.ActorType == ActorType.Player))
            {
                found = actor as Player;
                return true;
            }
            foreach (var actor in _actors[ActorLayer.TileLayer2].Where(actor => actor.ActorType == ActorType.Player))
            {
                found = actor as Player;
                return true;
            }
            foreach (var actor in _actors[ActorLayer.TileLayer3].Where(actor => actor.ActorType == ActorType.Player))
            {
                found = actor as Player;
                return true;
            }
            foreach (var actor in _actors[ActorLayer.TileLayer4].Where(actor => actor.ActorType == ActorType.Player))
            {
                found = actor as Player;
                return true;
            }
            foreach (var actor in _actors[ActorLayer.TileLayer5].Where(actor => actor.ActorType == ActorType.Player))
            {
                found = actor as Player;
                return true;
            }
            foreach (var actor in _actors[ActorLayer.TileLayer6].Where(actor => actor.ActorType == ActorType.Player))
            {
                found = actor as Player;
                return true;
            }
            foreach (var actor in _actors[ActorLayer.TileLayer7].Where(actor => actor.ActorType == ActorType.Player))
            {
                found = actor as Player;
                return true;
            }

            found = null;
            return false;
        }
    }

    public bool GetActorAt(Vector2 position, out Actor? found)
    {
        lock (_threadLock)
        {
            foreach (Actor? actor in _actors[ActorLayer.TileLayer0].Where(actor => Raylib.CheckCollisionPointRec(position, actor.Bounds) 
                         && actor.ActorType == ActorType.Npc || actor.ActorType == ActorType.Player || actor.ActorType == ActorType.Reactor))
            {
                found = actor;
                return true;
            }
            foreach (var actor in _actors[ActorLayer.TileLayer1].Where(actor => Raylib.CheckCollisionPointRec(position, actor.Bounds) 
                         && actor.ActorType == ActorType.Npc || actor.ActorType == ActorType.Player || actor.ActorType == ActorType.Reactor))
            {
                found = actor;
                return true;
            }
            foreach (var actor in _actors[ActorLayer.TileLayer2].Where(actor => Raylib.CheckCollisionPointRec(position, actor.Bounds) 
                         && actor.ActorType == ActorType.Npc || actor.ActorType == ActorType.Player || actor.ActorType == ActorType.Reactor))
            {
                found = actor;
                return true;
            }
            foreach (var actor in _actors[ActorLayer.TileLayer3].Where(actor => Raylib.CheckCollisionPointRec(position, actor.Bounds) 
                         && actor.ActorType == ActorType.Npc || actor.ActorType == ActorType.Player || actor.ActorType == ActorType.Reactor))
            {
                found = actor;
                return true;
            }
            foreach (var actor in _actors[ActorLayer.TileLayer4].Where(actor => Raylib.CheckCollisionPointRec(position, actor.Bounds) 
                         && actor.ActorType == ActorType.Npc || actor.ActorType == ActorType.Player || actor.ActorType == ActorType.Reactor))
            {
                found = actor;
                return true;
            }
            foreach (var actor in _actors[ActorLayer.TileLayer5].Where(actor => Raylib.CheckCollisionPointRec(position, actor.Bounds) 
                         && actor.ActorType == ActorType.Npc || actor.ActorType == ActorType.Player || actor.ActorType == ActorType.Reactor))
            {
                found = actor;
                return true;
            }
            foreach (var actor in _actors[ActorLayer.TileLayer6].Where(actor => Raylib.CheckCollisionPointRec(position, actor.Bounds)
                         && actor.ActorType == ActorType.Npc || actor.ActorType == ActorType.Player || actor.ActorType == ActorType.Reactor))
            {
                found = actor;
                return true;
            }
            foreach (var actor in _actors[ActorLayer.TileLayer7].Where(actor => Raylib.CheckCollisionPointRec(position, actor.Bounds)
                         && actor.ActorType == ActorType.Npc || actor.ActorType == ActorType.Player || actor.ActorType == ActorType.Reactor))
            {
                found = actor;
                return true;
            }

            found = null;
            return false;
        }
    }
    
    #endregion

    #region Sort/Clear/Dispatch

    public void SortAll()
    {
        //_actors[ActorLayer.Background].Sort(new ActorCompare<Actor>()); // don't need to be sorted.
        _actors[ActorLayer.TileLayer0].Sort(new ActorCompare<Actor>());
        _actors[ActorLayer.TileLayer1].Sort(new ActorCompare<Actor>());
        _actors[ActorLayer.TileLayer2].Sort(new ActorCompare<Actor>());
        _actors[ActorLayer.TileLayer3].Sort(new ActorCompare<Actor>());
        _actors[ActorLayer.TileLayer4].Sort(new ActorCompare<Actor>());
        _actors[ActorLayer.TileLayer5].Sort(new ActorCompare<Actor>());
        _actors[ActorLayer.TileLayer6].Sort(new ActorCompare<Actor>());
        _actors[ActorLayer.TileLayer7].Sort(new ActorCompare<Actor>());
        _actors[ActorLayer.Effects].Sort(new ActorCompare<Actor>()); // might not need to be sorted.
        //_actors[ActorLayer.Foreground].Sort(new ActorCompare<Actor>()); // same as background.
    }

    public void ClearActors()
    {
        lock (_threadLock)
        {
            foreach (var actor in _actors[ActorLayer.Background])
            {
                actor.Clear();
            }
            foreach (var actor in _actors[ActorLayer.TileLayer0])
            {
                actor.Clear();
            }
            foreach (var actor in _actors[ActorLayer.TileLayer1])
            {
                actor.Clear();
            }
            foreach (var actor in _actors[ActorLayer.TileLayer2])
            {
                actor.Clear();   
            }
            foreach (var actor in _actors[ActorLayer.TileLayer3])
            {
                actor.Clear();
            }
            foreach (var actor in _actors[ActorLayer.TileLayer4])
            {
                actor.Clear();
            }
            foreach (var actor in _actors[ActorLayer.TileLayer5])
            {
                actor.Clear();
            }
            foreach (var actor in _actors[ActorLayer.TileLayer6])
            {
                actor.Clear();
            }
            foreach (var actor in _actors[ActorLayer.TileLayer7])
            {
                actor.Clear();
            }
            foreach (var actor in _actors[ActorLayer.Effects])
            {
                actor.Clear();
            }
            foreach (var actor in _actors[ActorLayer.Foreground])
            {
                actor.Clear();
            }
            _actors.Clear();
        }
    }

    public void DispatchResponse(PacketResponse response)
    {
        lock (_threadLock)
        {
            foreach (var actor in _actors[ActorLayer.Background])
            {
                actor.ProcessPacket(response);
            }
            foreach (var actor in _actors[ActorLayer.TileLayer0])
            {
                actor.ProcessPacket(response);
            }
            foreach (var actor in _actors[ActorLayer.TileLayer1])
            {
                actor.ProcessPacket(response);
            }
            foreach (var actor in _actors[ActorLayer.TileLayer2])
            {
                actor.ProcessPacket(response);
            }
            foreach (var actor in _actors[ActorLayer.TileLayer3])
            {
                actor.ProcessPacket(response);
            }
            foreach (var actor in _actors[ActorLayer.TileLayer4])
            {
                actor.ProcessPacket(response);
            }
            foreach (var actor in _actors[ActorLayer.TileLayer5])
            {
                actor.ProcessPacket(response);
            }
            foreach (var actor in _actors[ActorLayer.TileLayer6])
            {
                actor.ProcessPacket(response);
            }
            foreach (var actor in _actors[ActorLayer.TileLayer7])
            {
                actor.ProcessPacket(response);
            }
            foreach (var actor in _actors[ActorLayer.Effects])
            {
                actor.ProcessPacket(response);
            }
            foreach (var actor in _actors[ActorLayer.Foreground])
            {
                actor.ProcessPacket(response);
            }
        }
    }
    
    #endregion

    #region Update/Draw/Validate
    public void Update(float frameTime)
    {
        lock (_threadLock)
        {
            foreach (var actor in _actors[ActorLayer.Background])
            {
                actor.Update(frameTime);
            }
            foreach (var actor in _actors[ActorLayer.TileLayer0])
            {
                actor.Update(frameTime);
            }
            foreach (var actor in _actors[ActorLayer.TileLayer1])
            {
                actor.Update(frameTime);
            }
            foreach (var actor in _actors[ActorLayer.TileLayer2])
            {
                actor.Update(frameTime);   
            }
            foreach (var actor in _actors[ActorLayer.TileLayer3])
            {
                actor.Update(frameTime);   
            }
            foreach (var actor in _actors[ActorLayer.TileLayer4])
            {
                actor.Update(frameTime);   
            }
            foreach (var actor in _actors[ActorLayer.TileLayer5])
            {
                actor.Update(frameTime);   
            }
            foreach (var actor in _actors[ActorLayer.TileLayer6])
            {
                actor.Update(frameTime);   
            }
            foreach (var actor in _actors[ActorLayer.TileLayer7])
            {
                actor.Update(frameTime);   
            }
            foreach (var actor in _actors[ActorLayer.Effects])
            {
                actor.Update(frameTime);
            }
            foreach (var actor in _actors[ActorLayer.Foreground])
            {
                actor.Update(frameTime);
            }
        }
    }

    public void Draw(float frameTime)
    {
        lock (_threadLock)
        {
            foreach (var actor in _actors[ActorLayer.Background])
            {
                actor.Draw(frameTime);
            }
            foreach (var actor in _actors[ActorLayer.TileLayer0])
            {
                actor.Draw(frameTime);
            }
            foreach (var actor in _actors[ActorLayer.TileLayer1])
            {
                actor.Draw(frameTime);
            }
            foreach (var actor in _actors[ActorLayer.TileLayer2])
            {
                actor.Draw(frameTime);   
            }
            foreach (var actor in _actors[ActorLayer.TileLayer3])
            {
                actor.Draw(frameTime);   
            }
            foreach (var actor in _actors[ActorLayer.TileLayer4])
            {
                actor.Draw(frameTime);   
            }
            foreach (var actor in _actors[ActorLayer.TileLayer5])
            {
                actor.Draw(frameTime);   
            }
            foreach (var actor in _actors[ActorLayer.TileLayer6])
            {
                actor.Draw(frameTime);   
            }
            foreach (var actor in _actors[ActorLayer.TileLayer7])
            {
                actor.Draw(frameTime);   
            }
            foreach (var actor in _actors[ActorLayer.Effects])
            {
                actor.Draw(frameTime);
            }
            foreach (var actor in _actors[ActorLayer.Foreground])
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
                _actors[actorToRemove.Layer].Remove(actorToRemove);
            }

            // Process actors to be added
            while (_actorsToAdd.Count > 0)
            {
                var actorToAdd = _actorsToAdd.Dequeue();
                _actors[actorToAdd.Layer].Add(actorToAdd);
                _actors[actorToAdd.Layer].Sort(new ActorCompare<Actor>());
            }

            // Process actors whose layers need to be changed
            while (_actorsToChange.Count > 0)
            {
                var (actorToChange, newLayer) = _actorsToChange.Dequeue();
                _actors[actorToChange.Layer].Remove(actorToChange);
                actorToChange.Layer = newLayer;
                _actors[newLayer].Add(actorToChange);
                _actors[newLayer].Sort(new ActorCompare<Actor>());
            }
        }
    }
    
    #endregion
}