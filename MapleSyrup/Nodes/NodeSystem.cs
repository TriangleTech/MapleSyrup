using MapleSyrup.Core.Interface;
using MapleSyrup.Graphics;

namespace MapleSyrup.Nodes;

public class NodeSystem : ISubsystem
{
    private int nextId = 1; // 0 is reserved for the root node
    private Queue<int> recycledIds;
    
    public void Initialize()
    {
        recycledIds = new ();
    }

    public void Update(float timeDelta)
    {
    }

    public void Shutdown()
    {
    }
    
    public T CreateNode<T>() where T : Node, new()
    {
        var node = new T();
        node.ID = GetNextId();
        return node;
    }
    
    public SpriteNode CreateSpriteNode(Image image)
    {
        var node = new SpriteNode(image);
        node.ID = GetNextId();
        return node;
    }
    
    public void DestroyNode(Node node)
    {
        node.Dispose();
    }

    public int GetNextId()
    {
        int id = 0;
        if (recycledIds.Count > 0)
            id = recycledIds.Dequeue();
        else
            id = nextId++;
        return id;
    }
}