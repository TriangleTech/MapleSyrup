using System.Numerics;
using MSClient.Net;
using MSClient.NX;
using Raylib_CsLo;

namespace MSClient.Actors;

public abstract class Actor(NxNode? node, ActorType type)
{
    protected string _name = "";
    protected ActorType _type = ActorType.None;
    protected int _zIndex = 0;
    protected ActorLayer _layer;
    protected bool _visible = true;
    protected NxNode _node = node;
    protected Rectangle _bounds = new();
    protected Vector2 _position = Vector2.Zero, _origin = Vector2.Zero;

    public Vector2 Position => _position - _origin;
    public Rectangle Bounds => _bounds;
    public ActorType ActorType => _type;

    public NxNode Node
    {
        get => _node;
        set => _node = value;
    }

    public string Name
    {
        get => _name;
        set => _name = value;
    }

    public int Z
    {
        get => _zIndex;
        set => _zIndex = value;
    }

    public ActorLayer Layer
    {
        get => _layer;
        set => _layer = value;
    }

    public bool Visible
    {
        get => _visible;
        set => _visible = value;
    }

    public abstract void Clear();
    public abstract void Draw(float frameTime);
    public abstract void Update(float frameTime);
    public abstract void ProcessPacket(PacketResponse response);
}