using GameServer.Client;
using MapleSyrup.Common.Packets;

namespace GameServer.Servers.Channel;

public class ChannelServer : NetworkServer
{
    public ChannelServer(int port)
        : base($"Channel {port - 7575 + 1} Server", port)
    {
        
    }

    public override void ProcessPacket(GameClient client, Packet packet)
    {
        
    }
}