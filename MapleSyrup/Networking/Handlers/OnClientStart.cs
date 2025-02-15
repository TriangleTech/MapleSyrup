using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using MapleSyrup.Common.Map;
using MapleSyrup.Common.Packets;
using MapleSyrup.Scenes;
using MapleSyrup.Windowing;
using MapleMapContext = MapleSyrup.Common.Map.MapleMapContext;

namespace MapleSyrup.Networking.Handlers;

public class OnClientStart : IPacketHandler
{
    public void HandlePacket(Packet packet)
    {
        var json = Encoding.ASCII.GetString(packet.Data);
        var map = JsonSerializer.Deserialize(json, MapleMapContext.Default.MapleMap)
            ?? throw new NullReferenceException();
        SceneFactory.Shared.CreateScene<LoginScene>(map);
    }
}