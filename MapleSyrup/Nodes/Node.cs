using MapleSyrup.Core;
using MapleSyrup.Graphics;
using MapleSyrup.Resources.Scripting;
using OpenTK.Mathematics;

namespace MapleSyrup.Nodes;

public class Node : IDisposable
{
    private string nodeName;
    private int nodeId;
    private Vector2 position;
    private int zBuffer;
    private RenderLayer renderLayer;
    private Dictionary<RenderLayer, Dictionary<int, List<Node>>> children;
    private LuaScript script;
    private Node rootNode;
    
    protected Engine Engine => Engine.Instance;
    
    public string Name
    {
        get => nodeName;
        set => nodeName = value;
    }
    
    public int ID
    {
        get => nodeId;
        set => nodeId = value;
    }
    
    public Vector2 Position
    {
        get => position;
        set => position = value;
    }

    public int Z
    {
        get => zBuffer;
        set => zBuffer = value;
    }
    
    public RenderLayer Layer
    {
        get => renderLayer;
        set => renderLayer = value;
    }

    public Node Root
    {
        get => rootNode;
        set => rootNode = value;
    }

    public Node()
    {
        Name = "Node";
        ID = 0;
        Position = Vector2.Zero;
        Z = 0;
        Layer = RenderLayer.Background;
        children = new Dictionary<RenderLayer, Dictionary<int, List<Node>>>();
        InitLayers();
    }

    private void InitLayers()
    {
        children.Add(RenderLayer.Background, new Dictionary<int, List<Node>>());
        children.Add(RenderLayer.TileObject1, new Dictionary<int, List<Node>>());
        children.Add(RenderLayer.TileObject2, new Dictionary<int, List<Node>>());
        children.Add(RenderLayer.TileObject3, new Dictionary<int, List<Node>>());
        children.Add(RenderLayer.TileObject4, new Dictionary<int, List<Node>>());
        children.Add(RenderLayer.TileObject5, new Dictionary<int, List<Node>>());
        children.Add(RenderLayer.TileObject6, new Dictionary<int, List<Node>>());
        children.Add(RenderLayer.TileObject7, new Dictionary<int, List<Node>>());
        children.Add(RenderLayer.WeatherEffect, new Dictionary<int, List<Node>>());
        children.Add(RenderLayer.Foreground, new Dictionary<int, List<Node>>());
        children.Add(RenderLayer.UI, new Dictionary<int, List<Node>>());
    }

    public virtual void Render()
    {
            foreach (var keyValuePair in children[RenderLayer.Background])
            {
                foreach (var node in keyValuePair.Value)
                {
                    node.Render();
                }
            }
            
            foreach (var keyValuePair in children[RenderLayer.TileObject1])
            {
                foreach (var node in keyValuePair.Value)
                {
                    node.Render();
                }
            }
    }

    public virtual void Update(float timeDelta)
    {
        foreach (var keyValuePair in children[RenderLayer.Background])
        {
            foreach (var node in keyValuePair.Value)
            {
                node.Update(timeDelta);
            }
        }
            
        foreach (var keyValuePair in children[RenderLayer.TileObject1])
        {
            foreach (var node in keyValuePair.Value)
            {
                node.Update(timeDelta);
            }
        }
    }

    public void AddChild(Node node)
    {
        if (node == null)
            return;
        if (!children[node.Layer].ContainsKey(node.Z))
            children[node.Layer].Add(node.Z, new List<Node>());
        children[node.Layer][node.Z].Add(node);
    }
    
    public T FindNode<T>(string name) where T : Node
    {
        foreach (var keyValuePair in children)
        {
            foreach (var keyValuePair2 in keyValuePair.Value)
            {
                foreach (var node in keyValuePair2.Value)
                {
                    if (node.Name == name)
                        return (T) node;
                }
            }
        }

        return null;
    }
    
    public Node FindNode(string name)
    {
        foreach (var keyValuePair in children)
        {
            foreach (var keyValuePair2 in keyValuePair.Value)
            {
                foreach (var node in keyValuePair2.Value)
                {
                    if (node.Name == name)
                        return node;
                }
            }
        }

        return null;
    }
    
    public void RemoveChild(Node node)
    {
        if (node == null)
            return;
        if (children[node.Layer][node.Z] == null)
            return;
        children[node.Layer][node.Z].Remove(node);
    }

    public virtual void Dispose()
    {
        
    }
}