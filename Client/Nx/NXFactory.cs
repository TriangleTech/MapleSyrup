namespace Client.Nx;

public class NXFactory
{
    private readonly Dictionary<MapleFiles, NXFile> _files = new();
    private const string DefaultPath = "D:/v41";
    
    public static NXFactory Shared { get; private set; }
    
    public Dictionary<MapleFiles, NXFile> Files => _files;
    
    public NXFactory()
    {
        _files[MapleFiles.Character] = new NXFile(string.Concat(DefaultPath, "/Character.nx"));
        _files[MapleFiles.Effect] = new NXFile(string.Concat(DefaultPath, "/Effect.nx"));
        _files[MapleFiles.Etc] = new NXFile(string.Concat(DefaultPath, "/Etc.nx"));
        _files[MapleFiles.Map] = new NXFile(string.Concat(DefaultPath, "/Map.nx"));
        _files[MapleFiles.Mob] = new NXFile(string.Concat(DefaultPath, "/Mob.nx"));
        _files[MapleFiles.Npc] = new NXFile(string.Concat(DefaultPath, "/Npc.nx"));
        _files[MapleFiles.Quest] = new NXFile(string.Concat(DefaultPath, "/Quest.nx"));
        _files[MapleFiles.Reactor] = new NXFile(string.Concat(DefaultPath, "/Reactor.nx"));
        _files[MapleFiles.Skill] = new NXFile(string.Concat(DefaultPath, "/Skill.nx"));
        _files[MapleFiles.Sound] = new NXFile(string.Concat(DefaultPath, "/Sound.nx"));
        _files[MapleFiles.TamingMob] = new NXFile(string.Concat(DefaultPath, "/TamingMob.nx"));
        _files[MapleFiles.UI] = new NXFile(string.Concat(DefaultPath, "/UI.nx"));
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