using MapleSyrup.Core.Interface;
using MapleSyrup.NX;

namespace MapleSyrup.Resources;

public unsafe class NxSystem : ISubsystem
{
    private NxFile character, map;
    
    public NxFile Character => character;
    public NxFile Map => map;
    
    public NxSystem()
    {
        character = new NxFile("D:/v62/Character.nx");
        map = new NxFile("D:/v62/Map.nx");
    }

    public NxNode ResolvePath(string path)
    {
        var splitPath = path.Split('/');
        NxFile nxFile = null;

        switch (splitPath[0])
        {
            case "character":
                nxFile = character;
                break;
            case "map":
                nxFile = map;
                break;
        }

        var currentNode = nxFile.Root;
        NxNode nextNode = null;

        for (int i = 1; i < splitPath.Length; i++)
        {
            Console.WriteLine($"Current node: {currentNode.Name}");
            nextNode = currentNode[splitPath[i]];
            currentNode = nextNode;
        }

        return currentNode;
    }

    public void Initialize()
    {
        
    }

    public void Update(float timeDelta)
    {
    }

    public void Shutdown()
    {
        character.Release();
        map.Release();
    }
}