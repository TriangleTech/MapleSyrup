using System.Runtime.CompilerServices;
using System.Text;

namespace MapleSyrup.Nx;

public unsafe class NxReader : IDisposable
{
    private Memory<byte> _memory;
    private uint _position;
    private int _nodeCount, _stringCount, _bitmapCount, _audioCount;
    private long _nodeBlockOffset, _stringBlockOffset, _bitmapBlockOffset, _audioBlockOffset;

    public Memory<byte> Memory => _memory;

    public NxReader(byte[] data)
    {
        _memory = data;
        _position = 0;
        
        _nodeCount = ReadInt();
        _nodeBlockOffset = ReadLong();
        _stringCount = ReadInt();
        _stringBlockOffset = ReadLong();
        _bitmapCount = ReadInt();
        _bitmapBlockOffset = _bitmapCount > 0 ? ReadLong() : 0;
        _audioCount = ReadInt();
        _audioBlockOffset = _audioCount > 0 ? ReadLong() : 0;
    }
    
    #region Read Functions

    public Span<byte> ReadBytes(int offset, int length)
    {
        Span<byte> bytes = stackalloc byte[length];
        fixed (byte* data = _memory.Span)
        {
            for (int i = 0; i < length; i++)
            {
                bytes[i] = *(data + offset + i);
            }
        }

        return bytes.ToArray();
    }

    public short ReadShort(int offset = -1)
    {
        fixed (byte* data = _memory.Span)
        {
            if (offset > -1)
            {
                var offsetValue = Unsafe.Read<short>(data + offset);
                return offsetValue;
            }

            var value = Unsafe.Read<short>(data + _position);
            _position += sizeof(short);

            return value;
        }
    }

    public int ReadInt(int offset = -1)
    {
        fixed (byte* data = _memory.Span)
        {
            if (offset > -1)
            {
                var offsetValue = Unsafe.Read<int>(data + offset);
                return offsetValue;
            }
            
            var value = Unsafe.Read<int>(data + _position);
            _position += sizeof(int);

            return value;
        }
    }

    public long ReadLong(int offset = -1)
    {
        fixed (byte* data = _memory.Span)
        {
            if (offset > -1)
            {
                var offsetValue = Unsafe.Read<long>(data + offset);
                return offsetValue;
            }

            var value = Unsafe.Read<long>(data + _position);
            _position += sizeof(long);

            return value;
        }
    }

    public string ReadString(int offset = -1)
    {
        if (offset > -1)
        {
            var len = ReadShort(offset);
            var str = ReadBytes(offset + sizeof(short), len);

            return Encoding.UTF8.GetString(str);
        }

        var length = ReadShort();
        var stringData = ReadBytes((int)_position, length);

        return Encoding.UTF8.GetString(stringData);
    }
    
    #endregion
    
    #region Data Parsing

    public void ParseChildren(int firstChildId, int childCount, out List<int> offsets)
    {
        
    }
    
    #endregion

    public void Dispose()
    {
        _memory.Span.Clear();
    }
}