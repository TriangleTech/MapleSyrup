namespace MapleSyrup.Nx;

/// <summary>
/// Container for the Audio Node TODO:Implement Audio Support
/// </summary>
public class NxAudioNode : NxNode
{
    public NxAudioNode(string name, int childId, int count, NodeType nType)
        : base(name, childId, count, nType)
    {
        
    }

    public void GetAudio()
    {
        throw new NotImplementedException();
    }
}