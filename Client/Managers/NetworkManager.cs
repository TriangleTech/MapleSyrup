using System.Net;
using System.Net.Sockets;
using System.Text;
using Client.Net;

namespace Client.Managers;

public class NetworkManager : IManager
{
    private Socket _socket;
    private object _lock;
    private Thread _thread;
    private readonly int _loginPort = 8484;
    private readonly List<int> _channelPorts = [7575, 7576];
    private byte[] _buffer;
    private PacketProcessor _processor;
    
    public NetworkManager()
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _lock = new();
        _processor = new PacketProcessor();
    }

    public void Initialize()
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _thread = new Thread(BeginConnect)
        {
            IsBackground = true
        };
    }

    public void Connect()
    {
        _thread.Start();
    }

    private void BeginConnect()
    {
        _socket.BeginConnect("127.0.0.1", _loginPort, OnClientConnected, _socket);
    }

    private void OnClientConnected(IAsyncResult ar)
    {
        var socket = ar.AsyncState as Socket;
        socket?.EndConnect(ar);

        if ((bool)socket?.Connected)
        {
            Console.WriteLine("Successfully Connected To Login Server.");

            _buffer = new byte [4];
            _socket.BeginReceive(_buffer, 0, 4, SocketFlags.None, OnHeaderReceived, _socket);
        }
        else
        {
            _socket.Disconnect(false);
            _socket.Dispose();
        }
    }

    private void OnHeaderReceived(IAsyncResult ar)
    {
        var socket = ar.AsyncState as Socket;

        if (socket?.ReceiveBufferSize != 2)
        {
            _buffer = new byte [4];
            _socket.BeginReceive(_buffer, 0, 4, SocketFlags.None, OnHeaderReceived, _socket);
            return;
        }
        
        var stream = new MemoryStream(_buffer);
        var reader = new BinaryReader(stream);
        var packetLength = reader.ReadUInt16();
        var packetHeader = reader.ReadUInt16();
        var response = new PacketResponse((ResponseOps)packetHeader, packetLength);
        _buffer = new byte[packetLength];
            
        _socket.BeginReceive(_buffer, 0, packetLength, SocketFlags.None, OnDataReceived, response);
    }

    private void OnDataReceived(IAsyncResult ar)
    {
        var packet = ar.AsyncState as PacketResponse;

        if (packet?.Length is 0 or > 1024 or null)
        {
            _buffer = new byte [4];
            _socket.BeginReceive(_buffer, 0, 4, SocketFlags.None, OnHeaderReceived, _socket);
            return;
        }
        
        packet.SetData(_buffer);
        Task.Run(() => _processor.ProcessResponse(packet));
        
        _buffer = new byte [4];
        _socket.BeginReceive(_buffer, 0, 4, SocketFlags.None, OnHeaderReceived, _socket);
    }

    public void Shutdown()
    {
        _socket.Disconnect(false);
        _socket.Dispose();
    }
}