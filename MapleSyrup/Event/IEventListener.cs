using MapleSyrup.EC;

namespace MapleSyrup.Event;

public interface IEventListener
{
    /// <summary>
    /// Contains the current flags that the listener is waiting for.
    /// </summary>
    EventFlag Flags { get; }
    
    /// <summary>
    /// Process an event based on the flag.
    /// </summary>
    /// <example>Player clicks a button, sends OnClick event.</example>
    /// <param name="flag"></param>
    void ProcessEvent(EventFlag flag);
    
    /// <summary>
    /// Process an event based on the flag and sends the entity calling the event.
    /// </summary>
    /// <example>Entity created, sent to the scene. </example>
    /// <example>Avatar collided with mob, send to physics and damage report.</example>
    /// <param name="flag"></param>
    /// <param name="entity"></param>
    void ProcessEvent(EventFlag flag, IEntity entity);

    /// <summary>
    /// Easier way of determining if the listener contains the specified flag.
    /// </summary>
    /// <param name="listener"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public static bool operator &(IEventListener listener, EventFlag flag)
    {
        return (listener.Flags & flag) != 0;
    }
}