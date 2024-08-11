using System.Net.Sockets;
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
        _thread = new Thread(BeginConnect)
        {
            IsBackground = true
        };
    }

    public void Initialize()
    {
        
    }

    public void Connect()
    {
        _thread.Start();
    }

    public void Disconnect()
    {
        _thread.Join();
        _socket.Disconnect(false);
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

            _buffer = new byte [6];
            _socket.BeginReceive(_buffer, 0, 6, SocketFlags.None, OnHeaderReceived, _socket);
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
        
        if (socket == null || !socket.Connected)
        {
            return;
        }

        if (socket?.ReceiveBufferSize < 6)
        {
            _buffer = new byte [6];
            _socket.BeginReceive(_buffer, 0, 6, SocketFlags.None, OnHeaderReceived, _socket);
            return;
        }
        
        var stream = new MemoryStream(_buffer);
        var reader = new BinaryReader(stream);
        var packetLength = reader.ReadInt32();
        var packetIdentifier = reader.ReadUInt16();
        var response = new InPacket((ResponseOps)packetIdentifier, packetLength);
        Console.WriteLine($"Packet Received: {response.Opcode}");
        _buffer = new byte[packetLength];
        _socket.BeginReceive(_buffer, 0, packetLength, SocketFlags.None, OnDataReceived, response);
    }

    private void OnDataReceived(IAsyncResult ar)
    {
        var packet = ar.AsyncState as InPacket;

        if (packet == null || packet.PacketLength <= 0)
        {
            _buffer = new byte [6];
            _socket.BeginReceive(_buffer, 0, 6, SocketFlags.None, OnHeaderReceived, _socket);
            return;
        }
        
        Console.WriteLine($"Processing Packet: {packet.Opcode}");
        packet.SetData(_buffer);
        _processor.Queue(packet);
        _buffer = new byte [6];
        _socket.BeginReceive(_buffer, 0, 6, SocketFlags.None, OnHeaderReceived, _socket);
    }

    public void SendPacket(OutPacket request)
    {
        if (request.Data.Length <= 0) return;
        byte[] packetSize = BitConverter.GetBytes(request.Data.Length);
        byte[] opcode = BitConverter.GetBytes((short)request.Opcode);
        byte[] header = new byte[6];
        packetSize.CopyTo(header, 0);
        opcode.CopyTo(header, 4);
        //Console.WriteLine($"Sending Packet: {request.Opcode} to server, with length of {request.Data.Length}");
        //Console.WriteLine($"HEX DUMP: {BitConverter.ToString(request.Data)}");
        _socket.Send(header);
        _socket.Send(request.Data);
    }

    public void HandlePacket()
    {
        _processor.ProcessPacketResponses();
    }

    public void Shutdown()
    {
        _socket.Disconnect(false);
        _socket.Dispose();
    }
}