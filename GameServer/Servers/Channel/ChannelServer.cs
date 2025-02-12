using GameServer.Client;
using GameServer.Servers.Packets;

namespace GameServer.Servers.Channel;

public class ChannelServer : NetworkServer
{
    public ChannelServer(int port)
        : base(port)
    {
        ServerName = $"Channel {port - 7575 + 1} Server";
    }

    public override void ProcessPacket(GameClient client, InPacket packet)
    {
        //throw new NotImplementedException();
    }
}