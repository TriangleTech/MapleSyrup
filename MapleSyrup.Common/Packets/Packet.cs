using System.Text;
using CommunityToolkit.HighPerformance;

namespace MapleSyrup.Common.Packets;

public class Packet
{
    public short PacketType { get; }
    private List<byte> _data;
    
    public ArraySegment<byte> Data => _data.ToArray();

    public Packet(short packetType, int capacity = 256)
    {
        PacketType = packetType;
        _data = new(capacity);
    }

    public void SetData(byte[] data)
    {
        _data = new(data);
    }

    public void WriteByte(byte value)
    {
        _data.Add(value);
    }
    
    public void WriteShort(short value)
    {
        _data.AddRange(BitConverter.GetBytes(value));
    }

    public void WriteInt(int value)
    {
        _data.AddRange(BitConverter.GetBytes(value));
    }

    public void WriteLong(long value)
    {
        _data.AddRange(BitConverter.GetBytes(value));
    }

    public void WriteFloat(float value)
    {
        _data.AddRange(BitConverter.GetBytes(value));
    }

    public void WriteDouble(double value)
    {
        _data.AddRange(BitConverter.GetBytes(value));
    }

    public void WriteString(string value)
    {
        WriteShort((short)value.Length);
        _data.AddRange(Encoding.UTF8.GetBytes(value));
    }

    public ReadOnlySpan<byte> GetData()
    {
        return _data.AsSpan();
    }

    public void Destroy()
    {
        _data.Clear();
    }
}