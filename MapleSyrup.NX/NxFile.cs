using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace MapleSyrup.NX;

public unsafe class NxFile 
{
    private FileData* fileData;

    //public NxHeader* header;
    //public NodeData* nodeTable;
    //public ulong* StringOffsetTable;
    //public ulong* BitmapOffsetTable;
    //public ulong* AudioOffsetTable;
    private NxNode root;

    public NxFile(string path)
    {
        OpenFile(path);
    }

    private void OpenFile(string path)
    {
        Console.WriteLine($"Loading NX file: {path}");
        var time = Stopwatch.StartNew();
        if (path == null)
            throw new ArgumentNullException(nameof(path));
        fixed (byte* fileHandle = File.ReadAllBytes(path))
        {
            if (fileHandle == null)
                throw new Exception("Failed to open file.");
            fileData = (FileData*)Marshal.AllocHGlobal(sizeof(FileData));
            fileData->fileBase = fileHandle;
            fileData->header = (NxHeader*)fileHandle;
            if (fileData->header->magic != 0x34474B50)
                throw new Exception("Invalid NX file.");
            fileData->nodeTable = (NodeData*)(fileHandle + fileData->header->nodeOffset);
            fileData->stringOffsetTable = (ulong*)(fileHandle + fileData->header->stringOffset);
            fileData->bitmapOffsetTable = (ulong*)(fileHandle + fileData->header->bitmapOffset);
            fileData->audioOffsetTable = (ulong*)(fileHandle + fileData->header->audioOffset);
        }

        //for (int i = 0; i < fileData->header->nodeCount; i++)
        // {
        //    //Console.WriteLine($"Node Count {i}: {nodeTable[i].childCount}");
        //   var nameLoc = (byte*)(fileHandle + fileData->stringOffsetTable[fileData->nodeTable[i].nodeName]);
        //   var nameData = new Span<byte>(nameLoc + 2, *(ushort*)nameLoc).ToArray();
        //   var name = Encoding.ASCII.GetString(nameData);
        //   Console.WriteLine($"Root node name: {name}");
        // }
        root = new(fileData->nodeTable, fileData);
        Console.WriteLine($"Loaded NX file in {time.ElapsedMilliseconds}ms.");

    }

    public NxNode Root => root;

    public NxNode this[string name] => root.GetChild(name);
    
    public NxNode ResolvePath(string path) => root.ResolvePath(path);

    public void Release()
    {
        Marshal.FreeHGlobal((IntPtr)fileData);
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 34)]
public unsafe struct NxHeader
{
    public uint magic;
    public uint nodeCount;
    public ulong nodeOffset;
    public uint stringCount;
    public ulong stringOffset;
    public uint bitmapCount;
    public ulong bitmapOffset;
    public uint audioCount;
    public ulong audioOffset;
}

public unsafe struct FileData
{
    public byte* fileBase;
    public NxHeader* header;
    public NodeData* nodeTable;
    public ulong* stringOffsetTable;
    public ulong* bitmapOffsetTable;
    public ulong* audioOffsetTable;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 14)]
public unsafe struct NodeData
{
    public uint nodeName;
    public uint firstChildId;
    public ushort childCount;
    public ushort nodeType;
    public byte* nodeData;
}

public enum NodeType : ushort
{
    NONE = 0,
    INT64,
    DOUBLE,
    STRING,
    VECTOR,
    BITMAP,
    AUDIO,
}