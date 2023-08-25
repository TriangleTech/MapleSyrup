using MapleSharp.NX;

namespace MapleSharp.Resources;

public class NxFactory
{
    private Dictionary<string, NxFile> nxFiles;
    private static NxFactory? instance;
    
    public static NxFactory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new NxFactory();
            }

            return instance;
        }
    }
    
    public NxFactory()
    {
        nxFiles = new ();
        instance = this;
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