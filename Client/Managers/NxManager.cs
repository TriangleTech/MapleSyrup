using Client.NX;

namespace Client.Managers;

public class NxManager : IManager
{
    private readonly Dictionary<MapleFiles, NxFile> _nx;

    public NxManager()
    {
        _nx = new();
    }

    public void Initialize()
    {
        _nx[MapleFiles.Character] = new NxFile("D:/v62/Character.nx", NxSaveMode.None) ;
        _nx[MapleFiles.Map] = new NxFile("D:/v62/Map.nx", NxSaveMode.None) ;
        _nx[MapleFiles.UI] = new NxFile("D:/v62/UI.nx", NxSaveMode.None) ;
        //_nx[MapleFiles.Character] = new NxFile("/ext/dev/maple_dev/v62/Character.nx", NxSaveMode.None) ;
        //_nx[MapleFiles.Map] = new NxFile("/ext/dev/maple_dev/v62/Map.nx", NxSaveMode.None) ;
        //_nx[MapleFiles.UI] = new NxFile("/ext/dev/maple_dev/v62/UI.nx", NxSaveMode.None) ;
    }

    public NxFile Get(MapleFiles file)
    {
        return _nx[file];
    }

    public void Shutdown()
    {
        _nx.Clear();
    }
}

public enum MapleFiles
{
    Character,
    Effect,
    Etc,
    Map,
    Mob,
    Npc,
    Quest,
    Reactor,
    Skill,
    Sound,
    TamingMob,
    UI,
}