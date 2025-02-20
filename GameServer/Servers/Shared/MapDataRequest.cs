using GameServer.Client;
using GameServer.Servers.Interfaces;
using MapleSyrup.Common.Packets;

namespace GameServer.Servers.Shared;

public class MapDataRequest : IPacketHandler
{
    public void HandlePacket(GameClient client, Packet packet)
    {
        var type = packet.ReadByte();
        var mapId = packet.ReadString();
        
        
    }
}