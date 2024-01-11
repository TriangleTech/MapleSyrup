using MapleSyrup.Event;
using MapleSyrup.Managers;

namespace MapleSyrup.Map;

public class MapleMap : IEventListener
{
    public EventFlag Flags { get; }
    private readonly ManagerLocator _locator;
    
    public MapleMap(string mapId, ManagerLocator locator)
    {
        Flags = EventFlag.OnMapLoaded | EventFlag.OnMapChanged | EventFlag.OnMapUnloaded;
        _locator = locator;
        _locator.GetManager<EventManager>().Register(this);
    }
    
    public void ProcessEvent(EventFlag flag)
    {
        Console.WriteLine(flag.ToString());
    }

    public void Unload()
    {
        var events = _locator.GetManager<EventManager>();
        events.Dispatch(EventFlag.OnMapUnloaded);
    }

    public static bool operator&(MapleMap map, EventFlag flag)
    {
        return (map.Flags & flag) != 0;
    }
}