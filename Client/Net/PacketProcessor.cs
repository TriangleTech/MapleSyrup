using System.Collections.Concurrent;
using Client.Managers;

namespace Client.Net;

public class PacketProcessor
{
    private readonly ConcurrentQueue<InPacket> _packetResponses;
    private readonly object _lock;
    
    public PacketProcessor()
    {
        _packetResponses = new ();
        _lock = new();
    }

    public void Queue(InPacket packet)
    {
        lock (_lock)
        {
            _packetResponses.Enqueue(packet);
        }
    }

    public void ProcessPacketResponses()
    {
        lock (_lock)
        {
            while (_packetResponses.Count > 0)
            {
                _packetResponses.TryDequeue(out var response);
                if (response == null) continue;
                var actor = ServiceLocator.Get<ActorManager>();
                var world = ServiceLocator.Get<WorldManager>();
                Console.WriteLine(
                    $"Processing Packet: {response.Opcode} Data: {BitConverter.ToString(response.Data)}");

                world.ProcessPackets(response);
            }
        }
    }
}