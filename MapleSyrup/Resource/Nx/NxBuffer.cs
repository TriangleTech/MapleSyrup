using System.IO.MemoryMappedFiles;
using System.Text;

namespace MapleSyrup.Resource.Nx;

public class NxBuffer
{
    private int dataIndex = 0;
    private int endOfFile;
    private readonly MemoryMappedViewStream stream;

    public NxBuffer(ref MemoryMappedViewStream viewStream)
    {
        stream = viewStream;
        endOfFile = (int) stream.Capacity;
    }

    public ReadOnlySpan<byte> ReadBytes(int len, int offset)
    {
        var span = new byte[len];
        stream.Seek(offset, SeekOrigin.Begin);
        _ = stream.Read(span, 0, len);
        dataIndex += len;
        
        return span;
    }

    public byte ReadByte()
    {
        if (dataIndex >= endOfFile)
            throw new Exception("Reached end of file");

        var data = ReadBytes(1, dataIndex);

        return data[0];
    }

    public short ReadShort()
    {
        if (dataIndex >= endOfFile)
            throw new Exception("Reached end of file");

        var data = BitConverter.ToInt16(ReadBytes(2, dataIndex));

        return data;
    }

    public int ReadInt()
    {
        if (dataIndex >= endOfFile)
            throw new Exception("Reached end of file");

        var data = BitConverter.ToInt32(ReadBytes(4, dataIndex));

        return data;
    }

    public long ReadLong()
    {
        if (dataIndex >= endOfFile)
            throw new Exception("Reached end of file");

        var data = BitConverter.ToInt64(ReadBytes(8, dataIndex));

        return data;
    }

    public string ReadString()
    {
        if (dataIndex >= endOfFile)
            throw new Exception("Reached end of file");
        var stringLength = ReadShort();
        var stringData = ReadBytes(stringLength, dataIndex);

        return Encoding.UTF8.GetString(stringData);
    }

    public void SetIndex(int newIndex)
    {
        if (dataIndex >= endOfFile)
            throw new Exception("Reached end of file");
        dataIndex = newIndex;
    }
}

