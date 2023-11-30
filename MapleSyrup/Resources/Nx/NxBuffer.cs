using System.Text;

namespace MapleSyrup.Resources.Nx;

public class NxBuffer
{
    private ReadOnlyMemory<byte> dataBlock;
    private int dataIndex = 0;
    private int endOfFile;

    public NxBuffer(ref byte[] fileData)
    {
        dataBlock = fileData;
        endOfFile = fileData.Length;
    }

    public ReadOnlySpan<byte> ReadBytes(int len, int offset)
    {
        if (len <= 0)
            return null;
        var span = dataBlock.Slice(offset, len).Span;
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

