namespace MapleSharp.NX;

public class StringNode : Node
{
    private string value;
    public StringNode(string name, uint childId, uint count, NodeType nType, string data)
        : base(name, childId, count, nType)
    {
        value = data;
    }

    public string GetString()
    {
        if (value == string.Empty)
            throw new Exception("[NxStringNode] Attempted to retrieve empty string value.");
        
        return value;
    }
}