namespace MapleSyrup.Resources.Nx;

public class NxStringNode : NxNode
{
    private string value;
    public NxStringNode(string name, int childId, int count, NodeType nType, string data)
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