using System.Runtime.CompilerServices;
using Client.NX;

namespace Client.Managers;

public class NxManager : IManager
{
    private readonly Dictionary<MapleFiles, NxFile> _nx;

    private readonly List<string> _fileName =
    [
        "Character",
        "Effect",
        "Etc",
        "Map",
        "Mob",
        "Npc",
        "Quest",
        "Reactor",
        "Skill",
        "Sound",
        "TamingMob",
        "UI",
    ];

    public NxManager()
    {
        _nx = new();
    }

    public void Initialize()
    {
        if (!AppConfig.IsBetaVersion)
        {
            _nx[MapleFiles.Character] = new NxFile($"{AppConfig.GameFilePath}/Character.nx", NxSaveMode.Save);
            _nx[MapleFiles.Map] = new NxFile($"{AppConfig.GameFilePath}/Map.nx", NxSaveMode.Save);
            _nx[MapleFiles.UI] = new NxFile($"{AppConfig.GameFilePath}/UI.nx", NxSaveMode.Save);
        }
        else
        {
            _nx[MapleFiles.Data] = new NxFile($"{AppConfig.GameFilePath}/Data.nx", NxSaveMode.Save);
        }
    }

    public NxFile Get(MapleFiles file)
    {
        return _nx[file];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NxNode? GetNode(string node)
    {
        if (!AppConfig.IsBetaVersion)
        {
            foreach (var (_, file) in _nx)
            {
                if (file.StringPool.ContainsKey(node))
                {
                    return file.GetNode(node);
                }
            }
        }
        else
        {
            foreach (var file in _fileName)
            {
                if (!_nx[MapleFiles.Data].StringPool.ContainsKey($"{file}/{node}")) continue;
                return _nx[MapleFiles.Data].GetNode($"{file}/{node}");
            }
        }

        return null;
    }

    public void Shutdown()
    {
        foreach (var (_, nx) in _nx)
            nx.Dispose();
        _nx.Clear();
    }
}

public enum MapleFiles
{
    Data = 0,
    Character = 1,
    Effect = 2,
    Etc = 3,
    Map = 4,
    Mo = 5,
    Npc = 6,
    Quest = 7,
    Reactor = 8,
    Skill = 9,
    Sound = 10,
    TamingMob = 11,
    UI = 12,
}