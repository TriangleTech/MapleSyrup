namespace MapleSyrup.Scenes.Map;

public class MapBackground
{
    public required string NodePath { get; init; }
    public required int BackgroundType { get; init; }
    public required float X { get; init; }
    public required float Y { get; init; }
    public required int Z { get; init; }
    public int Cx { get; init; } = 0;
    public int Cy { get; init; } = 0;
    public int Rx { get; init; } = 0;
    public int Ry { get; init; } = 0;
}