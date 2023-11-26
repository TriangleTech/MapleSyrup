using System.Buffers;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Text;

namespace MapleSyrup.Resources.Nx;

public class NxFile : IDisposable
{
    private NxBuffer reader;
    private NxHeader header;
    private IMemoryOwner<NxNode> nodeData;
    private IMemoryOwner<long> stringOffsetTable;
    private IMemoryOwner<string> stringData;
    private IMemoryOwner<long> bitmapOffsetTable;
    private IMemoryOwner<long> audioOffSetTable;
    
    public NxNode BaseNode => nodeData.Memory.Span[0];

    /// <summary>
    /// Initializes the NX File 
    /// </summary>
    /// <param name="path">Path to the NX File</param>
    public NxFile(string path)
    {
        ParseFile(path);
    }

    /// <summary>
    /// Parse the NX File Nodes, Strings, and Offsets
    /// </summary>
    /// <param name="path">Path to the NX File</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ParseFile(string path)
    {
        byte[] data = File.ReadAllBytes(path);
        reader = new NxBuffer(data);

        var magic = reader.ReadInt();
        if (magic != 0x34474B50)
        {
            Console.WriteLine("Failed to obtain PKG4 magic");
            Environment.Exit(-1);
        }

        ParseHeader();
        stringOffsetTable = MemoryPool<long>.Shared.Rent(header.GetStringCount());
        bitmapOffsetTable = MemoryPool<long>.Shared.Rent(header.GetBitmapCount());
        audioOffSetTable = MemoryPool<long>.Shared.Rent(header.GetAudioCount());
        nodeData = MemoryPool<NxNode>.Shared.Rent(header.GetNodeCount());
        stringData = MemoryPool<string>.Shared.Rent(header.GetStringCount());
        ParseOffsetTables();
        ParseStrings();
        ParseNodes();
    }

    /// <summary>
    /// Parses the Header and populates its fields.
    /// </summary>
    private void ParseHeader()
    {
        int nodeCount = reader.ReadInt();
        long nodeOffset = reader.ReadLong();
        int stringCount = reader.ReadInt();
        long stringTableOffset = reader.ReadLong();
        int bitmapCount = reader.ReadInt();
        
        long bitmapTableOffset = 0;
        if (bitmapCount != 0)
            bitmapTableOffset = reader.ReadLong();

        int audioCount = reader.ReadInt();
        
        long audioTableOffset = 0;
        if (audioCount != 0)
            audioTableOffset = reader.ReadLong();

        header = new(nodeCount, nodeOffset, stringCount, stringTableOffset, 
            bitmapCount, bitmapTableOffset, audioCount, audioTableOffset);
    }

    /// <summary>
    /// Parses the Offset Tables for Strings, Bitmaps, and Audios(TODO)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ParseOffsetTables()
    {
        if (header.GetStringCount() > 0)
        {
            var strMem = stringOffsetTable.Memory;
            var strSpan = strMem.Span;
            reader.SetIndex((int)header.GetStringOffsetTableOffset());
            for (int i = 0; i < header.GetStringCount(); i++)
            {
                var offset = reader.ReadLong();
                strSpan[i] = offset;
            }
        }

        if (header.GetBitmapCount() > 0)
        {
            var bitMem = bitmapOffsetTable.Memory;
            var bitSpan = bitMem.Span;
            reader.SetIndex((int)header.GetBitmapOffsetTableOffset());
            for (int i = 0; i < header.GetBitmapCount(); i++)
            {
                var offset = reader.ReadLong();
                bitSpan[i] = offset;
            }
        }

        // TODO: Implement this when able, at this current time it throws an EOF error for all files.
        if (header.GetAudioCount() > 0 && header.GetAudioTableOffset() > 0)
        {
            var audMem = audioOffSetTable.Memory;
            var audSpan = audMem.Span;
            reader.SetIndex((int)header.GetAudioTableOffset());
            for (int i = 0; i < header.GetAudioCount(); i++)
            {
                var offset = reader.ReadLong();
                audSpan[i] = offset;
            }
        }
    }
    
    /// <summary>
    /// Parses all Strings within the file according to their offset
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ParseStrings()
    {
        var stringMemory = stringData.Memory;
        var stringSpan = stringMemory.Span;

        var offsetMemory = stringOffsetTable.Memory;
        var offsetSpan = offsetMemory.Span;
        
        for (int i = 0; i < header.GetStringCount(); i++)
        {
            reader.SetIndex((int)offsetSpan[i]);
            stringSpan[i] = reader.ReadString();
        }
    }

    /// <summary>
    /// Parses all nodes and assigns the Node Type.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ParseNodes()
    {
        var stringMemory = stringData.Memory;
        var stringSpan = stringMemory.Span;

        var nodeMemory = nodeData.Memory;
        var nodeSpan = nodeMemory.Span;

        var bitmapMemory = bitmapOffsetTable.Memory;
        var bitmapSpan = bitmapMemory.Span;
        
        for (int i = 0; i < header.GetNodeCount(); i++)
        {
            reader.SetIndex((int)header.GetNodeOffset() + (20 * i));
            var name = reader.ReadInt();
            var childId = reader.ReadInt();
            var count = reader.ReadShort();
            var type = (NodeType)reader.ReadShort();
            
            switch (type)
            {
                case NodeType.NoData:
                    nodeSpan[i] = new NxNode(stringSpan[name], childId, count, type);
                    break;
                case NodeType.Int64:
                    nodeSpan[i] = new NxIntNode(stringSpan[name], childId, count, type, (int)reader.ReadLong());
                    break;
                case NodeType.Double:
                    nodeSpan[i] = new NxDoubleNode(stringSpan[name], childId, count, type, (int)reader.ReadLong());
                    break;
                case NodeType.String:
                    nodeSpan[i] = new NxStringNode(stringSpan[name], childId, count, type, stringSpan[reader.ReadInt()]);
                    break;
                case NodeType.Vector:
                    nodeSpan[i] = new NxVectorNode(stringSpan[name], childId, count, type, reader.ReadInt(), reader.ReadInt());
                    break;
                case NodeType.Bitmap:
                    nodeSpan[i] = new NxBitmapNode(stringSpan[name], childId, count, type, 
                        bitmapSpan[reader.ReadInt()],reader.ReadShort(), reader.ReadShort(), reader);
                    break;
                case NodeType.Audio: // TODO: Implement Audio
                    nodeSpan[i] = new NxAudioNode(stringSpan[name], childId, count, type);
                    break;
            }
        }

        LinkChildren();
    }

    /// <summary>
    /// Links the Parent Node to their respective child nodes
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private void LinkChildren()
    {
        var nodeMemory = nodeData.Memory;
        var nodeSpan = nodeMemory.Span;
        
        for (int i = 0; i < header.GetNodeCount(); i++)
        {
            if (nodeSpan[i].ChildCount > 0)
                  nodeSpan[i].PopulateChildren(nodeSpan);
        }
    }
    
    /// <summary>
    /// Allows you to index through the Nodes using the Node Name.
    /// </summary>
    /// <param name="name"></param>
    public NxNode this[string name] => BaseNode[name];

    public void Dispose()
    {
        audioOffSetTable.Dispose();
        bitmapOffsetTable.Dispose();
        stringOffsetTable.Dispose();
        stringData.Dispose();
        nodeData.Dispose();
    }

    public enum LoadProtocol
    {
        MappedFile,
        BinaryMode
    }
}