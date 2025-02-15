using MapleSyrup.Common.Packets;

namespace MapleSyrup.Networking.Handlers;

public interface IPacketHandler
{
    void HandlePacket(Packet packet);
}