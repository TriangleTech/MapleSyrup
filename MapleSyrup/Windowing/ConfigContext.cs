using System.Text.Json.Serialization;
using MapleSyrup.Scenes;

namespace MapleSyrup.Windowing;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(WindowConfig))]
internal partial class ConfigContext : JsonSerializerContext
{
    
}