using MapleSharp.Core.Interface;
using MapleSharp.NX;

namespace MapleSharp.Resources;

public class NxSystem : ISubsystem
{
    private readonly Dictionary<string, NxFile> nxFiles = new();

    public NxFile this[string name]
    {
        get
        {
            if (nxFiles.TryGetValue(name, out var requestedNxFile))
            {
                return requestedNxFile;
            }
            
            throw new NullReferenceException($"[NxSystem] Attempted to get NxFile that does not exist. ({name})");
        }
    }

    public T ResolvePath<T>(string path) where T : Node
    {
        var splitPath = path.Split('/');
        var nxFile = this[splitPath[0]];
        var currentNode = nxFile.BaseNode;

        for (int i = 1; i < splitPath.Length; i++)
        {
            currentNode = currentNode[splitPath[i]];
        }

        return currentNode.To<T>();
    }

    public void Initialize()
    {
#if DEBUG
        Directory.GetFiles("D:/v62/", "*.nx").ToList().ForEach(x =>
        {
            var nxFile = new NxFile(x);
            Console.WriteLine($"[NxSystem] Loaded {Path.GetFileNameWithoutExtension(x)}");
            nxFiles.Add(Path.GetFileNameWithoutExtension(x).ToLower(), nxFile);
        });
#else
        Directory.GetFiles(".", "*.nx").ToList().ForEach(x =>
        {
            var nxFile = new NxFile(x);
            nxFiles.Add(Path.GetFileNameWithoutExtension(x).ToLower(), nxFile);
        });
#endif
    }

    public void Update(float timeDelta)
    {
    }

    public void Shutdown()
    {
        foreach (var nxFile in nxFiles)
        {
            nxFile.Value.Dispose();
        }
        nxFiles.Clear();
    }
}