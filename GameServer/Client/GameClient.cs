using System.Net.Sockets;
using GameServer.Servers;
using MapleSyrup.Networking.Packets;

namespace GameServer.Client;

public class GameClient
{
    public required int Id { get; init; }
    public required TcpClient Socket { get; init; }
    public required NetworkServer AssignedServer { get; init; }

    public async Task WaitForData()
    {
        try
        {
            while (Socket.Connected)
            {
                if (Socket.Available <= 0) continue;
                await using var ns = Socket.GetStream();
                using var reader = new BinaryReader(ns);

                var headerLength = reader.ReadInt32();
                var packetId = reader.ReadInt16();
                var packet = new Packet(packetId);
                Console.WriteLine($"Packet with ID: {(ServerToClient)packetId} with length of {headerLength}");

                AssignedServer.ProcessPacket(this, packet);
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task SendPacket(Packet packet)
    {
        try
        {
            var packetLength = BitConverter.GetBytes(packet.Data.Count);
            var packetId = BitConverter.GetBytes(packet.PacketType);
            await using var ns = Socket.GetStream();
            await using var writer = new BinaryWriter(ns);
            writer.Write(packetLength);
            writer.Write(packetId);
            writer.Write(packet.Data);
            writer.Flush();
            ns.Flush();
            writer.Close();
            ns.Close();
            Console.WriteLine($"Sent {packetLength} bytes to {Socket.Client.RemoteEndPoint}");
        }
        catch (SocketException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}