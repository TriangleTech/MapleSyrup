using MapleSyrup.Core;
using MapleSyrup.Nodes;

namespace MapleSyrup.Subsystems;

public class NodeSystem : ISubsystem
{
    public GameContext Context { get; private set; }
    public void Initialize(GameContext context)
    {
        Context = context;
    }

    public void Shutdown()
    {
    }

    public T CreateNode<T>() where T : Node2D
    {
        var node = new Node2D(Context) as T;
        return (T)node;
    }
}