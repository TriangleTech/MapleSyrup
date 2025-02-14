using System.Text.Json.Serialization;

namespace MapleSyrup.Scenes.Map;

[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true, 
    WriteIndented = true, 
    GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(MapleMap))]
internal partial class MapleMapContext : JsonSerializerContext
{
    
}