using System.Net.Sockets;
using GameServer.Servers;
using GameServer.Servers.Packets;

namespace GameServer.Client;

public class GameClient
{
    public required int Id { get; init; }
    public required Socket Socket { get; init; }
    public required NetworkServer AssignedServer { get; init; }
    public int ChannelId { get; set; }

    public void WaitForData()
    {
        Task.Factory.StartNew(() =>
        {
            try
            {
                while (AssignedServer.IsRunning)
                {
                    var header = new byte[NetworkServer.MinPacketSize];
                    var headerLength = Socket.Receive(header, SocketFlags.None);
                    if (headerLength == 0) // Client disconnected.
                    {
                        AssignedServer.DisconnectClient(this);
                        break;
                    }
                    if (headerLength != NetworkServer.MinPacketSize)
                        continue;
                    Console.WriteLine($"Received {headerLength} bytes from {Socket.RemoteEndPoint}");
                    using var mem = new MemoryStream(header);
                    using var reader = new BinaryReader(mem);
                    var packetLength = reader.ReadInt32();
                    var packetId = reader.ReadInt16();
                    var clientId = reader.ReadInt32();

                    if (packetLength > NetworkServer.MaxPacketSize)
                        throw new Exception("Possible malicious packet length.");

                    var packet = new InPacket()
                    {
                        PacketType = packetId,
                        ClientId = clientId,
                        Data = new byte[packetLength],
                    };

                    var length = Socket.Receive(packet.Data, SocketFlags.None);
                    if (length == 0 || length > packet.Data.Count)
                        continue;
                    AssignedServer.ProcessPacket(this, packet);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
                throw;
            }
        });
    }
}