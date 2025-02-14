using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using GameServer.Client;
using MapleSyrup.Networking.Packets;

namespace GameServer.Servers;

public abstract class NetworkServer
{
    private readonly TcpListener _tcpListener;
    private readonly ConcurrentDictionary<int, GameClient> _clients = new();
    private int _clientId;

    public CancellationTokenSource TerminationToken { get; }
    public string ServerName { get; protected set; }
    public int Port { get; }

    public NetworkServer(string serverName, int port)
    {
        ServerName = serverName;
        Port = port;
        TerminationToken = new CancellationTokenSource();
        _tcpListener = new TcpListener(IPAddress.Loopback, Port);
        _clientId = 0;
    }

    public async Task StartAsync()
    {
        try
        {
            Console.WriteLine($"{ServerName} Server started on {IPAddress.Loopback}:{Port}");
            _tcpListener.Start();

            while (!TerminationToken.IsCancellationRequested)
            {
                var clientTask = _tcpListener.AcceptTcpClientAsync();
                var clientConnected = await Task.WhenAny(clientTask, Task.Delay(1000, TerminationToken.Token));
                if (clientTask != clientConnected) continue;

                var client = await clientTask;
                _clients.TryAdd(_clientId, new GameClient()
                {
                    Id = _clientId,
                    AssignedServer = this,
                    Socket = client,
                });

                Console.WriteLine($"Client connected from {client.Client.RemoteEndPoint}");
                _ = _clients[_clientId].WaitForData();

                _clientId++;
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            _tcpListener.Stop();

            foreach (var (_, client) in _clients)
            {
                client.Socket.Close();
            }

            _clients.Clear();
        }
    }

    public async Task Terminate()
    {
        await TerminationToken.CancelAsync();
    }

    public abstract void ProcessPacket(GameClient client, Packet packet);
}