using System.Text.Json.Serialization;

namespace MapleSyrup.Common.Map;

[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true, 
    WriteIndented = true, 
    GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(MapleMap))]
public partial class MapleMapContext : JsonSerializerContext
{
    
}