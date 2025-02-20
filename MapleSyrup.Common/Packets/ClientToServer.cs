namespace MapleSyrup.Common.Packets;

public enum ClientToServer : short
{
    RequestMapData = 0x00,
    RequestUIData = 0x01,
    RequestPlayerData = 0x02
}