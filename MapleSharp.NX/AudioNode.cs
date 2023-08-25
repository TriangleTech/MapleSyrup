namespace MapleSharp.NX;

public class AudioNode : Node
{
    public AudioNode(string name, uint childId, uint count, NodeType nType)
        : base(name, childId, count, nType)
    {
        
    }

    public void GetAudio()
    {
        throw new NotImplementedException();
    }
}