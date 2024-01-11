namespace MapleSyrup.Nx;

public class NxDoubleNode : NxNode
{
    private double value;
    public NxDoubleNode(string name, int childId, int count, NodeType nType, long data)
        : base(name, childId, count, nType)
    {
        value = data;
    }

    public double GetDouble()
    {
        return value;
    }
}