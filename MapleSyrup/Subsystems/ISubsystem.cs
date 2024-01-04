using MapleSyrup.Core;

namespace MapleSyrup.Subsystems;

public interface ISubsystem
{
    /// <summary>
    /// Copies the GameContext instance
    /// </summary>
    GameContext Context { get; }
    
    /// <summary>
    /// Initializes the SubSystem and any variables.
    /// </summary>
    /// <param name="context"></param>
    void Initialize(GameContext context);
    
    /// <summary>
    /// Disposes of any data used within the subsystem
    /// </summary>
    void Shutdown();
}