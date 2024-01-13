using MapleSyrup.EC.Components;
using MapleSyrup.Map;
using MapleSyrup.Managers;

namespace MapleSyrup.EC;

/// <summary>
/// Interface used to create various entity types. All entity types must have <see cref="ManagerLocator"/> as the first varible
/// in the constructor.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Name of the entity.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Contains the <see cref="EntityFlag"/> registered to the Entity.
    /// You are allow to add or remove flags as need.
    /// </summary>
    public EntityFlag Flags { get; set; }
    
    /// <summary>
    /// Contains the <see cref="ComponentFlag"/> registered to the Entity.
    /// Each flag determines what components are inside the Entity.
    /// You are allow to add or remove flags as need.
    /// </summary>
    public ComponentFlag CFlags { get; set; }
    
    /// <summary>
    /// The primary component for positioning
    /// </summary>
    public TransformComponent Transform { get; }
    
    /// <summary>
    /// The layer the entity sits on.
    /// </summary>
    public RenderLayer Layer { get; set; }
    
    /// <summary>
    /// Easier way to determine what flags are contained in the entity
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public static bool operator&(IEntity entity, EntityFlag flag)
    {
        return (entity.Flags & flag) != 0;
    }

    /// <summary>
    /// Easier way to determine what components are contained within the entity.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public static bool operator&(IEntity entity, ComponentFlag flag)
    {
        return (entity.CFlags & flag) != 0;
    }
}