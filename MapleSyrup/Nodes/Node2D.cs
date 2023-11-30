using MapleSyrup.Core;
using MapleSyrup.Gameplay.Map;
using Microsoft.Xna.Framework;

namespace MapleSyrup.Nodes;

public class Node2D : IGameObject
{
    private List<object> components;
    public Vector2 Position = Vector2.Zero;
    public Vector2 Origin = Vector2.One;
    public Vector2 Scale = Vector2.One;
    public float Rotation = 0f;
    public RenderLayer Layer = RenderLayer.Background;
    
    public GameContext Context { get; }
    
    public Node2D(GameContext context)
    {
        Context = context;
        components = new();
    }

    public void AddComponent<T>()
    {
        if (components.Any(x => x is T))
            return;
        
        var component = Activator.CreateInstance<T>();
        components.Add(component);
    }

    public T GetComponent<T>()
    {
        return (T)components.Find(x => x is T);
    }
}