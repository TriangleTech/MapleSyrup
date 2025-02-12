using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using GameServer.Client;
using GameServer.Servers.Packets;

namespace GameServer.Servers;

public abstract class NetworkServer
{
    private readonly int _port;
    private readonly Socket _socket;
    private const int MaxClients = 10;
    private readonly ConcurrentDictionary<int, GameClient> _clients;
    private int _clientCount;
    
    public const int MinPacketSize = 10;
    public const int MaxPacketSize = 2048;
    public string ServerName { get; protected set; }

    public bool IsRunning { get; private set; }
    
    protected NetworkServer(int port)
    {
        _port = port;
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            Blocking = true,
        };
        _clients = new ();
        _clientCount = 0;
    }

    public void Start()
    {
        _socket.Bind(new IPEndPoint(IPAddress.Parse(Configuration.ServerAddress), _port));
        _socket.Listen();
        IsRunning = true;
        Console.WriteLine($"{ServerName} listening on {Configuration.ServerAddress}:{_port}");
        _ = OnStart();
    }

    public void Stop()
    {
        Console.WriteLine($"{ServerName} stopping on {_port}");
        IsRunning = false;
        foreach (var client in _clients.Values)
            client.Socket.Close();
        _socket.Close();
    }

    private async Task OnStart()
    {
        await Task.Factory.StartNew(async Task () =>
        {
            while (IsRunning)
            {
                var client = await OnListenForConnection();
                if (client == -1) continue;
                _clients[client].WaitForData();
            }
        });
    }

    private async Task<int> OnListenForConnection()
    {
        if (!IsRunning) return -1;
        
        try
        {
            var client = await _socket.AcceptAsync();
            if (!client.Connected) return -1;

            lock (this)
            {
                var clientId = _clientCount++;
                _clients.TryAdd(clientId, new GameClient()
                {
                    Id = clientId,
                    Socket = client,
                    AssignedServer = this
                });


                _clients[clientId].Socket.Send(BitConverter.GetBytes(clientId), SocketFlags.None);
                Console.WriteLine($"Client connected from {client.RemoteEndPoint}");
                return clientId;
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void DisconnectClient(GameClient client)
    {
        lock (this)
        {
            try
            {
                client.Socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                client.Socket.Close();
                _clients.TryRemove(client.Id, out _);
                Console.WriteLine($"Client {client.Id} disconnected");
            }
        }
    }
    
    public abstract void ProcessPacket(GameClient client, InPacket packet);
}