namespace MapleSyrup.Common.Packets;

public enum ServerToClient : short
{
    MapData = 0x00,
    UIData = 0x01,
    PlayerData = 0x02,
}