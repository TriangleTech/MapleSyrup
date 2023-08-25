using System.IO.MemoryMappedFiles;
using System.Text;

namespace MapleSharp.NX;

public class Buffer : IDisposable
{
    private byte[] buffer;
    private int index;
    private int endOfFile;
    private MemoryStream stream;

    /// <summary>
    /// Populate the buffer using a MemoryMappedViewStream
    /// </summary>
    /// <param name="viewStream"></param>
    public Buffer(MemoryMappedViewStream viewStream)
    {
        stream = new MemoryStream();
        viewStream.CopyTo(stream);
        stream.Dispose();
        buffer = stream.ToArray();
        index = 0;
        endOfFile = buffer.Length - 1;
    }

    /// <summary>
    /// Populate the buffer using raw data
    /// </summary>
    /// <param name="data"></param>
    public Buffer(byte[] data)
    {
        buffer = data;
        index = 0;
        endOfFile = data.Length - 1;
    }

    /// <summary>
    /// Reads a byte from the buffer and returns the value
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public byte ReadByte()
    {
        if (index >= endOfFile)
            throw new Exception("Attempted to read past end of file");
        
        var val = buffer[index];
        index += sizeof(byte);

        return val;
    }
    
    /// <summary>
    /// Reads a byte from a specific point the buffer and returns the value
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public byte ReadByte(int offset)
    {
        if (index >= endOfFile)
            throw new Exception("Attempted to read past end of file");
        if (offset > endOfFile || offset < 0)
            throw new Exception("Attempted to read outside of buffer.");
        
        var val = buffer[offset];
        index += sizeof(byte);

        return val;
    }

    /// <summary>
    /// Reads a set amount bytes from the buffer and returns the array
    /// </summary>
    /// <param name="len"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public byte[] ReadBytes(int len)
    {
        if (index >= endOfFile)
            throw new Exception("Attempted to read past end of file");

        var data = buffer.Skip(index).Take(len).ToArray();
        index += data.Length;

        return data;
    }
    
    /// <summary>
    /// Reads a set a bytes from a specific point in the buffer and returns the array
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="len"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public byte[] ReadBytes(int offset, int len)
    {
        if (index >= endOfFile)
            throw new Exception("Attempted to read past end of file");
        if (offset > endOfFile || offset < 0)
            throw new Exception("Attempted to read outside of buffer.");
        
        var data = buffer.Skip(offset).Take(len).ToArray();
        index += data.Length;

        return data;
    }

    /// <summary>
    /// Reads a 2-byte value from the buffer and returns the value
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public short ReadShort()
    {
        if (index >= endOfFile)
            throw new Exception("Attempted to read past end of file");

        var data = buffer.Skip(index).Take(sizeof(short)).ToArray();
        var val = BitConverter.ToInt16(data);
        index += sizeof(short);

        return val;
    }
    
    /// <summary>
    /// Reads a 2-byte value from a specific point in the buffer and returns the value
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public short ReadShort(int offset)
    {
        if (index >= endOfFile)
            throw new Exception("Attempted to read past end of file");
        if (offset > endOfFile || offset < 0)
            throw new Exception("Attempted to read outside of buffer.");

        var data = buffer.Skip(offset).Take(sizeof(short)).ToArray();
        var val = BitConverter.ToInt16(data);
        index += sizeof(short);

        return val;
    }

    /// <summary>
    /// Reads a unsigned 2-byte value from the buffer and returns the value
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public ushort ReadUShort()
    {
        if (index >= endOfFile)
            throw new Exception("Attempted to read past end of file");

        var data = buffer.Skip(index).Take(sizeof(ushort)).ToArray();
        var val = BitConverter.ToUInt16(data);
        index += sizeof(ushort);

        return val;
    }
    
    /// <summary>
    /// Reads an unsigned 2-byte value value from a specific point in the buffer and returns the value
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public ushort ReadUShort(int offset)
    {
        if (index >= endOfFile)
            throw new Exception("Attempted to read past end of file");
        if (offset > endOfFile || offset < 0)
            throw new Exception("Attempted to read outside of buffer.");

        var data = buffer.Skip(offset).Take(sizeof(ushort)).ToArray();
        var val = BitConverter.ToUInt16(data);
        index += sizeof(ushort);

        return val;
    }

    /// <summary>
    /// Reads a 4-byte value from the buffer and returns the value
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public int ReadInt()
    {
        if (index >= endOfFile)
            throw new Exception("Attempted to read past end of file");

        var data = buffer.Skip(index).Take(sizeof(int)).ToArray();
        var val = BitConverter.ToInt32(data);
        index += sizeof(int);

        return val;
    }
    
    /// <summary>
    /// Reads a 4-byte value from the buffer at a specific point and returns the value.
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public int ReadInt(int offset)
    {
        if (index >= endOfFile)
            throw new Exception("Attempted to read past end of file");
        if (offset > endOfFile || offset < 0)
            throw new Exception("Attempted to read outside of buffer.");

        var data = buffer.Skip(offset).Take(sizeof(int)).ToArray();
        var val = BitConverter.ToInt32(data);
        index += sizeof(int);

        return val;
    }
    
    /// <summary>
    /// Reads an unsigned 4-byte value from the buffer and returns the value.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public uint ReadUInt()
    {
        if (index >= endOfFile)
            throw new Exception("Attempted to read past end of file");

        var data = buffer.Skip(index).Take(sizeof(uint)).ToArray();
        var val = BitConverter.ToUInt32(data);
        index += sizeof(uint);

        return val;
    }
    
    /// <summary>
    /// Reads an unsigned 4-byte value from the buffer at a specific point and returns the value.
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public uint ReadUInt(int offset)
    {
        if (index >= endOfFile)
            throw new Exception("Attempted to read past end of file");
        if (offset > endOfFile || offset < 0)
            throw new Exception("Attempted to read outside of buffer.");

        var data = buffer.Skip(offset).Take(sizeof(uint)).ToArray();
        var val = BitConverter.ToUInt32(data);
        index += sizeof(uint);

        return val;
    }

    /// <summary>
    /// Reads an 8-byte value from the buffer and returns the value.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public long ReadLong()
    {
        if (index >= endOfFile)
            throw new Exception("Attempted to read past end of file");

        var data = buffer.Skip(index).Take(sizeof(long)).ToArray();
        var val = BitConverter.ToInt64(data);
        index += sizeof(long);

        return val;
    }
    
    /// <summary>
    /// Reads an 8-byte value from the buffer at a specific point and returns the value.
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public long ReadLong(int offset)
    {
        if (index >= endOfFile)
            throw new Exception("Attempted to read past end of file");
        if (offset > endOfFile || offset < 0)
            throw new Exception("Attempted to read outside of buffer.");

        var data = buffer.Skip(offset).Take(sizeof(long)).ToArray();
        var val = BitConverter.ToInt64(data);
        index += sizeof(long);

        return val;
    }
    
    /// <summary>
    /// Reads an unsigned 8-byte value from the buffer and returns the value.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public ulong ReadULong()
    {
        if (index >= endOfFile)
            throw new Exception("Attempted to read past end of file");

        var data = buffer.Skip(index).Take(sizeof(ulong)).ToArray();
        var val = BitConverter.ToUInt64(data);
        index += sizeof(ulong);

        return val;
    }
    
    /// <summary>
    /// Reads an unsigned 8-byte value from the buffer at a specific point and returns the value.
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public ulong ReadULong(int offset)
    {
        if (index >= endOfFile)
            throw new Exception("Attempted to read past end of file");
        if (offset > endOfFile || offset < 0)
            throw new Exception("Attempted to read outside of buffer.");

        var data = buffer.Skip(offset).Take(sizeof(ulong)).ToArray();
        var val = BitConverter.ToUInt64(data);
        index += sizeof(ulong);

        return val;
    }

    /// <summary>
    /// Reads an 8-byte double value from the buffer and returns the value.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public double ReadDouble()
    {
        if (index >= endOfFile)
            throw new Exception("Attempted to read past end of file");
        var data = buffer.Skip(index).Take(sizeof(double)).ToArray();
        var val = BitConverter.ToDouble(data);
        index += sizeof(double);

        return val;
    }
    
    /// <summary>
    /// Reads an 8-byte double value from the buffer at a specific point and returns the value.
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public double ReadDouble(int offset)
    {
        if (index >= endOfFile)
            throw new Exception("Attempted to read past end of file");
        if (offset > endOfFile || offset < 0)
            throw new Exception("Attempted to read outside of buffer.");

        var data = buffer.Skip(offset).Take(sizeof(double)).ToArray();
        var val = BitConverter.ToDouble(data);
        index += sizeof(double);

        return val;
    }

    /// <summary>
    /// Reads an 8-byte float value from the buffer and returns the value.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public float ReadFloat()
    {
        if (index >= endOfFile)
            throw new Exception("Attempted to read past end of file");
        var data = buffer.Skip(index).Take(sizeof(float)).ToArray();
        var val = BitConverter.ToSingle(data);
        index += sizeof(float);

        return val;
    }
    
    /// <summary>
    /// Reads an 8-byte float value from the buffer at a specific point and returns the value.
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public float ReadFloat(int offset)
    {
        if (index >= endOfFile)
            throw new Exception("Attempted to read past end of file");
        if (offset > endOfFile || offset < 0)
            throw new Exception("Attempted to read outside of buffer.");

        var data = buffer.Skip(offset).Take(sizeof(float)).ToArray();
        var val = BitConverter.ToSingle(data);
        index += sizeof(float);

        return val;
    }

    /// <summary>
    /// Read a string with a set length and returns the value.
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public string ReadString(int length)
    {
        if (index >= endOfFile)
            throw new Exception("Attempted to read past end of file");

        var data = ReadBytes(length);

        return Encoding.UTF8.GetString(data);
    }
    
    /// <summary>
    /// Reads a string with a set length at a specific point in the buffer and returns the value.
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public string ReadString(int offset, int length)
    {
        if (index >= endOfFile)
            throw new Exception("Attempted to read past end of file");
        if (offset > endOfFile || offset < 0)
            throw new Exception("Attempted to read outside of buffer.");

        var data = ReadBytes(offset,length);
        return Encoding.UTF8.GetString(data);
    }

    /// <summary>
    /// Reads a string with a 2-byte length value and returns the value.
    /// </summary>
    /// <returns></returns>
    public string ReadMapleString()
    {
        var length = ReadUShort();
        return ReadString(length);
    }
    
    /// <summary>
    /// Reads a string with a 2-byte length value at a specific point and returns the value.
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    public string ReadMapleString(int offset)
    {
        var length = ReadUShort(offset);
        return ReadString(length);
    }

    /// <summary>
    /// Gets the length of the buffer
    /// </summary>
    /// <returns></returns>
    public int GetLength() => buffer.Length;
    
    /// <summary>
    /// Gets the current index of the buffer
    /// </summary>
    /// <returns></returns>
    public int GetIndex() => index;

    /// <summary>
    /// Sets the buffer index to a specific position.
    /// </summary>
    /// <param name="offset"></param>
    /// <exception cref="Exception"></exception>
    public void SetIndex(int offset)
    {
        if (offset > endOfFile || offset < 0)
            throw new Exception("Attempted to Set Index past end of file");
        index = offset;
    }

    /// <summary>
    /// Skips a specific amount of bytes 
    /// </summary>
    /// <param name="offset"></param>
    /// <exception cref="Exception"></exception>
    public void Skip(int offset)
    {
        if (index > endOfFile || index < 0)
            throw new Exception("Attempted to increment index past end of file");
        index += offset;
    }

    public void Dispose()
    {
        buffer = null;
    }
}