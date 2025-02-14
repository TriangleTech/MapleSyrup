namespace MapleSyrup.Scenes.Map;

public class MapObject
{
    public required string NodePath { get; init; }
    public required short ObjType { get; init; }
    public required float X { get; init; }
    public required float Y { get; init; }
    public required int Z { get; init; }
    public required int Layer { get; init; }
}