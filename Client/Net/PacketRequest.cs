using System.Diagnostics;
using Client.Managers;

namespace Client.Net;

public class PacketRequest
{
    private byte[] _buffer;
    private readonly RequestOps _opcode;
    private MemoryStream _stream;
    private BinaryWriter _writer;
    private readonly NetworkManager _network;
    public byte[] Data => _buffer;
    public RequestOps Opcode => _opcode;

    public PacketRequest(RequestOps opcode)
    {
        _opcode = opcode;
        _stream = new MemoryStream();
        _writer = new BinaryWriter(_stream);
        _network = ServiceLocator.Get<NetworkManager>();
    }

    public void Write(byte val)
    {
        _writer.Write(val);
    }

    public void Write(byte[] data)
    {
        _writer.Write(data);
    }

    public void Write(short val)
    {
        _writer.Write(val);
    }

    public void Write(int val)
    {
        _writer.Write(val);
    }

    public void Write(long val)
    {
        _writer.Write(val);
    }

    public void Write(double val)
    {
        _writer.Write(val);
    }

    public void Write(float val)
    {
        _writer.Write(val);
    }

    public void Write(bool val)
    {
        _writer.Write(val);
    }

    public void Write(char val)
    {
        _writer.Write(val);
    }

    public void WriteString(string val)
    {
        _writer.Write(val);
    }

    public void WriteMapleString(string val)
    {
        _writer.Write((short)val.Length);
        _writer.Write(val);
    }

    public void Write(TimeOnly val)
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
        Write(new byte[length]);
    }

    public void Queue()
    {
        _buffer = _stream.ToArray();
        _writer.Close();
        _stream.Dispose();
        _writer.Dispose();
        Debug.WriteLine($"Sending Packet: {_opcode} to server, with length of {_buffer.Length}");
    }
}