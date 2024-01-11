namespace MapleSyrup.EC;

public interface IComponent
{
    public Entity Parent { get; set; }
    public ComponentFlag Flag { get; }

    public static bool operator &(Entity entity, IComponent component)
    {
        return (entity.ComponentFlag & component.Flag) != 0;
    }
}