using System.Diagnostics;

namespace Client.NX;

public class NxFile
{
    private NxReader _reader; // Used as reference.
    private readonly Dictionary<string, long> _stringPool;
    private readonly string _parentFile;
    private readonly NxSaveMode _saveMode;

    public NxFile(string path, NxSaveMode saveMode)
    {
        _parentFile = Path.GetFileNameWithoutExtension(path);
        _reader = new (File.OpenRead(path));
        _stringPool = new();
        _reader.ParseHeader();
        ReadNodeData();
    }

    ~NxFile()
    {
        _stringPool.Clear();
    }

    private void ReadNodeData()
    {
        var timer = Stopwatch.StartNew();
        var nodeCount = _reader.NodeCount;
        var rootNodeOffset = _reader.NodeBlockOffset;
        _reader.Seek(rootNodeOffset);
        var rootNameOffset = _reader.ReadInt();
        var rootFirstChildId = _reader.ReadInt();
        var rootChildCount = _reader.ReadShort();

        if (_parentFile == "Map")
        {
            for (var i = rootFirstChildId; i < rootFirstChildId + rootChildCount; i++)
            {
                var offset = _reader.NodeBlockOffset + 20 * i;
                _reader.Seek(offset);
                var nameOffset = _reader.ReadInt();
                var firstChildId = _reader.ReadInt();
                var childCount = _reader.ReadShort();
                _reader.Seek(_reader.StringBlockOffset + 8 * nameOffset);
                var stringOffset = _reader.ReadLong();
                var name = _reader.ReadString(stringOffset);
                switch (name)
                {
                    case "Map":
                    {
                        for (var j = firstChildId; j < firstChildId + childCount; j++)
                        {
                            var mapOffset = _reader.NodeBlockOffset + 20 * j;
                            _reader.Seek(mapOffset);
                            var mapNameOffset = _reader.ReadInt();
                            var mapFirstChildId = _reader.ReadInt();
                            var mapChildCount = _reader.ReadShort();
                            _reader.Seek(_reader.StringBlockOffset + 8 * mapNameOffset);
                            var mapStringOffset = _reader.ReadLong();
                            var mapName = _reader.ReadString(mapStringOffset);
                            for (var k = mapFirstChildId; k < mapFirstChildId + mapChildCount; k++)
                            {
                                var childOffset = _reader.NodeBlockOffset + 20 * k;
                                _reader.Seek(childOffset);
                                var childNameOffset = _reader.ReadInt();
                                _reader.Seek(_reader.StringBlockOffset + 8 * childNameOffset);
                                var childStringOffset = _reader.ReadLong();
                                var childName = _reader.ReadString(childStringOffset);
                                _stringPool.Add($"{mapName}/{childName}", childOffset);
                            }
                        }
                    }
                        break;
                    case "Back": 
                    case "Tile":
                    case "Obj":
                    case "WorldMap":
                    {
                        for (var j = firstChildId; j < firstChildId + childCount; j++)
                        {
                            var childOffset = _reader.NodeBlockOffset + 20 * j;
                            _reader.Seek(childOffset);
                            var childNameOffset = _reader.ReadInt();
                            _reader.Seek(_reader.StringBlockOffset + 8 * childNameOffset);
                            var childStringOffset = _reader.ReadLong();
                            var childName = _reader.ReadString(childStringOffset);
                            _stringPool.Add($"{name}/{childName}", childOffset);
                        }
                    }
                        break;
                    case "Effect.img":
                    case "MapHelper.img":
                    case "Physics.img":
                    {
                        _stringPool.Add($"{name}", offset);
                    }
                        break;
                }
            }
        } else if (_parentFile == "Character")
        {
            for (var i = rootFirstChildId; i < rootFirstChildId + rootChildCount; i++)
            {
                var offset = _reader.NodeBlockOffset + 20 * i;
                _reader.Seek(offset);
                var nameOffset = _reader.ReadInt();
                var firstChildId = _reader.ReadInt();
                var childCount = _reader.ReadShort();
                _reader.Seek(_reader.StringBlockOffset + 8 * nameOffset);
                var stringOffset = _reader.ReadLong();
                var name = _reader.ReadString(stringOffset);
                switch (name)
                {
                    case "Cap": 
                    case "Cape":
                    case "Coat":
                    case "Face":
                    case "Glove":
                    case "Hair":
                    case "Longcoat":
                    case "Pants":
                    case "PetEquip":
                    case "Ring":
                    case "Shield":
                    case "Shoes":
                    case "TamingMob":
                    case "Weapon":
                    {
                        for (var j = firstChildId; j < firstChildId + childCount; j++)
                        {
                            var childOffset = _reader.NodeBlockOffset + 20 * j;
                            _reader.Seek(childOffset);
                            var childNameOffset = _reader.ReadInt();
                            _reader.Seek(_reader.StringBlockOffset + 8 * childNameOffset);
                            var childStringOffset = _reader.ReadLong();
                            var childName = _reader.ReadString(childStringOffset);
                            _stringPool.Add($"{name}/{childName}", childOffset);
                        }
                    }
                        break;
                    default:
                    {
                        _stringPool.Add($"{name}", offset);
                    }
                        break;
                }
            }
        }
        else
        {

            for (var i = rootFirstChildId; i < rootFirstChildId + rootChildCount; i++)
            {
                var offset = _reader.NodeBlockOffset + 20 * i;
                _reader.Seek(offset);

                var nameOffset = _reader.ReadInt();
                _reader.Seek(_reader.StringBlockOffset + 8 * nameOffset);
                var stringOffset = _reader.ReadLong();
                var name = _reader.ReadString(stringOffset);
                _stringPool.Add(name, offset);
            }
        }
        
        timer.Stop();
        Console.WriteLine($"All nodes parsed in: {timer.ElapsedMilliseconds} milliseconds");
    }

    public NxNode GetNode(string nodeName)
    {
        if (!_stringPool.TryGetValue(nodeName, out var offset))
            throw new NullReferenceException($"[NX] The node ({nodeName}) was not found within the string pool.");
        return new NxNode(_reader, offset, nodeName, _saveMode);
    }
}

public enum NodeType
{
    NoData = 0,
    Int64 = 1,
    Double = 2,
    String = 3,
    Vector = 4,
    Bitmap = 5,
    Audio = 6
};