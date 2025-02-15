using System.Collections.ObjectModel;
using System.IO.MemoryMappedFiles;
using CommunityToolkit.HighPerformance;

namespace MapleSyrup.NX;

public class NXFile : IDisposable
{
    private readonly MemoryMappedFile _mmf;
    private readonly NXBuffer _buffer;
    private uint _nodeCount, _bitmapCount, _stringCount, _audioCount;
    private ulong _nodeBlock, _bitmapBlock, _stringBlock, _audioBlock;
    
    public NXFile(string path)
    {
        _mmf = MemoryMappedFile.CreateFromFile(path);
        _buffer = CreateBuffer();
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
    }

    /// <summary>
    /// Zero Recursion Node Acquisition. 
    /// </summary>
    /// <param name="nodePath"></param>
    /// <returns></returns>
    public NXNode? GetNode(string nodePath)
    {
        lock (_buffer)
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
                            Buffer = CreateBuffer(),
                        };
                }
            }
            return null;
        }
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
                    Buffer = CreateBuffer(),
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
                    Buffer = CreateBuffer(),
                };
            }
        }

        return null;
    }
    
    private NXBuffer CreateBuffer()
    {
        return new NXBuffer(_mmf)
        {
            NodeBlock = _nodeBlock,
            StringBlock = _stringBlock,
            BitmapBlock = _bitmapBlock,
            AudioBlock = _audioBlock,
        };
    }

    public void Dispose()
    {
        _buffer.Dispose();
        _mmf.Dispose();
        GC.SuppressFinalize(this);
    }
}