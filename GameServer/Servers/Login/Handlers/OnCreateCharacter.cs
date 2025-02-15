using GameServer.Client;
using GameServer.Servers.Interfaces;
using MapleSyrup.Common.Packets;

namespace GameServer.Servers.Login.Handlers;

public record OnCreateCharacter : IPacketHandler
{
    public void HandlePacket(GameClient client, Packet packet)
    {
        
    }
}