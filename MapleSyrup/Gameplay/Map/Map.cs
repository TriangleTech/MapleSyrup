using MapleSyrup.ECS;
using MapleSyrup.ECS.Components;

namespace MapleSyrup.Gameplay.Map;

public class Map 
{
    private List<MapLayer> mapLayers;
    private MapleWorld currentWorld;

    public Map(MapleWorld world)
    {
        mapLayers = new List<MapLayer>();
        currentWorld = world;
    }

    
}