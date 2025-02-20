using MapleSyrup.Common.Map;
using MapleSyrup.Common.Packets;

namespace MapleSyrup.Networking.Packets;

public static class CommonPackets
{
    public static Packet RequestMapData(MapType mapType, string mapName)
    {
        var packet = new Packet((short)ClientToServer.RequestMapData);
        packet.WriteByte((byte)mapType);
        packet.WriteString(mapName);
        return packet;
    }
}