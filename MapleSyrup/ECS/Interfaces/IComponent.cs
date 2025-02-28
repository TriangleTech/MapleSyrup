namespace MapleSyrup.ECS.Interfaces;

public interface IComponent
{
    public int Owner { get; init; }
}