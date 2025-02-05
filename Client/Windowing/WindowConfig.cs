using System.Text.Json.Serialization;

namespace Client.Windowing;

public class WindowConfig
{
    public required int Width { get; init; }
    public required int Height { get; init; }
    public required string Title { get; init; }
    public bool Fullscreen { get; init; } = false;
}