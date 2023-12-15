namespace MapleSyrup.Resources.Nx;

public interface INxNode : IDisposable
{
    public string Name { get; }
    public int FirstChildId { get; }
    public int ChildCount { get; }
    public NodeType NodeType { get; }
    public Dictionary<string, INxNode> Children { get; }
    public INxNode this[string name] { get; }
    public INxNode? CheckChildren(string name);
    public void AddChild(INxNode node);
    public void PopulateChildren(Span<INxNode> nodes);
}