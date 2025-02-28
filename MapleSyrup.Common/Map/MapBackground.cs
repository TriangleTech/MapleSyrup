namespace MapleSyrup.Common.Map;

public struct MapBackground
{
    public MapBackground()
    {
    }

    public required string NodePath { get; init; } = string.Empty;
    public required int BackgroundType { get; init; } = 0;
    public required float X { get; init; } = 0;
    public required float Y { get; init; } = 0;
    public required int Z { get; init; } = 0;
    public required int Cx { get; init; } = 0;
    public required int Cy { get; init; } = 0;
    public required int Rx { get; init; } = 0;
    public required int Ry { get; init; } = 0;
}