using MapleSyrup.Core;
using OpenTK.Mathematics;

namespace MapleSyrup.Nodes;

public class Node : IDisposable
{
    private readonly List<Node> children = new();
    private string name = "";
    private int nodeId = 0;
    private Node parent = null;
    private Vector2 position = Vector2.Zero;
    private Vector2 origin = Vector2.Zero;
    private Vector2 size = Vector2.Zero;
    private float zOrder = 0.0f;
    private float rotation = 0.0f;
    private float alpha = 1.0f;
    private Vector3 color = Vector3.One;
    private Matrix4 transform = Matrix4.Identity;
    protected Engine Engine => Engine.Instance;
    
    public string Name
    {
        get => name;
        set => name = value;
    }
    
    public int ID
    {
        get => nodeId;
        set => nodeId = value;
    }
    
    public Node Parent
    {
        get => parent;
        set => parent = value;
    }
    
    public Vector2 Position
    {
        get => position;
        set => position = value;
    }
    
    public Vector2 Origin
    {
        get => origin;
        set => origin = value;
    }
    
    public Vector2 Size
    {
        get => size;
        set => size = value;
    }
    
    public float ZOrder
    {
        get => zOrder;
        set => zOrder = value;
    }
    
    public float Rotation
    {
        get => rotation;
        set => rotation = value;
    }
    
    public float Alpha
    {
        get => alpha;
        set => alpha = value;
    }
    
    public Vector3 Color
    {
        get => color;
        set => color = value;
    }
    
    public Matrix4 Transform
    {
        get => transform;
        set => transform = value;
    }

    public void AddChild(Node child)
    {
        children.Add(child);
    }
    
    public void RemoveChild(Node child)
    {
        children.Remove(child);
    }
    
    public void RemoveChild(int index)
    {
        children.RemoveAt(index);
    }
    
    public void RemoveAllChildren()
    {
        children.Clear();
    }
    
    public T GetChild<T>() where T : Node
    {
        return children.OfType<T>().FirstOrDefault();
    }
    
    public IEnumerable<T> GetChildren<T>() where T : Node
    {
        return children.OfType<T>();
    }
    
    public IEnumerable<Node> GetChildren()
    {
        return children;
    }
    
    public virtual void Update(float timeDelta)
    {
        foreach (var child in children)
            child.Update(timeDelta);
    }
    
    public virtual void Render()
    {
        foreach (var child in children)
            child.Render();
    }
    
    public virtual void Dispose()
    {
        foreach (var child in children)
            child.Dispose();
        GC.SuppressFinalize(this);
    }
}