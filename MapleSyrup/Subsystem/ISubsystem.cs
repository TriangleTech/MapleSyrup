using MapleSyrup.Core;

namespace MapleSyrup.Subsystem;

public interface ISubsystem
{
    MapleContext Context { get; }
    
    void Initialize(MapleContext context);
    void Shutdown();
}