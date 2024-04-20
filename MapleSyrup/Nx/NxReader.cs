using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Text;

namespace MapleSyrup.Nx;

public unsafe class NxReader : IDisposable
{
    private MemoryMappedFile _mmf;
    private Dictionary<string, long> _stringPool;
    private int _magic, _nodeCount, _stringCount, _bitmapCount, _audioCount;
    private long _nodeBlockOffset, _stringBlockOffset, _bitmapBlockOffset, _audioBlockOffset;
    private string _fileName;

    public int NodeCount => _nodeCount;
    public int StringCount => _stringCount;
    public int BitmapCount => _bitmapCount;
    public int AudioCount => _audioCount;

    public long NodeBlockOffset => _nodeBlockOffset;
    public long StringBlockOffset => _stringBlockOffset;
    public long BitmapBlockOffset => _bitmapBlockOffset;
    public long AudioBlockOffset => _audioBlockOffset;
    public Dictionary<string, long> StringPool => _stringPool;
    public string File => _fileName;

    public NxReader(string path)
    {
        _mmf = MemoryMappedFile.CreateFromFile(path, FileMode.Open);
        _stringPool = new();
        _fileName = Path.GetFileNameWithoutExtension(path);

        var magic = ReadInt(0);
        if (magic != 0x34474B50)
            throw new Exception("Nope");

        // Parse Header
        _nodeCount = ReadInt(4);
        _nodeBlockOffset = ReadLong(8);
        _stringCount = ReadInt(16);
        _stringBlockOffset = ReadLong(20);
        _bitmapCount = ReadInt(28);
        _bitmapBlockOffset = _bitmapCount > 0 ? ReadLong(32) : 0;
        _audioCount = ReadInt(40);
        _audioBlockOffset = _audioCount > 0 ? ReadLong(44) : 0;

        GenerateStringPool(_fileName == "Map");
    }

    public Span<byte> ReadBytes(long offset, int length)
    {
        var buffer = new byte[length];
        using (var accessor = _mmf.CreateViewAccessor(offset, length))
        {
            accessor.ReadArray(0, buffer, 0, length);
        }

        return buffer.AsSpan();
    }

    public short ReadShort(long offset)
    {
        short result;
        using MemoryMappedViewAccessor accessor = _mmf.CreateViewAccessor(offset, sizeof(short));
        result = accessor.ReadInt16(0);

        return result;
    }

    public int ReadInt(long offset)
    {
        using MemoryMappedViewAccessor accessor = _mmf.CreateViewAccessor(offset, sizeof(int));
        var result = accessor.ReadInt32(0);

        return result;
    }

    public long ReadLong(long offset)
    {
        using MemoryMappedViewAccessor accessor = _mmf.CreateViewAccessor(offset, sizeof(long));
        var result = accessor.ReadInt64(0);

        return result;
    }

    public string ReadString(long offset)
    {
        var length = ReadShort(offset);
        var buffer = ReadBytes(offset + sizeof(short), length);
        return Encoding.UTF8.GetString(buffer.ToArray());
    }

    public string ReadNullTerminatedString(int offset)
    {
        var value = 0;
        var length = 0;
        do
        {
            using (var accessor = _mmf.CreateViewAccessor(offset + length, 1))
            {
                value = accessor.ReadByte(0);
            }

            length++;
        } while (value != 0);

        // Subtract 1 because we don't want to include the null terminator in the final string
        Span<byte> buffer = ReadBytes(offset, length - 1);
        return Encoding.Default.GetString(buffer.ToArray());
    }

    #region String Pool

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GenerateStringPool(bool isMap)
    {
        using var accessor = _mmf.CreateViewAccessor();
        byte* data = null;
        accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref data);

        if (isMap)
        {
            // fucking maps wanting to be special...
            {
                var nodeOffset = _nodeBlockOffset;
                var firstChildId = ReadInt((int)nodeOffset + 4);
                var childCount = ReadShort((int)nodeOffset + 8);

                for (int i = firstChildId; i < firstChildId + childCount; i++)
                {
                    var childOffset = (int)_nodeBlockOffset + 20 * i;
                    var childNameOffset = ReadInt(childOffset);
                    var nodeChildId = ReadInt(childOffset + 4);
                    var nodeChildCount = ReadShort(childOffset + 8);
                    var stringOffset = Unsafe.Read<long>(data + _stringBlockOffset + (sizeof(long) * childNameOffset));
                    var childName = ReadString((int)stringOffset);

                    switch (childName)
                    {
                        case "Map":
                            for (var j = nodeChildId; j < nodeChildId + nodeChildCount; j++)
                            {
                                var dirOffset = (int)_nodeBlockOffset + 20 * j;
                                var dirChildId = ReadInt(dirOffset + 4);
                                var dirChildCount = ReadShort(dirOffset + 8);

                                // Eventually I wanna get rid of this shit but fuck it works so leave it
                                for (var k = dirChildId; k < dirChildId + dirChildCount; k++)
                                {
                                    var mapOffset = (int)_nodeBlockOffset + 20 * k;
                                    var mapNameOffset = ReadInt(mapOffset);
                                    var mapStringOffset =
                                        Unsafe.Read<long>(data + _stringBlockOffset + (sizeof(long) * mapNameOffset));
                                    var mapName = ReadString((int)mapStringOffset);
                                    _stringPool.Add($"Map/{mapName}", mapOffset);
                                }

                            }

                            break;
                        case "Tile":
                            for (var j = nodeChildId; j < nodeChildId + nodeChildCount; j++)
                            {
                                var offset = (int)_nodeBlockOffset + 20 * j;
                                var mapNameOffset = ReadInt(offset);
                                var mapNameStringOffset =
                                    Unsafe.Read<long>(data + _stringBlockOffset + (sizeof(long) * mapNameOffset));
                                var mapName = ReadString((int)mapNameStringOffset);
                                _stringPool.Add($"Map/Tile/{mapName}", offset);
                            }

                            break;
                        case "Obj":
                            for (var j = nodeChildId; j < nodeChildId + nodeChildCount; j++)
                            {
                                var offset = (int)_nodeBlockOffset + 20 * j;
                                var mapNameOffset = ReadInt(offset);
                                var mapNameStringOffset =
                                    Unsafe.Read<long>(data + _stringBlockOffset + (sizeof(long) * mapNameOffset));
                                var mapName = ReadString((int)mapNameStringOffset);
                                _stringPool.Add($"Map/Obj/{mapName}", offset);
                            }

                            break;
                        case "Back":
                            for (var j = nodeChildId; j < nodeChildId + nodeChildCount; j++)
                            {
                                var offset = (int)_nodeBlockOffset + 20 * j;
                                var mapNameOffset = ReadInt(offset);
                                var mapNameStringOffset =
                                    Unsafe.Read<long>(data + _stringBlockOffset + (sizeof(long) * mapNameOffset));
                                var mapName = ReadString((int)mapNameStringOffset);
                                _stringPool.Add($"Map/Back/{mapName}", offset);
                            }

                            break;
                        case "WorldMap":
                            for (var j = nodeChildId; j < nodeChildId + nodeChildCount; j++)
                            {
                                var offset = (int)_nodeBlockOffset + 20 * j;
                                var mapNameOffset = ReadInt(offset);
                                var mapNameStringOffset =
                                    Unsafe.Read<long>(data + _stringBlockOffset + (sizeof(long) * mapNameOffset));
                                var mapName = ReadString((int)mapNameStringOffset);

                                _stringPool.Add($"Map/{mapName}", offset);
                            }

                            break;
                        case "MapHelper.img":
                            _stringPool.Add($"Map/{childName}", childOffset);
                            break;
                        case "Effect.img":
                            _stringPool.Add($"Map/{childName}", childOffset);
                            break;
                        case "Physics.img":
                            _stringPool.Add($"Map/{childName}", childOffset);
                            break;
                    }
                }
            }
        }
        else
        {
            if (_fileName == "Character")
            { // now characters wants to be special tooo ffs
                var nodeOffset = _nodeBlockOffset;
                var firstChildId = ReadInt((int)nodeOffset + 4);
                var childCount = ReadShort((int)nodeOffset + 8);

                for (int i = firstChildId; i < firstChildId + childCount; i++)
                {
                    var childOffset = (int)_nodeBlockOffset + 20 * i;
                    var childNameOffset = ReadInt(childOffset);
                    var nodeChildId = ReadInt(childOffset + 4);
                    var nodeChildCount = ReadShort(childOffset + 8);
                    var stringOffset = Unsafe.Read<long>(data + _stringBlockOffset + (sizeof(long) * childNameOffset));
                    var childName = ReadString((int)stringOffset);
                    
                    if (childName.Contains(".img"))
                    {
                        _stringPool.Add($"{_fileName}/{childName}", (int)childOffset);
                        continue;
                    }

                    switch (childName)
                    {
                        case "Shield":
                        case "Shoe":
                        case "Helmet":
                        case "Pant":
                        case "Overall":
                        case "Face":
                        {
                            for (var j = nodeChildId; j < nodeChildId + nodeChildCount; j++)
                            {
                                var offset = (int)_nodeBlockOffset + 20 * i;
                                var nameOffset = ReadInt(childOffset);
                                var nameTableOffset = Unsafe.Read<long>(data + _stringBlockOffset + (sizeof(long) * nameOffset));
                                var nodeName = ReadString((int)nameTableOffset);

                                if (nodeName.Contains(".img"))
                                {
                                    _stringPool.Add($"{_fileName}/{nodeName}", (int)offset);
                                }
                            }
                        }
                            break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < _nodeCount; i++)
                {
                    {
                        var nodeOffset = _nodeBlockOffset + 20 * i;
                        var nameOffset = ReadInt((int)nodeOffset);
                        var rootStringOffset =
                            Unsafe.Read<long>(data + _stringBlockOffset + (sizeof(long) * nameOffset));
                        var name = ReadString((int)rootStringOffset);

                        if (name.Contains(".img"))
                        {
                            _stringPool.Add($"{_fileName}/{name}", (int)nodeOffset);
                        }
                    }
                }
            }
        }

        accessor.SafeMemoryMappedViewHandle.ReleasePointer();
    }

    #endregion

    public void Dispose()
    {
        _stringPool.Clear();
        _mmf?.Dispose();
    }
}