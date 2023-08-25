namespace MapleSharp.NX;

public class IntNode : Node
{
    private int value;
    
    public IntNode(string name, uint childId, uint count, NodeType nType, int data)
        : base(name, childId, count, nType)
    {
        value = data;
    }

    public int GetInt()
    {
        return value;
    }
}