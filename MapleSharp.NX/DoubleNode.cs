namespace MapleSharp.NX;

public class DoubleNode : Node
{
    private double value;
    public DoubleNode(string name, uint childId, uint count, NodeType nType, long data)
        : base(name, childId, count, nType)
    {
        value = data;
    }

    public double GetDouble()
    {
        return value;
    }
}