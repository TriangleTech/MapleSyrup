using MapleSyrup.GameObjects;
using MapleSyrup.GameObjects.Avatar;
using MapleSyrup.GameObjects.Components;
using MapleSyrup.Nx;
using MapleSyrup.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Managers;

public class ActorManager
{
    private SafeSortedSet<Actor> _actors;
    private static ActorManager _instance;
    private event Action<Actor> actorRemoved;
    private event Action<Actor> actorAdded;
    private event Action<Actor> actorChanged;
    private Thread actorThread;

    private Queue<Actor> toBeAdded, toBeRemoved, toBeChanged;
    private Queue<Actor> randomQueue;

    public static ActorManager Instance => _instance;

    public SafeSortedSet<Actor> Actors => _actors;

    public ActorManager(Application app)
    {
        _instance = this;
        _actors = new();
        toBeAdded = new();
        toBeRemoved = new();
        toBeChanged = new();
        randomQueue = new();
        actorAdded += OnActorAdded;
        actorRemoved += OnActorRemoved;
        actorChanged += OnActorChanged;
    }

    private void OnActorAdded(Actor obj)
    {
        _actors.Add(obj);
    }

    private void OnActorRemoved(Actor obj)
    {
        _actors.Remove(obj);
    }

    private void OnActorChanged(Actor obj)
    {
        _actors.Remove(obj);
        _actors.Add(obj);
    }

    public Mob CreateMob(string mobId, Vector2 pos)
    {
        var mob = new Mob();
        mob.Position = pos;
        mob.Layer = ActorLayer.TileLayer5;
        mob.Z = _actors.Count();
        var resource = ResourceManager.Instance;
        var nx = resource["Mob"];
        var node = nx.GetNode(mobId);

        foreach (var stateNode in node.Children)
        {
            if (stateNode.Name == "info") // TODO: Handle this when it matters.
                continue;

            var state = new Animation();
            foreach (var frame in stateNode.Children)
            {
                if (frame.Name.Length > 3)
                    continue; // TODO: Deal with this later.

                var texture = frame.GetTexture(resource.GraphicsDevice).Get();
                var i = int.Parse(frame.Name);
                state.AddFrame(
                    frame.Has("delay", out var delay) ? delay.GetInt() : 150,
                    ref texture);
            }

            mob.StateMachine.AddState(stateNode.Name, state);
        }
        
        return mob;
    }

    /// <summary>
    /// Creates a tile and adds it to the Actor list. We don't need to add a return, we don't modify tiles dynamically.
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="texture"></param>
    /// <param name="position"></param>
    /// <param name="origin"></param>
    /// <param name="zIndex"></param>
    public void CreateTile(ActorLayer layer, ref Texture2D texture, Vector2 position, Vector2 origin, int zIndex)
    {
        var tile = new Tile
        {
            Layer = layer,
            Z = zIndex,
            Position = position,
            Origin = origin,
            Texture = texture
        };

        Task.Run(() => actorAdded?.Invoke(tile));
    }

    public void CreateObject(ActorLayer layer, ref Texture2D texture, Vector2 position, Vector2 origin, int zIndex, int delay)
    {
        var obj = new MapObj()
        {
            Animation = new(),
            Layer = layer,
            Position = position,
            Origin = origin,
            Z = zIndex,
        };
        
        obj.Animation.AddFrame(delay, ref texture);
        Task.Run(() => actorAdded?.Invoke(obj));
    }
    
    public MapObj CreateObject(ActorLayer layer, Vector2 position, Vector2 origin, int zIndex)
    {
        var obj = new MapObj()
        {
            Animation = new(),
            Layer = layer,
            Position = position,
            Origin = origin,
            Z = zIndex,
        };
        Task.Run(() => actorAdded?.Invoke(obj));
        
        return obj;
    }
    
    public T CreateTestActor<T>(ActorLayer layer, Vector2 position, Vector2 origin, int zIndex) where T : Actor
    {
        var actor = Activator.CreateInstance<T>();
        actor.Layer = layer;
        actor.Position = position;
        actor.Origin = origin;
        actor.Z = zIndex;

        return actor;
    }

    public Portal CreatePortal(NxNode node, Vector2 position, Vector2 origin)
    {
        var portal = new Portal()
        {
            Node = node,
            Animation = new(),
            Layer = ActorLayer.TileLayer7,
            Position = position,
            Origin = origin,
            Z = 0
        };
        
        Task.Run(() => actorAdded?.Invoke(portal));
        return portal;
    }

    public void Add(Actor actor)
    {
        actor.Visible = false;
        toBeAdded.Enqueue(actor);

        if (toBeAdded.Count > 5)
            Task.Run(() => actorAdded?.Invoke(actor));
    }

    public void AddRandom(Actor actor)
    {
        Console.WriteLine("[RANDOM] Actor Added");
        randomQueue.Enqueue(actor);
    }

    public void SendIt()
    {
        if (randomQueue.Count > 0)
            Task.Run(() => actorAdded?.Invoke(randomQueue.Dequeue()));
    }

    public void Remove(Actor actor)
    {
        actor.Visible = false;
        toBeRemoved.Enqueue(actor);

        if (toBeRemoved.Count > 5)
            Task.Run(() => actorRemoved?.Invoke(actor));
    }

    public void Change(Actor actor)
    {
        actor.Visible = false;
        toBeChanged.Enqueue(actor);

        if (toBeChanged.Count > 1)
            Task.Run(() => actorChanged?.Invoke(actor));
    }

    public void Destroy()
    {
        foreach (var actor in _actors)
        {
            actor.Clear();
        }
    }
}