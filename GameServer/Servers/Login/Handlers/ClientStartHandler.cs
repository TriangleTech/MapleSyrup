using GameServer.Client;
using GameServer.Servers.Interfaces;
using MapleSyrup.Networking.Packets;

namespace GameServer.Servers.Login.Handlers;

public class ClientStartHandler : IPacketHandler 
{
    public void HandlePacket(GameClient client, Packet packet)
    {
        lock (this)
        {
            Console.WriteLine("Received ClientStartPacket");
            using var fs = File.OpenRead("MapData/MapLogin.json");
            using var stream = new MemoryStream((int)fs.Length);
            fs.CopyTo(stream);
            stream.Seek(0, SeekOrigin.Begin);
            var outPacket = new Packet((short)ServerToClient.ClientStart);
            outPacket.SetData(stream.ToArray());

            _ = client.SendPacket(outPacket);
        }
    }
}