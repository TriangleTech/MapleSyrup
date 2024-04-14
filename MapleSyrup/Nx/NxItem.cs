using System.Runtime.CompilerServices;

namespace MapleSyrup.Nx;

public unsafe class NxItem : IDisposable
{
    private Dictionary<string, int> _children;
    private NxReader _reader;
    private long _nameOffset;
    private byte _firstChildId;
    private byte _childCount;

    public string Name
    {
        get
        {
            fixed (byte* data = _reader.Memory.Span)
            {
                var stringOffset = Unsafe.Read<long>(data + _stringBlockOffset + (sizeof(long) * nameOffset));
            }
        }
    }

    public NxItem(ref NxReader reader)
    {
        _reader = reader;
        _children = new();
    }
    
    public void Dispose()
    {
        
    }
}