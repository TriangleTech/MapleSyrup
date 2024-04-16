using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Nx;

public class NxFile : IDisposable
{
    private NxReader _reader;
    private string _fileName;
    private NxNode _root;
    private int _parsedCount = 0;

    public NxFile(string path, GraphicsDevice device)
    {
        _reader = new NxReader(path);
        _fileName = Path.GetFileNameWithoutExtension(path);
    }

    public NxNode this[string name] => _root[name];

    public NxNode GetNode(string node)
    {
        if (!_reader.StringPool.TryGetValue($"{_reader.File}/{node}", out var offset))
            throw new NullReferenceException();
        
        //var nameOffset = _reader.ReadInt(offset);
        //var firstChildId = _reader.ReadInt(offset + 4);
        //var childCount = _reader.ReadShort(offset + 8);
        //var nodeType = _reader.ReadShort(offset + 10);
        //var stringOffset = _reader.ReadLong(_reader.StringBlockOffset + (sizeof(long) * nameOffset));
        //var name = _reader.ReadString((int)stringOffset);
        var info = ProcessNodeInfo(offset);
        var parent = new NxNode(ref _reader, info.nodeType, info.name, offset);
        ParseChildren(parent, info.firstChildId, info.childCount);

        return parent;

    }

    private void ParseChildren(NxNode node, int firstChildId, int childCount)
    {
        for (var i = firstChildId; i < firstChildId + childCount; i++)
        {
            var nodeOffset = _reader.NodeBlockOffset + 20 * i;
            var info = ProcessNodeInfo(nodeOffset);
            //var nameOffset = _reader.ReadInt(nodeOffset);
            //var nodeChildId = _reader.ReadInt(nodeOffset + 4);
            //var nodeChildCount = _reader.ReadShort(nodeOffset + 8);
            //var nodeType = _reader.ReadShort(nodeOffset + 10);
            //var stringOffset = _reader.ReadLong(_reader.StringBlockOffset + (sizeof(long) * nameOffset));
            //var name = _reader.ReadString((int)stringOffset);
            
            var childNode = new NxNode(ref _reader, info.nodeType, info.name, nodeOffset);
            if (info.childCount > 0)
            {
                ParseChildren(childNode, info.firstChildId, info.childCount);
                node.Children.Add(childNode);
            }
            else
            {
                node.Children.Add(childNode);
            }
        }
    }

    private (int firstChildId, short childCount, NodeType nodeType, string name) ProcessNodeInfo(long offset)
    {
        var nameOffset = _reader.ReadInt(offset);
        var firstChildId = _reader.ReadInt(offset + 4);
        var childCount = _reader.ReadShort(offset + 8);
        var nodeType = _reader.ReadShort(offset + 10);
        var stringOffset = _reader.ReadLong(_reader.StringBlockOffset + (sizeof(long) * nameOffset));
        var name = _reader.ReadString((int)stringOffset);

        return (firstChildId, childCount, (NodeType)nodeType, name);
    }
    
    public void Dispose()
    {
        _reader.Dispose();
    }
}