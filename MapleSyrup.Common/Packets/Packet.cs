using System.Text;

namespace MapleSyrup.Common.Packets;

public class Packet
{
    public short PacketType { get; }
    private MemoryStream _stream;
    private BinaryReader? _reader;
    private BinaryWriter? _writer;

    public ArraySegment<byte> Data => _stream.ToArray();

    public Packet(short packetType)
    {
        PacketType = packetType;
        _stream = new MemoryStream();
        _writer = new BinaryWriter(_stream);
    }

    public Packet(short packetType, Span<byte> data)
    {
        PacketType = packetType;
        _stream = new MemoryStream(data.ToArray());
        _reader = new BinaryReader(_stream);
    }

    ~Packet()
    {
        _writer?.Dispose();
        _reader?.Dispose();
    }
    
    #region Reader

    public byte ReadByte()
    {
        return _reader?.ReadByte() ?? 0;
    }

    public Span<byte> ReadBytes(int count)
    {
        return _reader?.ReadBytes(count);
    }

    public short ReadShort()
    {
        return _reader?.ReadInt16() ?? -1;
    }

    public int ReadInt()
    {
        return _reader?.ReadInt32() ?? -1;
    }

    public long ReadLong()
    {
        return _reader?.ReadInt64() ?? -1;
    }

    public double ReadDouble()
    {
        return _reader?.ReadDouble() ?? -1;
    }

    public float ReadFloat()
    {
        return _reader?.ReadSingle() ?? -1;
    }

    public string ReadString()
    {
        var len = ReadShort();
        if (len == -1) return string.Empty;
        
        var data = _reader?.ReadBytes(len);
        if (data == null) return string.Empty;
        
        return Encoding.UTF8.GetString(data);
    }
    
    #endregion
    
    #region Writer
    
    public void WriteByte(byte value)
    {
        _stream.WriteByte(value);
    }
    
    public void WriteShort(short value)
    {
        _writer?.Write(BitConverter.GetBytes(value));
    }

    public void WriteInt(int value)
    {
        _writer?.Write(BitConverter.GetBytes(value));
    }

    public void WriteLong(long value)
    {
        _writer?.Write(BitConverter.GetBytes(value));
    }

    public void WriteFloat(float value)
    {
        _writer?.Write(BitConverter.GetBytes(value));
    }

    public void WriteDouble(double value)
    {
        _writer?.Write(BitConverter.GetBytes(value));
    }

    public void WriteString(string value)
    {
        WriteShort((short)value.Length);
        _writer?.Write(Encoding.UTF8.GetBytes(value));
    }
    
    #endregion
}