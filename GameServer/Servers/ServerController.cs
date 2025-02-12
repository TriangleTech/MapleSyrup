using GameServer.Servers.Channel;
using GameServer.Servers.Login;

namespace GameServer.Servers;

public class ServerController
{
    private readonly LoginServer _loginServer;
    private readonly Dictionary<int, ChannelServer> _channelServers;

    public ServerController()
    {
        _loginServer = new LoginServer();
        _channelServers = new Dictionary<int, ChannelServer>();
    }

    public void Initialize()
    {
        for (var i = 0; i < Configuration.ChannelCount; i++)
        {
            _channelServers.Add(i, new ChannelServer(Configuration.ChannelPort + i));
        }
    }

    public void StartServers()
    {
        _loginServer.Start();
        foreach (var (_, channelServer) in _channelServers)
        {
            channelServer.Start();
        }
    }

    public void StopServers()
    {
        _loginServer.Stop();
        foreach (var (_, channelServer) in _channelServers)
        {
            channelServer.Stop();
        }
    }

    public void Hold()
    {
        while (true)
        {
            if (!Console.KeyAvailable) continue;
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Q)
            {
                Console.WriteLine("Exiting");
                break;
            };
        }
    }
}