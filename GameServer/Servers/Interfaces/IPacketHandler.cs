using GameServer.Client;
using MapleSyrup.Common.Packets;

namespace GameServer.Servers.Interfaces;

public interface IPacketHandler
{
    void HandlePacket(GameClient client, Packet packet);
}