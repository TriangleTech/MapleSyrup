using MapleSyrup.Core;

namespace MapleSyrup.Graphics.GUI;

public class UIPanel 
{
    private readonly GameContext Context;
    private List<UINode> nodes;
    
    
    public UIPanel(GameContext context)
    {
        Context = context;
        nodes = new();
    }

    public void AddNode(UINode node)
    {
        
    }

    public void RemoveNode(UINode node)
    {
        
    }

    public void UpdateState()
    {
        
    }
}