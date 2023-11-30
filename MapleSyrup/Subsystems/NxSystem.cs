using MapleSyrup.Core;
using MapleSyrup.Resources.Nx;

namespace MapleSyrup.Subsystems;

public class NxSystem : ISubsystem
{
    public GameContext Context { get; private set; }
    public Dictionary<string, NxFile> nxFiles = new();
    
    public void Initialize(GameContext context)
    {
        Context = context;

        //foreach (var file in Directory.GetFiles("/home/beray/mapledev/v62/v62_nx"))
       // {
       //     nxFiles[Path.GetFileNameWithoutExtension(file)] = new NxFile(file);
        //}
        nxFiles["Map"] = new NxFile("/home/beray/mapledev/v62/v62_nx/Map.nx");
    }

    public void Shutdown()
    {
        foreach (var file in nxFiles.Values)
        {
            file.Dispose();
        }
    }

    public NxFile Get(string fileName)
    {
        return nxFiles[fileName] ?? throw new NullReferenceException();
    }
}