using MapleSyrup.Core.Interface;
using MapleSyrup.Nodes;

namespace MapleSyrup.Scene;

public class SceneSystem : ISubsystem
{
    private SceneNode root;

    public SceneSystem(SceneNode scene)
    {
        root = new SceneNode();
    }
    
    public void Initialize()
    {
    }

    public void Update(float timeDelta)
    {
        root.Update(timeDelta);
    }

    public void Shutdown()
    {
        root.Dispose();
    }
    
    public SceneNode CreateSceneNode()
    {
        root = new SceneNode();
        return root;
    }
    
    public void DestroySceneNode(SceneNode node)
    {
        node.Dispose();
    }
    
    public SceneNode GetRootNode()
    {
        return root;
    }
    
    public void SetRootNode(SceneNode node)
    {
        root = node;
    }
    
    public void Render()
    {
        root.Render();
    }
    
    public void Dispose()
    {
        root.Dispose();
    }

    public void AddChild(Node child)
    {
        if (child == null)
            return;
        root.AddChild(child);
    }
    
    public void RemoveChild(Node child)
    {
        if (child == null)
            return;
        root.RemoveChild(child);
    }
}