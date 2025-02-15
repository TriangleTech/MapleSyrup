using System.Collections.ObjectModel;
using MapleSyrup.NX;

namespace MapleSyrup.Nx;

public class NXFactory
{
    private readonly Dictionary<MapleFile, NXFile> _files;
    private const string DefaultPath = "D:/v41";
    
    public static NXFactory Shared { get; private set; }
    
    public Dictionary<MapleFile, NXFile> Files => _files;
    
    public NXFactory()
    {
        _files = new(16)
        {
            [MapleFile.Character] = new(string.Concat(DefaultPath, "/Character.nx")),
            [MapleFile.Effect] = new(string.Concat(DefaultPath, "/Effect.nx")),
            [MapleFile.Etc] = new(string.Concat(DefaultPath, "/Etc.nx")),
            [MapleFile.Map] = new(string.Concat(DefaultPath, "/Map.nx")),
            [MapleFile.Mob] = new(string.Concat(DefaultPath, "/Mob.nx")),
            [MapleFile.Npc] = new(string.Concat(DefaultPath, "/Npc.nx")),
            [MapleFile.Quest] = new(string.Concat(DefaultPath, "/Quest.nx")),
            [MapleFile.Reactor] = new(string.Concat(DefaultPath, "/Reactor.nx")),
            [MapleFile.Skill] = new(string.Concat(DefaultPath, "/Skill.nx")),
            [MapleFile.Sound] = new(string.Concat(DefaultPath, "/Sound.nx")),
            [MapleFile.TamingMob] = new(string.Concat(DefaultPath, "/TamingMob.nx")),
            [MapleFile.UI] = new(string.Concat(DefaultPath, "/UI.nx"))
        };
        
        Shared = this;
    }

    public NXNode? GetNode(MapleFile file, string path)
    {
        return _files[file].GetNode(path);
    }

    /// <summary>
    /// Gets a node without verifying if it's the correct one. Only use this for '.img' nodes that are NOT in Map.nx EXCEPT for MAP IMGS (100000000.img is unique).
    /// Every other NX files does not have repeating node names. TODO: Verify.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="img"></param>
    /// <returns></returns>
    public NXNode? GetFastImg(MapleFile file, string img)
    {
        return _files[file].GetFastImg(img);
    }

    public NXNode? GetChildNode(MapleFile file, NXNode parent, string childName)
    {
        return _files[file].GetChildNode(parent, childName);
    }
    
    public void Shutdown()
    {
        foreach (var file in _files.Values)
        {
            file.Dispose();
        }
    }
}

public enum MapleFile
{
    Data = 0,
    Character = 1,
    Effect = 2,
    Etc = 3,
    Map = 4,
    Mob = 5,
    Npc = 6,
    Quest = 7,
    Reactor = 8,
    Skill = 9,
    Sound = 10,
    TamingMob = 11,
    UI = 12,
}