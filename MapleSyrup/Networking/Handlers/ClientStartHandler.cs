using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using MapleSyrup.Networking.Packets;
using MapleSyrup.Scenes;
using MapleSyrup.Windowing;
using MapleMapContext = MapleSyrup.Scenes.Map.MapleMapContext;

namespace MapleSyrup.Networking.Handlers;

public class ClientStartHandler : IPacketHandler
{
    public void HandlePacket(Packet packet)
    {
        var json = Encoding.ASCII.GetString(packet.Data);
        var map = JsonSerializer.Deserialize<Scenes.Map.MapleMap>(json, MapleMapContext.Default.MapleMap); 
        Console.WriteLine($"Backgrounds : {map.Backgrounds.Count}");
        SceneFactory.Shared.CreateScene<LoginScene>("MapLogin", map);
    }
}