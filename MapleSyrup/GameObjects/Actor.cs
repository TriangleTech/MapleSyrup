using MapleSyrup.Nx;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.GameObjects;

public abstract class Actor : IComparable<Actor>
{
    private string _name;
    private int _zIndex;
    private ActorLayer _layer;
    private bool _visible;
    private NxNode _node;

    public Vector2 Position = Vector2.Zero;
    public Vector2 Origin = Vector2.Zero;

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

    public int CompareTo(Actor? other)
    {
        if (ReferenceEquals(this, other)) 
            return 0;
        if (ReferenceEquals(null, other)) 
            return 1;

        var layerComparison = _layer.CompareTo(other.Layer);
        if (layerComparison != 0)
            return layerComparison;

        var indexComparison = _zIndex.CompareTo(other.Z);
        if (indexComparison != 0)
            return indexComparison;
        return 1;
    }

    public abstract void Clear();
    public abstract void Draw(SpriteBatch spriteBatch);
    public abstract void Update(GameTime gameTime);

}