using MapleSharp.Core.Interface;

namespace MapleSharp.Core;

public class Engine
{
    private readonly List<ISubsystem> subsystems = new();
    
    public Engine()
    {
        
    }

    public void AddSubsystem(ISubsystem subsystem)
    {
        if (subsystems.Any(x => x.GetType() == subsystem.GetType()))
        {
            Console.WriteLine("[Engine] Attempted to add subsystem that already exists.");
            return;
        }
        
        subsystems.Add(subsystem);
    }

    public void RemoveSubsystem<T>()
    {
        if (subsystems.Any(x => x.GetType() == typeof(T)))
        {
            subsystems.RemoveAll(x => x.GetType() == typeof(T));
            return;
        }
        
        Console.WriteLine("[Engine] Attempted to remove subsystem that does not exist.");
    }

    public T GetSubsystem<T>()
    {
        return (T)subsystems.Find(x => x is T) ?? throw new NullReferenceException("[Engine] Attempted to get subsystem that does not exist.");
    }

    public void Update(float timeDelta)
    {
        for (int i = 0; i < subsystems.Count; i++)
        {
            subsystems[i].Update(timeDelta);
        }
    }
}