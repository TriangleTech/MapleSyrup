namespace MapleSyrup.ECS;

public record Entity
{
    /// <summary>
    /// Returns the ID of the entity.
    /// </summary>
    public required int Id { get; init; }
    
    /// <summary>
    /// Returns the [optional] name for the entity.
    /// </summary>
    public required string Name { get; init; } = "Default";
    
    /// <summary>
    /// Returns the [required] tag for the entity.
    /// </summary>
    public required string Tag { get; init; } = "Default";

    public required int Layer { get; set; } = 0;
    
    /// <summary>
    /// Returns the visibility of the entity.
    /// </summary>
    internal bool Visible { get; set; } = true;
}