using System.Net;
using System.Net.Sockets;
using System.Text;
using MSClient.Net;

namespace MSClient.Managers;

public class NetworkManager : IManager
{
    private Socket _socket;
    private ArraySegment<byte> _segment;
    private const int HeaderLength = 4;
    private const int OpcodeLength = 2;
    private const int MinHeaderLength = OpcodeLength + HeaderLength;
    private bool _initialStart;
    private object _lock;
    private Thread _networkThread;
    private Queue<PacketRequest> _requests;
    private Queue<PacketResponse> _responses;
    
    public NetworkManager()
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _initialStart = true;
        _lock = new();
        _responses = new();
        _requests = new();
    }
    
    public void Initialize()
    {
        _networkThread = new Thread(() => Connect());
        _networkThread.Start();
    }

    public void Shutdown()
    {
        _networkThread.Join();
        _socket.Disconnect(false);
        _socket.Close();
    }

    public void Connect()
    {
        // Yes, I know you can turn this into a linq statement...
        // do I care? no.
        _socket.BeginConnect("127.0.0.1", 8484, OnClientConnected, _socket);
    }

    private void OnClientConnected(IAsyncResult ar)
    {
        var socket = ar.AsyncState as Socket;
        socket?.EndConnect(ar);
        Console.WriteLine("Client Successfully Connected.");
        
        // get that annoying on client start packet.
        // is it 16 bytes? idfk 
        WaitForData(16);
    }
    
    public void WaitForData(int length)
    {
        if (!_socket.Connected) return;
        
        _segment = new ArraySegment<byte>(new byte[length]);
        var buffer = _segment.Array;
        _socket.BeginReceive(buffer, 0, length, SocketFlags.None, OnDataReceived, _socket);
    }

    private void OnDataReceived(IAsyncResult ar)
    {
        if (!_socket.Connected) return;
        
        var socket = ar.AsyncState as Socket;
        socket?.EndReceive(ar);
        
        if (socket == null) return;

        if (_initialStart)
        {
            // this shit is useless
            // I got rid of all aes and ms encryption stuff
            // on the server I use.
            using var memory = new MemoryStream(_segment.Array);
            using var reader = new BinaryReader(memory);
            var opcode = reader.ReadInt16();
            var version = reader.ReadInt16();
            var unk1 = reader.ReadInt16();
            var unk2 = reader.ReadByte();
            var recvIv = reader.ReadBytes(4);
            var sendIv = reader.ReadBytes(4);
            var region = reader.ReadByte();
            _initialStart = false;
            
            WaitForData(HeaderLength);
        }
        
        if (socket?.ReceiveBufferSize <= 0)
            WaitForData(HeaderLength);

        if (socket?.ReceiveBufferSize == HeaderLength)
        {
            using var memory = new MemoryStream(_segment.Array);
            using var reader = new BinaryReader(memory);
            var packetLength = reader.ReadInt32();
            Console.WriteLine($"Packet Length: {packetLength}");
            WaitForData(packetLength);
        }

        if (socket?.ReceiveBufferSize >= MinHeaderLength)
        {
            using var memory = new MemoryStream(_segment.Array);
            using var reader = new BinaryReader(memory);
        }
    }

    public void ProcessRequests()
    {
        lock (_lock)
        {
            Task.Run(() =>
            {
                while (_requests.Count > 0)
                {
                    var request = _requests.Dequeue();
                    _socket.Send(request.Data);
                }
            });
        }
    }
    
    public void ProcessResponses()
    {
        lock (_lock)
        {
            Task.Run(() =>
            {
                var am = ServiceLocator.Get<ActorManager>();
                while (_responses.Count > 0)
                {
                    var response = _responses.Dequeue();
                    am.DispatchResponse(response);
                }
            });
        }
    }

    public void QueueRequest(PacketRequest request)
    {
        if (request.Data.Length <= 0)
            return;
        _requests.Enqueue(request);
    }
}