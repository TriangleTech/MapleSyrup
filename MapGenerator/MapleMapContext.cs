using System.Text.Json.Serialization;

namespace MapGenerator;

[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true, 
    WriteIndented = true, 
    GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(MapleMap))]
internal partial class MapleMapContext : JsonSerializerContext
{
    
}