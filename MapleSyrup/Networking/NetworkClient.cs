using System.Net;
using System.Net.Sockets;
using MapleSyrup.Common.Packets;

namespace MapleSyrup.Networking;

public class NetworkClient
{
    private readonly TcpClient _client;
    private readonly PacketProcessor _packetProcessor;
    public CancellationTokenSource TerminationToken { get; }

    public NetworkClient()
    {
        _client = new TcpClient();
        _packetProcessor = new PacketProcessor();
        TerminationToken = new CancellationTokenSource();
    }

    public async Task ConnectAsync()
    {
            try
            {
                await _client.ConnectAsync("127.0.0.1", 8484);
                _ = Send(ClientToServer.OnClientStart);

                while (!TerminationToken.IsCancellationRequested)
                {
                    WaitForData();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
                throw;
            }
    }

    private void WaitForData()
    {
        try
        {
            if (_client.Available <= 0) return;
            using var ns = new NetworkStream(_client.Client, false);
            using var reader = new BinaryReader(ns);

            var headerLength = reader.ReadInt32();
            var packetId = reader.ReadInt16();
            var packet = new Packet(packetId);
            packet.SetData(reader.ReadBytes(headerLength));
            Console.WriteLine($"Packet with ID: {(ServerToClient)packetId} with length of {headerLength}");

            _packetProcessor.ProcessPacket(packet);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task Send(Packet packet)
    {
        try
        {
            var packetLength = BitConverter.GetBytes(packet.Data.Count);
            var packetId = BitConverter.GetBytes(packet.PacketType);
            await using var ns = new NetworkStream(_client.Client, false);
            await using var writer = new BinaryWriter(ns);
            writer.Write(packetLength);
            writer.Write(packetId);
            writer.Write(packet.Data);
            writer.Flush();
            Console.WriteLine($"Sent {packetLength} bytes to server");
        }
        catch (SocketException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task Send(ClientToServer packet)
    {
        try
        {
            await using var ns = new NetworkStream(_client.Client, false);
            await using var writer = new BinaryWriter(ns);
            writer.Write((int)2);
            writer.Write((short)ServerToClient.ClientStart);
            writer.Flush();
            Console.WriteLine($"Sent {packet} to server");
        }
        catch (SocketException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void Stop()
    {
        TerminationToken.Cancel();
        _client.Close();
    }
}