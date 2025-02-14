using System.Text.Json.Serialization;

namespace MapGenerator;

public class MapleMap
{
    [JsonInclude]
    public List<MapBackground> Backgrounds { get; } = new();
    [JsonInclude]
    public List<MapObject> Objects { get; } = new();
    [JsonInclude]
    public List<MapTile> Tiles { get; } = new();

    public void Clear()
    {
        Backgrounds.Clear();
        Objects.Clear();
        Tiles.Clear();
    }
}