namespace MapGenerator;

public struct MapTile
{
    public required string NodePath { get; init; }
    public required short TileType { get; init; }
    public required float X { get; init; }
    public required float Y { get; init; }
    public required int Z { get; init; }
    public required int Layer { get; init; }
}