using System.Text;
using Client.Managers;

namespace Client.Net;

public class PacketResponse
{
    private readonly ResponseOps _opcode;
    private readonly int _packetLength;
    private MemoryStream _stream;
    private BinaryReader _reader;
    private long _position;
    private readonly NetworkManager _network;

    public ResponseOps Opcode => _opcode;
    public int Length => _packetLength;

    public PacketResponse(ResponseOps responseOps, int length)
    {
        _opcode = responseOps;
        _packetLength = length;
        _network = ServiceLocator.Get<NetworkManager>();
    }
    
    ~PacketResponse()
    {
        _reader.Close();
        _reader.Dispose();
    }

    public void SetData(byte[] data)
    {
        _stream = new MemoryStream(data);
        _reader = new BinaryReader(_stream);
    }
    
    public Span<byte> ReadBytes(int length)
    { 
        var list = _reader.ReadBytes(length);
        _position += length;
        return list;
    }
    
    public Span<byte> ReadBytes(long offset, int length)
    {
        Seek(offset);
        var list = _reader.ReadBytes(length);
        _position += length;
        
        return list;
    }

    public char ReadChar()
    {
        var result = _reader.ReadChar();
        _position += sizeof(char);

        return result;
    }

    public short ReadShort()
    {
        var result = _reader.ReadInt16();
        _position += sizeof(short);

        return result;
    }

    public int ReadInt()
    {
        var result = _reader.ReadInt32();
        _position += sizeof(int);

        return result;
    }

    public long ReadLong()
    {
        var result = _reader.ReadInt64();
        _position += sizeof(long);

        return result;
    }

    public float ReadFloat()
    {
        var result = _reader.ReadSingle();
        _position += sizeof(float);

        return result;
    }

    public double ReadDouble()
    {
        var result = _reader.ReadDouble();
        _position += sizeof(double);

        return result;
    }

    public bool ReadBoolean()
    {
        var result = _reader.ReadBoolean();
        _position += sizeof(bool);

        return result;
    }
    
    // TODO: Date, Time, DateTime

    public string ReadMapleString()
    {
        var length = ReadShort();
        var data = ReadBytes(length);

        return Encoding.UTF8.GetString(data);
    }
    
    public string ReadNullTerminatedString()
    {
        var count = 0;
        char character;
        List<char> chars = new List<char>();
        
        while ((character = _reader.ReadChar()) != 0)
        {
            chars[count] = character;
            count++;
        }

        return new string(chars.ToArray());
    }
    
    public string ReadString(long offset)
    {
        Seek(offset);
        var length = ReadShort();
        var data = ReadBytes(offset + 2, length);

        return Encoding.UTF8.GetString(data);
    }

    public void Seek(long position)
    {
        if (position >= _reader.BaseStream.Length)
            throw new Exception("Position is set greater than the file size.");
        if (position < 0)
            throw new Exception("Position is set less than zero.");
        _position = position;
        _reader.BaseStream.Position = _position;
    }

    public void Skip(long numberOfBytes)
    {
        if ((_position + numberOfBytes) >= _reader.BaseStream.Length)
            throw new Exception("Attempted to skip further than the file size.");
        _position += numberOfBytes;
        _reader.BaseStream.Position += numberOfBytes;
    }
}