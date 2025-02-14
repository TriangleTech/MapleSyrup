using GameServer.Client;
using GameServer.Servers.Interfaces;
using GameServer.Servers.Login.Handlers;
using MapleSyrup.Networking.Packets;

namespace GameServer.Servers.Login;

public class LoginServer : NetworkServer
{
    private readonly Dictionary<ClientToServer, IPacketHandler> _packetHandlers;
    
    public LoginServer() 
        : base("Login", Configuration.LoginPort)
    {
        _packetHandlers = new()
        {
            { ClientToServer.OnClientStart, new ClientStartHandler() }
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