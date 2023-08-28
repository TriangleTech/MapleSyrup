using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;

namespace MapleSharp.NX;

public class NxFile
{
    private MemoryMappedFile? nxMappedFile;
    private MemoryMappedViewStream? nxMappedStream;
    private LoadProtocol loadProtocol;
    private Buffer reader;
    private Header header;
    private Node[] nodeData;
    private long[] stringOffsetTable;
    private string[] stringData;
    private long[] bitmapOffsetTable;
    private long[] audioOffSetTable;
    
    public Node BaseNode => nodeData[0];

    /// <summary>
    /// Initializes the NX File 
    /// </summary>
    /// <param name="path">Path to the NX File</param>
    /// <param name="protocol">Load the file using File.ReadAllBytes or MemoryMappedFile</param>
    public NxFile(string path, LoadProtocol protocol = LoadProtocol.BinaryMode)
    {
        loadProtocol = protocol;
        ParseFile(path);
    }

    /// <summary>
    /// Parse the NX File Nodes, Strings, and Offsets
    /// </summary>
    /// <param name="path">Path to the NX File</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ParseFile(string path)
    {
        if (loadProtocol == LoadProtocol.MappedFile)
        {
            nxMappedFile = MemoryMappedFile.CreateFromFile(path);
            nxMappedStream = nxMappedFile.CreateViewStream();
            reader = new Buffer(nxMappedStream);
        }
        else if (loadProtocol == LoadProtocol.BinaryMode)
        {
            var data = File.ReadAllBytes(path);
            reader = new Buffer(data);
        }

        var magic = reader.ReadUInt();
        if (magic != 0x34474B50)
        {
            Console.WriteLine("Failed to obtain PKG4 magic");
            Environment.Exit(-1);
        }

        ParseHeader();
        stringOffsetTable = new long[header.GetStringCount()];
        bitmapOffsetTable = new long[header.GetBitmapCount()];
        audioOffSetTable = new long[header.GetAudioCount()];
        nodeData = new Node[header.GetNodeCount()];
        stringData = new string[header.GetStringCount()];
        ParseOffsetTables();
        ParseStrings();
        ParseNodes();
    }

    /// <summary>
    /// Parses the Header and populates its fields.
    /// </summary>
    private void ParseHeader()
    {
        uint nodeCount = reader.ReadUInt();
        ulong nodeOffset = reader.ReadULong();
        uint stringCount = reader.ReadUInt();
        ulong stringTableOffset = reader.ReadULong();
        uint bitmapCount = reader.ReadUInt();
        
        ulong bitmapTableOffset = 0;
        if (bitmapCount != 0)
            bitmapTableOffset = reader.ReadULong();

        uint audioCount = reader.ReadUInt();
        
        ulong audioTableOffset = 0;
        if (audioCount != 0)
            audioTableOffset = reader.ReadULong();

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
            reader.SetIndex((int)header.GetStringOffsetTableOffset());
            for (int i = 0; i < header.GetStringCount(); i++)
            {
                var offset = reader.ReadLong();
                stringOffsetTable[i] = offset;
            }
        }

        if (header.GetBitmapCount() > 0)
        {
            reader.SetIndex((int)header.GetBitmapOffsetTableOffset());
            for (int i = 0; i < header.GetBitmapCount(); i++)
            {
                var offset = reader.ReadLong();
                bitmapOffsetTable[i] = offset;
            }
        }

        // TODO: Implement this when able, at this current time it throws an EOF error for all files.
        if (header.GetAudioCount() > 0)
        {
            reader.SetIndex((int)header.GetAudioTableOffset());
            for (int i = 0; i < header.GetAudioCount(); i++)
            {
                var offset = reader.ReadLong();
                audioOffSetTable[i] = offset;
            }
        }
    }
    
    /// <summary>
    /// Parses all Strings within the file according to their offset
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ParseStrings()
    {
        for (int i = 0; i < header.GetStringCount(); i++)
        {
            reader.SetIndex((int)stringOffsetTable[i]);
            stringData[i] = reader.ReadMapleString();
        }
    }

    /// <summary>
    /// Parses all nodes and assigns the Node Type.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ParseNodes()
    {
        for (int i = 0; i < header.GetNodeCount(); i++)
        {
            reader.SetIndex((int)header.GetNodeOffset() + (20 * i));
            var name = reader.ReadInt();
            var childId = reader.ReadUInt();
            var count = reader.ReadUShort();
            var type = (NodeType)reader.ReadUShort();
            switch (type)
            {
                case NodeType.NoData:
                    nodeData[i] = new Node(stringData[name], childId, count, type);
                    break;
                case NodeType.Int64:
                    nodeData[i] = new IntNode(stringData[name], childId, count, type, (int)reader.ReadLong());
                    break;
                case NodeType.Double:
                    nodeData[i] = new DoubleNode(stringData[name], childId, count, type, (int)reader.ReadLong());
                    break;
                case NodeType.String:
                    nodeData[i] = new StringNode(stringData[name], childId, count, type, stringData[reader.ReadInt()]);
                    break;
                case NodeType.Vector:
                    nodeData[i] = new VectorNode(stringData[name], childId, count, type, reader.ReadInt(), reader.ReadInt());
                    break;
                case NodeType.Bitmap:
                    nodeData[i] = new BitmapNode(stringData[name], childId, count, type, 
                        bitmapOffsetTable[reader.ReadInt()],reader.ReadShort(), reader.ReadShort(), reader);
                    break;
                case NodeType.Audio: // TODO: Implement Audio
                    nodeData[i] = new AudioNode(stringData[name], childId, count, type);
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
        for (int i = 0; i < header.GetNodeCount(); i++)
        {
            if (nodeData[i].ChildCount > 0)
                nodeData[i].PopulateChildren(nodeData);
        }
    }
    
    /// <summary>
    /// Allows you to index through the Nodes using the Node Name.
    /// </summary>
    /// <param name="name"></param>
    public Node this[string name] => BaseNode[name];
    
    public Node ResolvePath(string path)
    {
        var node = BaseNode;
        var split = path.Split('/');
        foreach (var name in split)
        {
            if (node.HasChild(name))
                node = node[name];
            else
                return null;
        }

        return node;
    }
    
    public Image<Bgra32> GetImage(string path)
    {
        var node = ResolvePath(path);
        if (node == null)
            throw new Exception("Node does not exist.");
        return node.To<BitmapNode>().GetBitmap();
    }
    
    public void Dispose()
    {
        if (nxMappedStream != null)
            nxMappedStream.Dispose();
        if (nxMappedFile != null)
            nxMappedFile.Dispose();
    }

    public enum LoadProtocol
    {
        MappedFile,
        BinaryMode
    }
}