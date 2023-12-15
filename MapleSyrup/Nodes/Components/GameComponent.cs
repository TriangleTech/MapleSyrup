using MapleSyrup.Core;

namespace MapleSyrup.Nodes.Components;

public class GameComponent : GameObject
{
    public bool Enabled { get; set; } = true;
    
    public GameComponent(GameContext context) 
        : base(context)
    {
        
    }
}