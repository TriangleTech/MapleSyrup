using MapleSyrup.Core.Event;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Core;

/// <summary>
/// The spine of the entire engine, without this nothing will work.
/// </summary>
public class GameContext
{
    private List<ISubsystem> subsystems;
    public readonly GraphicsDevice GraphicsDevice;
    
    public GameContext(Game game)
    {
        GraphicsDevice = game.GraphicsDevice;
        subsystems = new List<ISubsystem>();
    }
    
    public void AddSubsystem<T>() where T : ISubsystem, new()
    {
        var subsystem = new T();
        subsystem.Initialize(this);
        subsystems.Add(subsystem);
    }
    
    public void RemoveSubsystem<T>() where T : ISubsystem
    {
        var subsystem = subsystems.Find(system => system is T);
        subsystem.Shutdown();
        subsystems.Remove(subsystem);
    }
    
    public T GetSubsystem<T>() where T : ISubsystem
    {
        return (T) subsystems.Find(system => system is T);
    }
    
    public void Shutdown()
    {
        subsystems.ForEach(system => system.Shutdown());
        GC.Collect();
    }
}