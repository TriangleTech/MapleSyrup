using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.GameObjects;

public abstract class Actor : IComparable<Actor>
{
    private string _name;
    private Vector2 _position, _origin;
    private int _zIndex;
    private ActorLayer _layer;

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
        return 0;
    }

    public abstract void Clear();
    public abstract void Draw(SpriteBatch spriteBatch);
    public abstract void Update(GameTime gameTime);

}