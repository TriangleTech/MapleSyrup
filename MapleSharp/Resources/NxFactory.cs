using MapleSharp.NX;
using MapleSharp.Services;

namespace MapleSharp.Resources;

public class NxFactory
{
    private readonly Dictionary<string, NxFile> nxFiles;
    
    public NxFactory()
    {
        nxFiles = new ();
    }

    public NxFile this[string name]
    {
        get
        {
            if (nxFiles.TryGetValue(name.ToLower(), out var requestedNxFile))
            {
                return requestedNxFile;
            }
#if DEBUG
            var nxFile = new NxFile(string.Concat($"D:/v62/{name}", ".nx"));
#else
            var nxFile = new NxFile(string.Concat(name.ToLower(), ".nx"));
#endif
            nxFiles.Add(name.ToLower(), nxFile);
            return nxFile;
        }
    }
}