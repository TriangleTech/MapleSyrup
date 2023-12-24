using Microsoft.Xna.Framework;

namespace MapleSyrup.ECS.Components;

public class Foothold : Component
{
    public int FootholdId;
    public int Layer;
    public Vector2 Point1;
    public Vector2 Point2;
    public int NextFoothold;
    public int PreviousFoothold;
}