namespace MapleSyrup.Gameplay.Map;

public class MapLayer
{
    public bool IsActive = false;
    public readonly List<MapItem> mapItems = new();

    public MapLayer()
    {
        
    }

    public void AddItem(MapItem item)
    {
        
    }

    public void Clear()
    {
        mapItems.Clear();
    }
}