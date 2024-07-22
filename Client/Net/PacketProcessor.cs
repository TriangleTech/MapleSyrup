namespace Client.Net;

public class PacketProcessor
{
    private readonly Queue<PacketResponse> _packetResponses;
    private readonly Queue<PacketRequest> _packetRequests;
    private readonly object _lock;
    
    public PacketProcessor()
    {
        _packetRequests = new Queue<PacketRequest>();
        _packetResponses = new Queue<PacketResponse>();
        _lock = new();
    }

    public void ProcessResponse(PacketResponse response)
    {
        switch (response.Opcode)
        {
            
        }
    }
}