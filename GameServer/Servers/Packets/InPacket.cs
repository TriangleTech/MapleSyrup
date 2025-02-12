namespace GameServer.Servers.Packets;

public struct InPacket
{
    public required short PacketType;
    public required int ClientId;
    public required ArraySegment<byte> Data;
}