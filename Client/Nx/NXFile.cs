using System.Collections.ObjectModel;
using System.IO.MemoryMappedFiles;
using System.Text;
using CommunityToolkit.HighPerformance;

namespace Client.Nx;

public class NXFile : IDisposable
{
    private readonly MemoryMappedFile _mmf;
    private readonly NXBuffer _buffer;
    private uint _nodeCount, _bitmapCount, _stringCount, _audioCount;
    private ulong _nodeBlock, _bitmapBlock, _stringBlock, _audioBlock;
    
    public uint NodeCount => _nodeCount;
    public ulong NodeBlock => _nodeBlock;
    public uint StringCount => _stringCount;
    public ulong StringBlock => _stringBlock;
    public uint BitmapCount => _bitmapCount;
    public ulong BitmapBlock => _bitmapBlock;
    public uint AudioCount => _audioCount;
    public ulong AudioBlock => _audioBlock;
    
    public NXBuffer Buffer => _buffer;
    
    public MemoryMappedFile Handle => _mmf; // I know I can make this an auto-property, but I'm lazy
    
    public NXFile(string path)
    {
        _mmf = MemoryMappedFile.CreateFromFile(path);
        
        _buffer = new NXBuffer(_mmf);
        var magic = _buffer.ReadUInt32();
        if (magic != 0x34474B50)
        {
            throw new Exception("Invalid Nx file");
        }
        
        _nodeCount = _buffer.ReadUInt32();
        _nodeBlock = _buffer.ReadUInt64();
        _stringCount = _buffer.ReadUInt32();
        _stringBlock = _buffer.ReadUInt64();
        _bitmapCount = _buffer.ReadUInt32();
        if (_bitmapCount > 0) 
            _bitmapBlock = _buffer.ReadUInt64();
        _audioCount = _buffer.ReadUInt32();
        if (_audioCount > 0) 
            _audioBlock = _buffer.ReadUInt64();

        _buffer.NodeBlock = _nodeBlock;
        _buffer.StringBlock = _stringBlock;
        _buffer.BitmapBlock = _bitmapBlock;
        _buffer.AudioBlock = _audioBlock;

        //Console.WriteLine($"Node count: {_nodeCount}");
        //Console.WriteLine($"Root node: {_nodeBlock}");
        //Console.WriteLine($"String count: {_stringCount}");
        //Console.WriteLine($"String block: {_stringBlock}");
        //Console.WriteLine($"Bitmap count: {_bitmapCount}");
        //Console.WriteLine($"Bitmap block: {_bitmapBlock}");
        //Console.WriteLine($"Audio count: {_audioCount}");
        //Console.WriteLine($"Audio block: {_audioBlock}");
    }

    /// <summary>
    /// Zero Recursion Node Acquisition. 
    /// </summary>
    /// <param name="nodePath"></param>
    /// <returns></returns>
    public NXNode? GetNode(string nodePath)
    {
        //Console.WriteLine($"Getting node: {nodePath}");
        // Get the individual nodes
        var path = nodePath.Split('/');
        var pathCount = 0;
        
        ulong offset = 0;
        var maxOffset = _nodeBlock + 20 * _nodeCount;
        ulong nodeStart = 1;
        ulong count = 0;

        while (offset < maxOffset)
        {
            offset = _nodeBlock + 20 * (nodeStart + count);
            _buffer.Seek((long)offset);
            var nameOffset = _buffer.ReadUInt32();
            var firstChildId = _buffer.ReadUInt32();
            var childCount = _buffer.ReadUInt16();
            var nodeType = (NodeType)_buffer.ReadUInt16();

            _buffer.Seek((long)(_stringBlock + 8 * nameOffset));
            var stringOffset = _buffer.ReadUInt64();

            _buffer.Seek((long)(stringOffset)); 
            var nodeName = _buffer.ReadString();

            if (path[pathCount] != nodeName)
            {
                count++;
                continue;
            }

            count = 0;
            nodeStart = firstChildId;
            pathCount++;
            /*
            Console.WriteLine($"---------------------------------------");
            Console.WriteLine($"Node: {nodeName}");
            Console.WriteLine($"Path Count: {pathCount}");
            Console.WriteLine($"Path: {nodePath}");
            Console.WriteLine($"Offset: {offset}");
            Console.WriteLine($"First Child Id: {firstChildId}");
            Console.WriteLine($"Child Count: {childCount}");
            Console.WriteLine($"Node Type: {nodeType}");
            Console.WriteLine($"String Offset: {stringOffset}");
            Console.WriteLine($"String Name: {nodeName}");
            Console.WriteLine($"---------------------------------------");*/

            if (pathCount == path.Length)
            {
                return new NXNode
                    { 
                        NodePath = nodePath, 
                        Name = nodeName, 
                        FirstChildId = firstChildId, 
                        ChildCount = childCount, 
                        Type = nodeType, 
                        Offset = offset,
                        Buffer = _buffer,
                    };
            }
        }

        return null;
    }

    public NXNode? GetChildNode(NXNode node, string childName)
    {
        if (node.ChildCount == 0) return null;
        
        for (var i = node.FirstChildId; i < node.FirstChildId + node.ChildCount; i++)
        {
            var offset = _nodeBlock + 20 * i;
            _buffer.Seek((long)offset);
            var nameOffset = _buffer.ReadUInt32();
            var firstChildId = _buffer.ReadUInt32();
            var childCount = _buffer.ReadUInt16();
            var nodeType = (NodeType)_buffer.ReadUInt16();

            _buffer.Seek((long)(_stringBlock + 8 * nameOffset));
            var stringOffset = _buffer.ReadUInt64();

            _buffer.Seek((long)(stringOffset)); 
            var nodeName = _buffer.ReadString();
            /*
            Console.WriteLine($"------------------CHILD NODE---------------------");
            Console.WriteLine($"Node: {nodeName}");
            Console.WriteLine($"Offset: {offset}");
            Console.WriteLine($"First Child Id: {firstChildId}");
            Console.WriteLine($"Child Count: {childCount}");
            Console.WriteLine($"Node Type: {nodeType}");
            Console.WriteLine($"String Offset: {stringOffset}");
            Console.WriteLine($"String Name: {nodeName}");
            Console.WriteLine($"---------------------------------------");
            */
            if (nodeName == childName)
            {
                return new NXNode
                {
                    NodePath = string.Concat(node.NodePath, $"/{nodeName}"), 
                    Name = nodeName, 
                    FirstChildId = firstChildId, 
                    ChildCount = childCount, 
                    Type = nodeType, 
                    Offset = offset,
                    Buffer = _buffer,
                };
            }
        }

        return null;
    }

    /// <summary>
    /// Acquire a node without verifying its origin. Only use with '.img' nodes. (NON-MAP nodes) EXCEPT for MAP IMGS (100000000.img is unique)
    /// </summary>
    /// <param name="imgNode"></param>
    /// <returns></returns>
    public NXNode? GetFastImg(string imgNode)
    {
        for (var i = 0; i < _nodeCount; i++)
        {
            var offset = (uint)_nodeBlock + 20 * i;
            
            _buffer.Seek((long)offset);
            var nameOffset = _buffer.ReadUInt32();
            var firstChildId = _buffer.ReadUInt32();
            var childCount = _buffer.ReadUInt16();
            var nodeType = (NodeType)_buffer.ReadUInt16();

            _buffer.Seek((long)(_stringBlock + 8 * nameOffset));
            var stringOffset = _buffer.ReadUInt64();

            _buffer.Seek((long)(stringOffset));
            var nodeName = _buffer.ReadString();
            if (nodeName == imgNode)
            {
                return new NXNode
                {
                    NodePath = string.Empty,
                    Name = nodeName,
                    FirstChildId = firstChildId,
                    ChildCount = childCount,
                    Type = nodeType,
                    Offset = (ulong)offset,
                    Buffer = _buffer,
                };
            }
        }

        return null;
    }

    public bool HasNode(NXNode node, string nodeName)
    {
        return GetChildNode(node, nodeName) != null;
    }

    /// <summary>
    /// Gets the children of the provided <see cref="NXNode"/>.
    /// </summary>
    /// <param name="node">The <see cref="NXNode"/> to parse.</param>
    /// <returns>A dictionary of <see cref="NXNode"/></returns>
    public ReadOnlyDictionary<string, NXNode> GetChildren(NXNode node)
    {
        var nodes = new Dictionary<string, NXNode>(node.ChildCount);
        if (node.ChildCount == 0) return nodes.AsReadOnly();

        for (var i = node.FirstChildId; i < node.FirstChildId + node.ChildCount; i++)
        {
            var offset = _nodeBlock + 20 * i;
            _buffer.Seek((long)offset);
            var nameOffset = _buffer.ReadUInt32();
            var firstChildId = _buffer.ReadUInt32();
            var childCount = _buffer.ReadUInt16();
            var nodeType = (NodeType)_buffer.ReadUInt16();

            _buffer.Seek((long)(_stringBlock + 8 * nameOffset));
            var stringOffset = _buffer.ReadUInt64();

            _buffer.Seek((long)(stringOffset));
            var nodeName = _buffer.ReadString();
            
            nodes.Add(nodeName, new NXNode
            {
                NodePath = string.Concat(node.NodePath, $"/{nodeName}"),
                Name = nodeName,
                FirstChildId = firstChildId,
                ChildCount = childCount,
                Type = nodeType,
                Offset = offset,
                Buffer = _buffer,
            });
        }

        return nodes.AsReadOnly();
    }

    /// <summary>
    /// Gets the names of the children contained in the <see cref="NXNode"/>
    /// </summary>
    /// <param name="node">The <see cref="NXNode"/> to parse</param>
    /// <returns>A span of strings containing the names.</returns>
    public Span<string> GetChildrenNames(NXNode node)
    {
        if (node.ChildCount == 0) return Array.Empty<string>();
        var nodes = new List<string>();
        
        for (var i = node.FirstChildId; i < node.FirstChildId + node.ChildCount; i++)
        {
            var offset = _nodeBlock + 20 * i;
            _buffer.Seek((long)offset);
            var nameOffset = _buffer.ReadUInt32();
            var firstChildId = _buffer.ReadUInt32();
            var childCount = _buffer.ReadUInt16();
            var nodeType = (NodeType)_buffer.ReadUInt16();

            _buffer.Seek((long)(_stringBlock + 8 * nameOffset));
            var stringOffset = _buffer.ReadUInt64();

            _buffer.Seek((long)(stringOffset));
            var nodeName = _buffer.ReadString();
            nodes.Add(nodeName);
        }
        
        return nodes.AsSpan();
    }

    public void Dispose()
    {
        _buffer.Dispose();
        _mmf.Dispose();
        GC.SuppressFinalize(this);
    }
}