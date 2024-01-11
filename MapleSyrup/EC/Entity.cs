using Microsoft.Xna.Framework;

namespace MapleSyrup.EC;

public class Entity
{
    public string Name;
    public string Tag;
    public Vector2 Position;
    public Vector2 Origin;
    public int zIndex;
    public EntityFlag EntityFlag;
    public ComponentFlag ComponentFlag;

    public Entity()
    {
        Name = "Entity";
        Tag = "Default";
        Position = Vector2.Zero;
        Origin = Vector2.Zero;
        zIndex = 0;
    }

    public Entity(string name, string tag)
    {
        Name = name;
        Tag = tag;
        Position = Vector2.Zero;
        Origin = Vector2.Zero;
        zIndex = 0;
    }

    public static bool operator &(Entity entity, EntityFlag flag)
    {
        return (entity.EntityFlag & flag) != 0;
    }

    public static bool operator &(Entity entity, ComponentFlag flag)
    {
        return (entity.ComponentFlag & flag) != 0;
    }
}