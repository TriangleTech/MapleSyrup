namespace Client.ECS;

public class Entity
{
    /// <summary>
    /// Returns the ID of the entity.
    /// </summary>
    public required int Id { get; init; }
    
    /// <summary>
    /// Returns the [optional] name for the entity.
    /// </summary>
    public string Name { get; init; } = "Default";
    
    /// <summary>
    /// Returns the [required] tag for the entity.
    /// </summary>
    public required string Tag { get; init; } = "Default";

    public int Layer { get; set; } = 0;
    
    /// <summary>
    /// Returns the visibility of the entity.
    /// </summary>
    public bool Visible { get; set; } = true;
}