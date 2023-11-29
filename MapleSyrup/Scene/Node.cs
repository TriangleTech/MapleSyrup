using MapleSyrup.Core;

namespace MapleSyrup.Scene;

public class Node : IMapleObject
{
    public MapleContext Context { get; }

    public Node(MapleContext context)
    {
        Context = context;
    }
}