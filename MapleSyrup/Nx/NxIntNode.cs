namespace MapleSyrup.Nx;

public class NxIntNode : NxNode
{
    private int value;
    
    public NxIntNode(string name, int childId, int count, NodeType nType, int data)
        : base(name, childId, count, nType)
    {
        value = data;
    }

    public int GetInt()
    {
        return value;
    }
}