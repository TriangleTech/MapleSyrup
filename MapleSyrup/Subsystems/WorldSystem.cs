using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.Gameplay;

namespace MapleSyrup.Subsystems;

public class WorldSystem : ISubsystem
{
    public GameContext Context { get; private set; }
    public MapleWorld Current { get; private set; }
    public void Initialize(GameContext context)
    {
        Context = context;
    }

    public void Shutdown()
    {
    }

    public MapleWorld Create(string id)
    {
        var eventData = new EventData()
        {
            [DataType.String] = id
        };
        Current = new MapleWorld(Context, id);
        Context.SendEvent(EventType.WorldCreated, eventData);
        
        return Current;
    }
}