using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MapleSyrup.Gameplay.World;

public class WorldInfo
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
    public Rectangle Bounds;
}