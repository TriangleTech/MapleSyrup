namespace MapleSyrup.ECS.Components;

public class WorldInfo : Component
{
    public string WorldId;
    public bool IsTown;
    public bool CanSwim;
    public double MobRate;
    public string Bmg;
    public int ReturnMap;
    public bool HideMinimap;
    public int ForcedReturn;
    public int MoveLimit;
    public string MapMark;
    public int FieldLimit;
    public int VrTop;
    public int VrLeft;
    public int VrBottom;
    public int VrRight;
    public RectangleF Bounds;
}