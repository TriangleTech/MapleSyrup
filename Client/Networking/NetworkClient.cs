using System.Net.Sockets;
using Client.Networking.Packets;

namespace Client.Networking;

public class NetworkClient
{
    private string _host;
    private int _port;
    private int _clientId;
    private Socket _socket; 
    private const int MinPacketSize = 6;
    private const int MaxPacketSize = 2048;
    
    public static NetworkClient Shared { get; set; }

    public NetworkClient(string host, int port)
    {
        _host = host;
        _port = port;
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Shared = this;
    }

    public void ChangeChannel(int channelId)
    {
        lock (this)
        {
            try
            {
                // TODO: Do Change Channel packet.
                _socket.Disconnect(true);
                _port = 7575 + channelId;
                Connect();
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    public void Connect()
    {
        Task.Factory.StartNew(() =>
        {
            try
            {
                _socket.Connect(_host, _port);

                if (!_socket.Connected) throw new SocketException();
                Console.WriteLine("Connected to server");
                var data = new byte[4];
                _socket.Receive(data);
            
                // For now, we only need the client id. This will help with processing packets.
                using var stream = new MemoryStream(data);
                using var r = new BinaryReader(stream);
                _clientId = r.ReadInt32();

                while (_socket.Connected)
                {
                    if (_socket.Available <= 0) continue;
            
                    var header = new byte[MinPacketSize];
                    var headerLength = _socket.Receive(header, SocketFlags.None);
                    if (headerLength == 0) // Client disconnected.
                        continue;
                    if (headerLength != MinPacketSize)
                        continue;
                    using var mem = new MemoryStream(header);
                    using var reader = new BinaryReader(mem);
                    var packetLength = reader.ReadInt32();
                    var packetId = reader.ReadInt16();
            
                    if (packetLength > MaxPacketSize)
                        throw new Exception("Possible malicious packet length.");

                    var packet = new InPacket()
                    {
                        PacketType = packetId,
                        Data = new byte[packetLength],
                    };
            
                    var length = _socket.Receive(packet.Data, SocketFlags.None);
                    if (length == 0 || length > packet.Data.Count)
                        continue;
                    Console.WriteLine("Next");
                    ProcessPacket(packet);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        });
    }
    
    private void ProcessPacket(InPacket packet)
    {
        lock (this)
        {
            try
            {

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    public void Stop()
    {
        _socket.Disconnect(false);
        _socket.Close();
    }
}