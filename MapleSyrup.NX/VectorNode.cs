namespace MapleSyrup.NX;

public class VectorNode : Node
{
    private int x, y;
    public VectorNode(string name, uint childId, uint count, NodeType nType, int pointOne, int pointTwo)
        : base(name, childId, count, nType)
    {
        x = pointOne;
        y = pointTwo;
    }

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return y;
    }
}