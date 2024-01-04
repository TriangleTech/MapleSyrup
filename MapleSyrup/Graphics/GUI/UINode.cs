using MapleSyrup.Core;
using MapleSyrup.State;

namespace MapleSyrup.Graphics.GUI;

public class UINode : IState
{
    protected readonly GameContext Context;
    
    public UINode(GameContext context)
    {
        Context = context;
    }
    
    
}