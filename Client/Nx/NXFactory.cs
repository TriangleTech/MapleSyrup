using System.Collections.ObjectModel;

namespace Client.Nx;

public class NXFactory
{
    private readonly Dictionary<MapleFiles, NXFile> _files;
    private const string DefaultPath = "D:/v41";
    
    public static NXFactory Shared { get; private set; }
    
    public Dictionary<MapleFiles, NXFile> Files => _files;
    
    public NXFactory()
    {
        _files = new(16)
        {
            [MapleFiles.Character] = new(string.Concat(DefaultPath, "/Character.nx")),
            [MapleFiles.Effect] = new(string.Concat(DefaultPath, "/Effect.nx")),
            [MapleFiles.Etc] = new(string.Concat(DefaultPath, "/Etc.nx")),
            [MapleFiles.Map] = new(string.Concat(DefaultPath, "/Map.nx")),
            [MapleFiles.Mob] = new(string.Concat(DefaultPath, "/Mob.nx")),
            [MapleFiles.Npc] = new(string.Concat(DefaultPath, "/Npc.nx")),
            [MapleFiles.Quest] = new(string.Concat(DefaultPath, "/Quest.nx")),
            [MapleFiles.Reactor] = new(string.Concat(DefaultPath, "/Reactor.nx")),
            [MapleFiles.Skill] = new(string.Concat(DefaultPath, "/Skill.nx")),
            [MapleFiles.Sound] = new(string.Concat(DefaultPath, "/Sound.nx")),
            [MapleFiles.TamingMob] = new(string.Concat(DefaultPath, "/TamingMob.nx")),
            [MapleFiles.UI] = new(string.Concat(DefaultPath, "/UI.nx"))
        };
        
        Shared = this;
    }

    public NXNode? GetNode(MapleFiles file, string path)
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
    public NXNode? GetFastImg(MapleFiles file, string img)
    {
        return _files[file].GetFastImg(img);
    }

    public NXNode? GetChildNode(MapleFiles file, NXNode parent, string childName)
    {
        return _files[file].GetChildNode(parent, childName);
    }

    public ReadOnlyDictionary<string, NXNode> GetChildren(MapleFiles file, NXNode parent)
    {
        return _files[file].GetChildren(parent);
    }

    public Span<string> GetChildrenNames(MapleFiles file, NXNode parent)
    {
        return _files[file].GetChildrenNames(parent);
    }

    public bool HasNode(MapleFiles file, NXNode parent, string nodeName)
    {
        return _files[file].HasNode(parent, nodeName);
    }
    
    public void Shutdown()
    {
        foreach (var file in _files.Values)
        {
            file.Dispose();
        }
    }
}

public enum MapleFiles
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