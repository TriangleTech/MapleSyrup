using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Text;

namespace Client.Nx;

public unsafe  class NXBuffer : IDisposable
{
    private readonly MemoryMappedViewAccessor _view;
    private long _position, _offset;
    private readonly long _size;
    
    public NXBuffer(MemoryMappedFile file)
    {
        _view = file.CreateViewAccessor();
        _position = 0;
        _offset = 0;
        _size = _view.Capacity;
    }

    public Span<byte> ReadBytes(int len)
    {
        if (!CheckBounds(_position, len)) return Span<byte>.Empty;

        byte *data = null;
        _view.SafeMemoryMappedViewHandle.AcquirePointer(ref data);
        try
        {
            var span = new Span<byte>(data + _offset + _position, len);
            _position += len;
            
            return span;
        }
        finally
        {
            _view.SafeMemoryMappedViewHandle.ReleasePointer();
        }
    }

    public byte ReadByte()
    {
        if (!CheckBounds(_position, 1)) return 0;
        
        byte *data = null;
        _view.SafeMemoryMappedViewHandle.AcquirePointer(ref data);
        try
        {
            byte value = *(data + _offset + _position);
            _position += sizeof(byte);
            
            return value;
        }
        finally
        {
            _view.SafeMemoryMappedViewHandle.ReleasePointer();
        }
    }

    public ushort ReadUInt16()
    {
        if (!CheckBounds(_position, 2)) return 0;
        
        byte *data = null;
        _view.SafeMemoryMappedViewHandle.AcquirePointer(ref data);
        try
        {
            ushort value = *(ushort*)(data + _offset + _position);
            _position += sizeof(ushort);
            
            return value;
        }
        finally
        {
            _view.SafeMemoryMappedViewHandle.ReleasePointer();
        }
    }
    
    public uint ReadUInt32()
    {
        if (!CheckBounds(_position, sizeof(uint))) return 0;
        
        byte *data = null;
        _view.SafeMemoryMappedViewHandle.AcquirePointer(ref data);
        try
        {
            uint value = *(uint*)(data + _offset + _position);
            _position += sizeof(uint);
            
            return value;
        }
        finally
        {
            _view.SafeMemoryMappedViewHandle.ReleasePointer();
        }
        
    }

    public ulong ReadUInt64()
    {
        if (!CheckBounds(_position, sizeof(ulong))) return 0;
        
        byte *data = null;
        _view.SafeMemoryMappedViewHandle.AcquirePointer(ref data);
        try
        {
            ulong value = *(ulong*)(data + _offset + _position);
            _position += sizeof(ulong);
            
            return value;
        }
        finally
        {
            _view.SafeMemoryMappedViewHandle.ReleasePointer();
        }
        
    }

    public string ReadString()
    {
        var len = ReadUInt16();
        if (len <= 0) return string.Empty;
        
        var data = ReadBytes(len);
        return Encoding.UTF8.GetString(data);
    }

    private bool CheckBounds(long offset, long size)
    {
        return offset + size <= _size;
    }

    public void Seek(long offset)
    {
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
        if (offset > _size) throw new ArgumentOutOfRangeException(nameof(offset));
        
        _offset = offset;
        _position = 0;
    }
    
    public void Dispose()
    {
        _view.Dispose();
        GC.SuppressFinalize(this);
    }
}