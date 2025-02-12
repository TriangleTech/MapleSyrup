using System.Text.Json.Serialization;

namespace Client.Windowing;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(WindowConfig))]
internal partial class ConfigContext : JsonSerializerContext
{
    
}