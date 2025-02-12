namespace GameServer.Servers.Packets;

public class PacketData
{
    public short PacketId;
    public required int ClientId;
    public required ArraySegment<byte> Data;
}