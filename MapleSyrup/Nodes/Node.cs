using MapleSyrup.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace MapleSyrup.Nodes;

public class Node : GameObject
{
    public Vector2 Position;
    public Vector2 Origin;
    public Vector2 Scale;
    public float Rotation;
    public SpriteEffects Flipped;
    public Color Color;
    public readonly List<Node> Children = new();
    public readonly List<object> Components = new();
    
    public Node(GameContext context) 
        : base(context)
    {
        
    }
    
    public Node CreateChild()
    {
        var child = new Node(Context);
        Children.Add(child);
        return child;
    }
    
    public T CreateChild<T>() where T : Node, new()
    {
        var child = new T();
        Children.Add(child);
        return child;
    }
    
    public void AddChild(Node child)
    {
        if (child == null)
            return;
        Children.Add(child);
    }
    
    public void RemoveChild(Node child)
    {
        if (child == null)
            return;
        Children.Remove(child);
    }
    
    public void AddComponent(object component)
    {
        if (component == null)
            return;
        Components.Add(component);
    }
    
    public void RemoveComponent(object component)
    {
        if (component == null)
            return;
        Components.Remove(component);
    }
    
    public void RemoveAllComponents()
    {
        Components.Clear();
    }
    
    public void RemoveAllChildren()
    {
        Children.Clear();
    }
}