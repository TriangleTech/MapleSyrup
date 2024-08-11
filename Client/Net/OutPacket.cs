using System.Diagnostics;
using System.Text;
using Client.Managers;

namespace Client.Net;

public class OutPacket : IDisposable
{
    private byte[] _buffer;
    private readonly RequestOps _opcode;
    private MemoryStream _stream;
    private BinaryWriter _writer;
    private readonly NetworkManager _network;
    public byte[] Data => _buffer;
    public RequestOps Opcode => _opcode;

    public OutPacket(RequestOps opcode)
    {
        _opcode = opcode;
        _stream = new MemoryStream();
        _writer = new BinaryWriter(_stream);
        _network = ServiceLocator.Get<NetworkManager>();
    }

    public void WriteByte(byte val)
    {
        _writer.Write(val);
    }

    public void WriteBytes(byte[] data)
    {
        _writer.Write(data);
    }

    public void WriteShort(short val)
    {
        _writer.Write(val);
    }

    public void WriteInt(int val)
    {
        _writer.Write(val);
    }

    public void WriteLong(long val)
    {
        _writer.Write(val);
    }

    public void WriteDouble(double val)
    {
        _writer.Write(val);
    }

    public void WriteFloat(float val)
    {
        _writer.Write(val);
    }

    public void WriteBoolean(bool val)
    {
        _writer.Write(val);
    }

    public void WriteChar(char val)
    {
        _writer.Write(val);
    }

    public void WriteString(string val)
    {
        _writer.Write(val);
    }

    public void WriteMapleString(string val)
    {
        WriteShort((short)val.Length);
        WriteBytes(Encoding.ASCII.GetBytes(val));
    }

    public void WriteTime(TimeOnly val)
    {
        _writer.Write(val.ToLongTimeString());
    }

    public void WriteDate(DateOnly val)
    {
        _writer.Write(val.ToLongDateString());
    }

    public void WriteDateTime(DateTime val)
    {
        // TODO: is this right?
        _writer.Write(val.ToLongDateString());
        _writer.Write(val.ToLongTimeString());
    }

    public void WriteZeroBytes(int length)
    {
        WriteBytes(new byte[length]);
    }

    public void Send()
    {
        _buffer = _stream.ToArray();
        _network.SendPacket(this);
    }

    public void Dispose()
    {
        _writer.Close();
        _stream.Dispose();
        _writer.Dispose();
    }
}