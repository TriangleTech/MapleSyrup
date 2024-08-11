using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;

namespace Client.NX;

public unsafe class NxReader : IDisposable
{
    private readonly BinaryReader _reader;
    private long _position;
    private int _nodeCount, _stringCount, _bitmapCount, _audioCount;
    private long _nodeBlockOffset, _stringBlockOffset, _bitmapBlockOffset, _audioBlockOffset;
    
    public int NodeCount => _nodeCount;
    public int StringCount => _stringCount;
    public int BitmapCount => _bitmapCount;
    public int AudioCount => _audioCount;
    public long NodeBlockOffset => _nodeBlockOffset;
    public long StringBlockOffset => _stringBlockOffset;
    public long BitmapBlockOffset => _bitmapBlockOffset;
    public long AudioBlockOffset => _audioBlockOffset;
    
    public NxReader(FileStream stream)
    {
        _reader = new BinaryReader(stream, Encoding.UTF8);
        _position = 0;
        ParseHeader();
    }

    private void ParseHeader()
    {
        Seek(0);
        var magic = ReadInt();
        if (magic != 0x34474B50) throw new Exception("Failed to obtain PKG4 Magic");
        _nodeCount = ReadInt();
        _nodeBlockOffset = ReadLong();
        _stringCount = ReadInt();
        _stringBlockOffset = ReadLong();
        _bitmapCount = ReadInt();
        _bitmapBlockOffset = _bitmapCount > 0 ? ReadLong() : 0;
        _audioCount = ReadInt();
        _audioBlockOffset = _audioCount > 0 ? ReadLong() : 0;
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

    public string ReadString()
    {
        var length = ReadShort();
        var data = ReadBytes(length);

        return Encoding.UTF8.GetString(data);
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

    public void Dispose()
    {
        _reader.Close();
        _reader.Dispose();
    }
}