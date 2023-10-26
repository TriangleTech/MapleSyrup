using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using K4os.Compression.LZ4;

namespace MapleSyrup.NX;

public unsafe class NxNode
{
    private NodeData* nodeData;
    private FileData* fileData;
    private string nodeName;
    private uint childCount;
    private uint firstChildId;
    private NodeType nodeType;
    private uint bitmapIndex;
    private ushort bitmapWidth;
    private ushort bitmapHeight;
    private uint intData;
    private string stringData;
    private double doubleData;
    
    public NxNode(NodeData* nodeData, FileData* fileData)
    {
        this.nodeData = nodeData;
        this.fileData = fileData;
        
        firstChildId = nodeData->firstChildId;
        ChildCount = nodeData->childCount;
        NodeType = (NodeType)nodeData->nodeType;
    }

    public string Name
    {
        get
        {
            if (nodeName == string.Empty)
            {
                var nameLoc = (byte*)(fileData->fileBase + fileData->stringOffsetTable[nodeData->nodeName]);
                var nameData = new Span<byte>(nameLoc + 2, *(ushort*)nameLoc).ToArray();
                nodeName = Encoding.ASCII.GetString(nameData);
                return nodeName;
            }
            
            return nodeName;
        }
        internal set => nodeName = value;
    }
    
    public uint ChildCount
    {
        get => childCount;
        internal set => childCount = value;
    }
    
    public NodeType NodeType
    {
        get => nodeType;
        internal set => nodeType = value;
    }
    
    public uint BitmapIndex
    {
        get => bitmapIndex;
        internal set => bitmapIndex = value;
    }
    
    public ushort BitmapWidth
    {
        get => bitmapWidth;
        internal set => bitmapWidth = value;
    }
    
    public ushort BitmapHeight
    {
        get => bitmapHeight;
        internal set => bitmapHeight = value;
    }
    
    public uint IntData
    {
        get => intData;
        internal set => intData = value;
    }
    
    public string StringData
    {
        get => stringData;
        internal set => stringData = value;
    }
    
    public double DoubleData
    {
        get => doubleData;
        internal set => doubleData = value;
    }
    
    public NxNode this[string name] => GetChild(name);

    public NxNode GetChild(string name)
    {
        if (fileData == null)
            throw new Exception("File data is null.");
        //var parentNode = nodeData;
        var count = childCount;

        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                var childNode = fileData->nodeTable + firstChildId + i;
                var nameLoc = (byte*)(fileData->fileBase + fileData->stringOffsetTable[childNode->nodeName]);
                var nameData = new Span<byte>(nameLoc + 2, *(ushort*)nameLoc).ToArray();
                var childName = Encoding.ASCII.GetString(nameData);
                if (childName == name)
                {
                    var nextNode = new NxNode(childNode, fileData);
                    nextNode.Name = childName;
                    nextNode.ChildCount = childNode->childCount;
                    nextNode.NodeType = (NodeType)childNode->nodeType;

                    switch ((NodeType)childNode->nodeType)
                    {
                        case NodeType.NONE:
                            break;
                        case NodeType.INT64:
                            nextNode.IntData = Unsafe.Read<uint>(nextNode.nodeData);
                            break;
                        case NodeType.STRING:
                            // TODO: Do this
                            break;
                        case NodeType.DOUBLE:
                            nextNode.DoubleData = Unsafe.Read<double>(nextNode.nodeData);
                            break;
                        case NodeType.VECTOR:
                            break;
                        case NodeType.BITMAP:
                            nextNode.BitmapIndex = Unsafe.Read<uint>(nextNode.nodeData);
                            nextNode.BitmapWidth = Unsafe.Read<ushort>(nextNode.nodeData + 4);
                            nextNode.BitmapHeight = Unsafe.Read<ushort>(nextNode.nodeData + 6);
                            break;
                        case NodeType.AUDIO:
                            break;
                    }

                    return nextNode;
                }
            }
        }

        throw new NullReferenceException("Node not found.");
    }

    public NxNode ResolvePath(string path)
    {
        if (fileData == null)
            throw new Exception("File data is null.");
        var splitPath = path.Split('/');
        var currentNode = this;
        NxNode nextNode = null;
        
        for (int i = 0; i < splitPath.Length; i++)
        {
            nextNode = currentNode.GetChild(splitPath[i]);
            currentNode = nextNode;
        }
        
        return currentNode;
    }

    public int GetInt()
    {
        return *(int*)nodeData->nodeData;
    }
    
    public string GetString()
    {
        throw new NotImplementedException();
    }
    
    public Image<Bgra32> GetImage()
    {
        if (nodeData->nodeType != (uint)NodeType.BITMAP)
            throw new Exception("Node is not a bitmap.");
        
        var bitmapLocation = fileData->fileBase + fileData->bitmapOffsetTable[BitmapIndex];
        var bitmapLength = Unsafe.Read<uint>(bitmapLocation);
        var compressedBitmap = new Span<byte>(bitmapLocation + 4, (int)bitmapLength).ToArray();
        var uncompressedBitmap = new byte[bitmapWidth * bitmapHeight * 4];
        
        LZ4Codec.Decode(compressedBitmap, 0, (int)bitmapLength, uncompressedBitmap, 0, bitmapWidth * bitmapHeight * 4);
        var image = Image.LoadPixelData<Bgra32>(uncompressedBitmap, bitmapWidth, bitmapHeight);
        return image;
    }
    
    public void GetAudio()
    {
        throw new NotImplementedException();
    }
    
    public Vector2 GetVector()
    {
        return new Vector2(*(float*)nodeData->nodeData, *(float*)(nodeData->nodeData + 4));
    }
}