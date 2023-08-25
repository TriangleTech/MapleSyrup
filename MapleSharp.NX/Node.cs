using System.Runtime.CompilerServices;

namespace MapleSharp.NX;

public class Node
{
    private string nodeName;
    private uint firstChildId;
    private uint childCount;
    private NodeType nodeType;
    private Dictionary<string, Node> children;

    /// <summary>
    /// Initializes the Node 
    /// </summary>
    /// <param name="name">Name of the node</param>
    /// <param name="childId">ID of the first child</param>
    /// <param name="count">Number of child nodes</param>
    /// <param name="nType">Type of the Node</param>
    public Node(string name, uint childId, uint count, NodeType nType)
    {
        nodeName = name;
        firstChildId = childId;
        childCount = count;
        nodeType = nType;
        children = new Dictionary<string, Node>((int)childCount);
    }
    
    /// <summary>
    /// Allows you to increment through the Node using the node name
    /// </summary>
    /// <param name="name">Name of the Node</param>
    /// <exception cref="Exception">Throws an exception if the Node doesn't exist within the Node or its children</exception>
    public Node this[string name]
    {
        get
        {
            if (children.TryGetValue(name, out Node? child))
                return child;
            if (children.Count > 1)
            {
                var innerChild = CheckChildren(name);
                if (innerChild != null)
                    return innerChild;
            }
            throw new Exception("[Node] Attempted to get non-existent child. Not located in first or second layer children.");
        }
    }
    
    public Node ResolvePath(string path)
    {
        var split = path.Split('/');
        var node = this;
        foreach (var s in split)
        {
            node = node[s];
        }

        return node;
    }
    
    /// <summary>
    /// Checks the Children of the Node for any Node that matches what you're looking for.
    /// This is a WildCard method. It grabs to FIRST instance of the node you're looking for.
    /// </summary>
    /// <param name="name">Name of the node we're looking for</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Node? CheckChildren(string name)
    {
        foreach (Node child in children.Values)
        {
            if (child.HasChild(name))
                return child[name];
        }

        return null;
    }

    /// <summary>
    /// Attempts to get the child from the node, if it doesn't exist within this child, it checks the child's child.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public Node GetChild(string name)
    {
        if (children.TryGetValue(name, out Node? child))
            return child;
        
        var innerChild = CheckChildren(name);
        if (innerChild == null)
            throw new Exception(
                "[Node] Attempted to get non-existent child. Not located in first or second layer children.");
        return innerChild;
    }

    /// <summary>
    /// Checks if the node exists within the top-level children
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool HasChild(string name)
    {
        return children.TryGetValue(name, out Node? child);
    }

    /// <summary>
    /// WARNING: ONLY USED WHEN PARSING THE FILE!!!.
    /// Links the children of the node and populates the directory.
    /// </summary>
    /// <param name="data">Data containing all the nodes</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PopulateChildren(Node[] data)
    {
        var maxId = childCount + firstChildId;

        for (uint i = firstChildId; i < maxId; i++)
        {
            AddChild(data[i]);
        }
    }

    /// <summary>
    /// WARNING: ONLY USED WHEN PARSING THE FILE!!!
    /// Adds the child of the node to the directory.
    /// </summary>
    /// <param name="node"></param>
    private void AddChild(Node node)
    {
        children.Add(node.nodeName, node);
    }

    /// <summary>
    /// Allows easy conversion from Node to any other Node Type.
    /// Do be aware that if the Node is not that type it will fault out.
    /// </summary>
    /// <typeparam name="T">Type to convert the node to.</typeparam>
    /// <returns></returns>
    public T To<T>() where T : Node
    {
        return (T)this;
    }

    /// <summary>
    /// Name of the current node.
    /// </summary>
    public string Name => nodeName;

    public Dictionary<string, Node> Children => children;

    /// <summary>
    /// Number of Child Nodes within the Node.
    /// Note: Don't change this to Children.Count...it will not work
    /// </summary>
    public int ChildCount => (int)childCount;
    
    /// <summary>
    /// Type of the Current Node.
    /// </summary>
    public NodeType NodeType => nodeType;
}