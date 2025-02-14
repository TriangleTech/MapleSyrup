using MapleSyrup.Networking.Handlers;
using MapleSyrup.Networking.Packets;
using MapleSyrup.Windowing;

namespace MapleSyrup.Networking;

public class PacketProcessor
{
    private readonly Dictionary<ServerToClient, IPacketHandler> _packetHandlers;
    
    public PacketProcessor()
    {
        _packetHandlers = new ()
        {
            { ServerToClient.ClientStart, new ClientStartHandler() }
        };
    }

    public void ProcessPacket(Packet packet)
    {

        try
        {
            var id = (ServerToClient)packet.PacketType;
            if (!_packetHandlers.TryGetValue(id, out var handler)) return;

            handler.HandlePacket(packet);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}