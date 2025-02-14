using MapleSyrup.Networking.Packets;

namespace MapleSyrup.Networking.Handlers;

public interface IPacketHandler
{
    void HandlePacket(Packet packet);
}