using MapleSyrup.Event;
using MapleSyrup.Map;

namespace MapleSyrup.Managers;

public class MapManager : IManager
{
    private ManagerLocator? _locator;
    private MapleMap? _current;

    public MapManager()
    {
        
    }
    
    public void Initialize(ManagerLocator locator)
    {
        _locator = locator;
    }

    public MapleMap Create(int mapId)
    {
        if (_current != null)
            _current.Unload();
        
        _current = new MapleMap(GenerateId(mapId), _locator);
        _current.Load();

        return _current;
    }

    private string GenerateId(int mapId)
    {
        switch (mapId)
        {
            case var _ when mapId < 100000: 
                return $"0000{mapId}";
            case var _ when mapId is > 100000 and < 9999999:
                return $"00{mapId}";
            default:
                return $"{mapId}";
        }
    }

    public void ChangeMap(int id)
    {
        var events = _locator.GetManager<EventManager>();
        events.Dispatch(EventFlag.OnMapChanged);
    }

    public void Shutdown()
    {
    }
}