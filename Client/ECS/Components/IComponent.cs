namespace Client.ECS.Components;

public interface IComponent
{
    public int Owner { get; init; }
}