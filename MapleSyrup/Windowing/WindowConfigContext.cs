using System.Text.Json.Serialization;

namespace MapleSyrup.Windowing;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(WindowConfig))]
internal partial class WindowConfigContext : JsonSerializerContext
{
    
}