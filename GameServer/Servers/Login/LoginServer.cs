using GameServer.Client;
using GameServer.Servers.Interfaces;
using GameServer.Servers.Shared;
using MapleSyrup.Common.Packets;

namespace GameServer.Servers.Login;

public class LoginServer : NetworkServer
{
    private readonly Dictionary<ClientToServer, IPacketHandler> _packetHandlers;
    
    public LoginServer() 
        : base("Login", Configuration.LoginPort)
    {
        _packetHandlers = new()
        {
            { ClientToServer.RequestMapData, new MapDataRequest() }
        };
    }

    public override void ProcessPacket(GameClient client, Packet packet)
    {
        lock (this)
        {
            try
            {
                var id = (ClientToServer)packet.PacketType;
                if (!_packetHandlers.TryGetValue(id, out var handler)) return;

                handler.HandlePacket(client, packet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}