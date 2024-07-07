using MSClient.NX;

namespace MSClient.Managers;

public class NxManager : IManager
{
    private readonly Dictionary<MapleFiles, NxFile> _nx;

    public NxManager()
    {
        _nx = new();
    }

    public void Initialize()
    {
        _nx[MapleFiles.Map] = new NxFile("/ext/dev/maple_dev/v62_nx/Map.nx", NxSaveMode.None) ;
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