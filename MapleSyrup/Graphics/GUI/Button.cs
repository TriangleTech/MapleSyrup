using MapleSyrup.Core;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Graphics.GUI;

public class Button : UINode
{
    private Dictionary<ButtonState, Texture2D> buttonStates = new();

    public Button(GameContext context)
        : base(context)
    {
        
    }
}