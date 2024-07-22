using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Client.Actors;
using Client.Avatar;
using Client.Map;
using Client.Net;
using Client.NX;
using Raylib_CsLo;

namespace Client.Managers;

public class ActorManager : IManager
{
   private readonly Dictionary<ActorLayer, List<IActor>> _actors;
    private readonly Queue<IActor> _actorsToRemove, _actorsToAdd;
    private readonly Queue<(IActor, ActorLayer)> _actorsToChange;
    private readonly object _threadLock;
    private uint _actorCount;

    public ActorManager()
    {
        _actors = new();
        _actorsToRemove = new();
        _actorsToAdd = new();
        _actorsToChange = new();
        _threadLock = new();
        _actorCount = 0;
    }

    public void Initialize()
    {
        _actors[ActorLayer.Background] = new ();
        _actors[ActorLayer.TileLayer0] = new ();
        _actors[ActorLayer.TileLayer1] = new ();
        _actors[ActorLayer.TileLayer2] = new ();
        _actors[ActorLayer.TileLayer3] = new ();
        _actors[ActorLayer.TileLayer4] = new ();
        _actors[ActorLayer.TileLayer5] = new ();
        _actors[ActorLayer.TileLayer6] = new ();
        _actors[ActorLayer.TileLayer7] = new ();
        _actors[ActorLayer.Effects] = new ();
        _actors[ActorLayer.Foreground] = new ();
    }

    public void Shutdown()
    {
        _actorCount = 0;
        ClearActors();
    }

    /// <summary>
    /// Create a specific actor, prior to client rendering. For any additional actors after rendering,
    /// please use AddActor or SpawnActor methods.
    /// </summary>
    /// <param name="actor"></param>
    /// <typeparam name="T"></typeparam>
    public void Create<T>(T actor) where T : IActor
    {
        lock (_threadLock)
        {
            actor.ID = _actorCount++;
            _actors[actor.Layer].Add(actor);
        }
    }

    public void CreateBackground(NxNode background)
    {
        lock (_threadLock)
        {
            var bS = background.GetString("bS");
            var no = background.GetInt("no");
            var x = background.GetInt("x");
            var y = background.GetInt("y");
            var rx = background.GetInt("rx");
            var ry = background.GetInt("ry");
            var cx = background.GetInt("cx");
            var cy = background.GetInt("cy");
            var a = background.GetInt("a");
            var front = background.GetInt("front");
            var ani = background.GetInt("ani");
            var f = background.GetInt("f");

            if (ani == 1)
            {
                var backgroundSet = ServiceLocator.Get<NxManager>().Get(MapleFiles.Map).GetNode($"Back/{bS}.img");
                var back = backgroundSet["ani"][$"{no}"];
                var frames = new List<Texture>(back.ChildCount);
                for (var j = 0; j < back.ChildCount - 1; j++)
                {
                    var texture = back.GetTexture($"{j}");
                    frames.Add(texture);
                }
                SpawnActor(new Background(back, frames, new Vector2(x, y), cx, cy, rx, ry, front == 1 ? ActorLayer.Foreground : ActorLayer.Background));
            }
            else
            {
                var backgroundSet = ServiceLocator.Get<NxManager>().Get(MapleFiles.Map).GetNode($"Back/{bS}.img");
                var texture = backgroundSet["back"].GetTexture($"{no}");
                var origin = backgroundSet["back"][$"{no}"].GetVector("origin");
                SpawnActor(new Background(background, texture, new Vector2(x, y), origin, cx, cy, rx, ry, front == 1 ? ActorLayer.Foreground : ActorLayer.Background));
            }
        }
    }

    public void CreateTile(NxNode tileSet, int layer, NxNode tile)
    {
        lock (_threadLock)
        {
            var x = tile.GetInt("x");
            var y = tile.GetInt("y");
            var no = tile.GetInt("no");
            var u = tile.GetString("u");
            var texture = tileSet[u].GetTexture($"{no}");
            var origin = tileSet[u][$"{no}"].GetVector("origin");
            var z = tileSet[u][$"{no}"].GetInt("z");
            var zM = tile.GetInt("zM");
            var order = z + 10 * (3000 * (int)(ActorLayer.TileLayer0 + layer) - zM) - 1073721834;
            SpawnActor(new MapObject(tile, texture, new Vector2(x, y), origin, ActorLayer.TileLayer0 + layer, order));
        }
    }

    public void SpawnNetworkActor()
    {
        
    }

    public void SpawnActor(IActor actor)
    {
        lock (_threadLock)
        {
            actor.ID = _actorCount++;
            _actorsToAdd.Enqueue(actor);
        }
    }

    public void RemoveActor(IActor actor)
    {
        lock (_threadLock)
            _actorsToRemove.Enqueue(actor);
    }

    public void ChangeActorLayer(IActor actor, ActorLayer layer)
    {
        lock (_threadLock)
            _actorsToChange.Enqueue((actor, layer));
    }
    
    #region IActor Locators

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

    public bool GetActorAt(Vector2 position, out IActor? found)
    {
        lock (_threadLock)
        {
            foreach (IActor? actor in _actors[ActorLayer.TileLayer0].Where(actor => Raylib.CheckCollisionPointRec(position, actor.Bounds) 
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
        _actors[ActorLayer.TileLayer0].Sort(new ActorCompare<IActor>());
        _actors[ActorLayer.TileLayer1].Sort(new ActorCompare<IActor>());
        _actors[ActorLayer.TileLayer2].Sort(new ActorCompare<IActor>());
        _actors[ActorLayer.TileLayer3].Sort(new ActorCompare<IActor>());
        _actors[ActorLayer.TileLayer4].Sort(new ActorCompare<IActor>());
        _actors[ActorLayer.TileLayer5].Sort(new ActorCompare<IActor>());
        _actors[ActorLayer.TileLayer6].Sort(new ActorCompare<IActor>());
        _actors[ActorLayer.TileLayer7].Sort(new ActorCompare<IActor>());
        _actors[ActorLayer.Effects].Sort(new ActorCompare<IActor>()); // might not need to be sorted.
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
                _actors[actorToAdd.Layer].Sort(new ActorCompare<IActor>());
            }

            // Process actors whose layers need to be changed
            while (_actorsToChange.Count > 0)
            {
                var (actorToChange, newLayer) = _actorsToChange.Dequeue();
                _actors[actorToChange.Layer].Remove(actorToChange);
                actorToChange.Layer = newLayer;
                _actors[newLayer].Add(actorToChange);
                _actors[newLayer].Sort(new ActorCompare<IActor>());
            }
        }
    }
    
    #endregion
}