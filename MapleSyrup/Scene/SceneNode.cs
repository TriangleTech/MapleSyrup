using MapleSyrup.Nodes;

namespace MapleSyrup.Scene;

public class SceneNode : Node
{
    public SceneNode()
    {
        ID = 0;
    }
    
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
    }
    
    public override void Render()
    {
        base.Render();
    }
    
    public override void Dispose()
    {
        base.Dispose();
    }
}