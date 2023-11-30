using MapleSyrup.Core;

namespace MapleSyrup.Subsystems;

public interface ISubsystem
{
    GameContext Context { get; }
    
    void Initialize(GameContext context);
    void Shutdown();
}