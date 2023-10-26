using MapleSyrup.Core.Interface;
using MapleSyrup.Nodes;

namespace MapleSyrup.Scene;

public class SceneSystem : ISubsystem
{
    private Node root;
    private readonly Queue<int> recycledIds;
    private int nextId;

    public SceneSystem(Node scene)
    {
        root = scene;
        root.Name = "Root";
        recycledIds = new Queue<int>();
        nextId = 0;
    }

    public void Initialize()
    {
        root.ID = GetNextId();
    }
    
    public void Render()
    {
        root.Render();
    }

    public void Update(float timeDelta)
    {
        root.Update(timeDelta);
    }

    public void Shutdown()
    {
    }

    public void AddChild(Node node)
    {
        if (node == null)
            throw new ArgumentNullException($"[SceneSystem] Cannot add null node to scene {root.Name}.");
        node.Root = root;
        node.ID = GetNextId();
        root.AddChild(node);
    }
    
    public void RemoveNode(string name)
    {
        var node = root.FindNode(name);
        if (node == null)
            throw new ArgumentNullException($"[SceneSystem] Cannot remove null node from scene {root.Name}.");
        root.RemoveChild(node);
        recycledIds.Enqueue(node.ID);
    }
    
    private int GetNextId()
    {
        if (recycledIds.Count > 0)
        {
            return recycledIds.Dequeue();
        }
        else
        {
            return nextId++;
        }
    }
}