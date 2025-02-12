namespace Client.Networking.Packets;

public struct InPacket
{
    public required short PacketType { get; init; }
    public required ArraySegment<byte> Data { get; init; }
}