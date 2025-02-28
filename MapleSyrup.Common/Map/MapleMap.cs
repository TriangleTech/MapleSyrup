using System.Text.Json.Serialization;

namespace MapleSyrup.Common.Map;

public class MapleMap
{
    //[JsonInclude]
    //public MapInfo MapInfo { get; set; } = new ();
    
    [JsonInclude]
    public List<MapBackground> Backgrounds { get; set; } = new();
    [JsonInclude]
    public List<MapObject> Objects { get; set; } = new();
    [JsonInclude]
    public List<MapTile> Tiles { get; set; } = new();
    
    [JsonInclude]
    public List<MapFoothold> Footholds { get; set; } = new();

    public void Clear()
    {
        Backgrounds.Clear();
        Objects.Clear();
        Tiles.Clear();
        Footholds.Clear();
    }
}