using System.Numerics;
using Client.NX;
using Raylib_CsLo;

namespace Client.Scene;

public struct WorldInfo
{
    private readonly NxNode? _node;
    public int Version { get; }
    public int Cloud { get; }
    public int Town { get; }
    public float MobRate { get; }
    public string Bgm { get; }
    public int ReturnMap { get; }
    public string MapDesc { get; }
    public int HideMinimap { get; }
    public int ForcedReturn { get; }
    public int MoveLimit { get; }
    public string MapMark { get; }
    public int Swim { get; }
    public int FieldLimit { get; }
    public int VRTop { get; }
    public int VRLeft { get; }
    public int VRBottom { get; }
    public int VRRight { get; }
    public int Fly { get; }
    public int NoMapCmd { get; }
    public string OnFirstUserEnter { get; }
    public string OnUserEnter { get; }
    
    public WorldInfo(NxNode info)
    {
        _node = info;
        Version = _node?.GetInt("version") ?? 0;
        Cloud = _node?.GetInt("cloud") ?? 0;
        Town = _node?.GetInt("town") ?? 0;
        MobRate = (float)(_node?.GetDouble("mobRate") ?? 0);
        Bgm = _node?.GetString("bgm") ?? "";
        ReturnMap = _node?.GetInt("returnMap") ?? 0;
        MapDesc = _node?.GetString("mapDesc") ?? "";
        HideMinimap = _node?.GetInt("hideMinimap") ?? 0;
        ForcedReturn = _node?.GetInt("forcedReturn") ?? 0;
        MoveLimit = _node?.GetInt("moveLimit") ?? 0;
        MapMark = _node?.GetString("mapMark") ?? "";
        Swim = _node?.GetInt("swim") ?? 0;
        FieldLimit = _node?.GetInt("fieldLimit") ?? 0;
        VRTop = _node?.GetInt("VRTop") ?? 0;
        VRLeft = _node?.GetInt("VRLeft") ?? 0;
        VRBottom = _node?.GetInt("VRBottom") ?? 0;
        VRRight = _node?.GetInt("VRRight") ?? 0;
        Fly = _node?.GetInt("fly") ?? 0;
        NoMapCmd = _node?.GetInt("noMapCmd") ?? 0;
        OnFirstUserEnter = _node?.GetString("onFirstUserEnter") ?? "";
        OnUserEnter = _node?.GetString("onUserEnter") ?? "";
    }
}