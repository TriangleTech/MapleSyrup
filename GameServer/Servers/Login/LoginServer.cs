using GameServer.Client;
using GameServer.Servers.Packets;

namespace GameServer.Servers.Login;

public class LoginServer : NetworkServer
{
    public LoginServer() 
        : base(Configuration.LoginPort)
    {
        ServerName = "LoginServer";
    }

    public override void ProcessPacket(GameClient client, InPacket packet)
    {
       // throw new NotImplementedException();
    }
}