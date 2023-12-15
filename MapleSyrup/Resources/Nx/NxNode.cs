using System.Runtime.CompilerServices;

namespace MapleSyrup.Resources.Nx;

/// <summary>
/// Container for the Node Type and is used by the other node types
/// </summary>
public class NxNode : INxNode
{
    private string nodeName;
    private int firstChildId;
    private int childCount;
    private NodeType nodeType;
    private Dictionary<string, INxNode> children;

    /// <summary>
    /// Initializes the Node 
    /// </summary>
    /// <param name="name">Name of the node</param>
    /// <param name="childId">ID of the first child</param>
    /// <param name="count">Number of child nodes</param>
    /// <param name="nType">Type of the Node</param>
    public NxNode(string name, int childId, int count, NodeType nType)
    {
        nodeName = name;
        firstChildId = childId;
        childCount = count;
        nodeType = nType;
        children = new Dictionary<string, INxNode>(childCount);
    }
    
    /// <summary>
    /// Allows you to increment through the Node using the node name
    /// </summary>
    /// <param name="name">Name of the Node</param>
    /// <exception cref="Exception">Throws an exception if the Node doesn't exist within the Node or its children</exception>
    public INxNode this[string name]
    {
        get
        {
            if (children.TryGetValue(name, out INxNode? child))
                return child;
            
            //var innerChild = CheckChildren(name);
            //if (innerChild == null)
            //    throw new Exception(
            //        "[Node] Attempted to get non-existent child. Not located in first or second layer children.");
            //return innerChild;
            throw new NullReferenceException();
        }
    }
    
    /// <summary>
    /// Checks the Children of the Node for any Node that matches what you're looking for.
    /// This is a WildCard method. It grabs to FIRST instance of the node you're looking for.
    /// </summary>
    /// <param name="name">Name of the node we're looking for</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public INxNode? CheckChildren(string name)
    {
        foreach (NxNode child in children.Values)
        {
            if (child.HasChild(name))
                return child[name];
        }

        return null;
    }

    public void AddChild(INxNode node)
    {
        throw new NotImplementedException();
    }

    public void PopulateChildren(Span<INxNode> nodes)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Attempts to get the child from the node, if it doesn't exist within this child, it checks the child's child.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public INxNode GetChild(string name)
    {
        if (children.TryGetValue(name, out INxNode? child))
            return child;
        throw new NullReferenceException();
    }

    /// <summary>
    /// Checks if the node exists within the top-level children
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool HasChild(string name)
    {
        return children.ContainsKey(name);
    }

    /// <summary>
    /// WARNING: ONLY USED WHEN PARSING THE FILE!!!.
    /// Links the children of the node and populates the directory.
    /// </summary>
    /// <param name="data">Data containing all the nodes</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PopulateChildren(Span<NxNode> data)
    {
        var maxId = childCount + firstChildId;

        for (int i = firstChildId; i < maxId; i++)
        {
            AddChild(data[i]);
        }
    }

    /// <summary>
    /// WARNING: ONLY USED WHEN PARSING THE FILE!!!
    /// Adds the child of the node to the directory.
    /// </summary>
    /// <param name="node"></param>
    private void AddChild(NxNode node)
    {
        children.Add(node.nodeName, node);
    }

    /// <summary>
    /// Allows easy conversion from NxNode to any other Node Type.
    /// Do be aware that if the Node is not that type it will fault out.
    /// </summary>
    /// <typeparam name="T">Type to convert the node to.</typeparam>
    /// <returns></returns>
    public T To<T>() where T : NxNode
    {
        return (T)this;
    }

    public object GetData<T>() where T : NxNode
    {
        switch (typeof(T))
        {
            case not null when typeof(T) == typeof(NxAudioNode):
                return new NotImplementedException();
            case not null when typeof(T) == typeof(NxDoubleNode):
                return ((NxDoubleNode)this).GetDouble();
            case not null when typeof(T) == typeof(NxIntNode):
                return ((NxIntNode)this).GetInt();
            case not null when typeof(T) == typeof(NxStringNode):
                return ((NxStringNode)this).GetString();
            case not null when typeof(T) == typeof(NxVectorNode):
                return ((NxVectorNode)this).GetVector();
            default:
                throw new ArgumentOutOfRangeException(nameof(T), typeof(T), null);
        }
    }

    /// <summary>
    /// Name of the current node.
    /// </summary>
    public string Name => nodeName;

    public int FirstChildId => firstChildId;

    public Dictionary<string, INxNode> Children => children;

    /// <summary>
    /// Number of Child Nodes within the Node.
    /// Note: Don't change this to Children.Count...it will not work
    /// </summary>
    public int ChildCount => (int)childCount;
    
    /// <summary>
    /// Type of the Current Node.
    /// </summary>
    public NodeType NodeType => nodeType;

    public void Dispose()
    {
        
    }
}