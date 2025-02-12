namespace Client.Networking.Packets;

public struct PacketData
{
    public short Id;
    public required byte[] Data;
}