using System.Numerics;
using System.Text.Json.Serialization;

namespace MapleSyrup.Common.Map;

public struct MapFoothold
{
   public required int Layer { get; init; }
   public required int X1 { get; init; }
   public required int Y1 { get; init; }
   public required int X2 { get; init; }
   public required int Y2 { get; init; }
}