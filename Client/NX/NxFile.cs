using System.Diagnostics;

namespace Client.NX;

public class NxFile : IDisposable
{
    private NxReader _reader; // Used as reference.
    private readonly Dictionary<string, long> _stringPool;
    private readonly string _parentFile;
    private readonly NxSaveMode _saveMode;

    public Dictionary<string, long> StringPool => _stringPool;

    public NxFile(string path, NxSaveMode saveMode)
    {
        _parentFile = Path.GetFileNameWithoutExtension(path);
        _reader = new (File.OpenRead(path));
        _stringPool = new();
        _saveMode = saveMode;
        if (Path.GetFileNameWithoutExtension(path) == "Data")
            ReadBetaNodeData();
        else
            ReadNodeData();
    }

    private void ReadBetaNodeData()
    {
        var dataFileRootOffset = _reader.NodeBlockOffset;
        _reader.Seek(dataFileRootOffset);
        _reader.Skip(4);
        var dataFileRootFirstChildId = _reader.ReadInt();
        var dataFileRootChildCount = _reader.ReadShort();
        
        for (var i = dataFileRootFirstChildId; i < dataFileRootFirstChildId + dataFileRootChildCount; i++)
        {
            var rootFileOffset = _reader.NodeBlockOffset + 20 * i;
            _reader.Seek(rootFileOffset);

            var rootNameOffset = _reader.ReadInt();
            var rootFirstChildId = _reader.ReadInt();
            var rootChildCount = _reader.ReadShort();
            _reader.Seek(_reader.StringBlockOffset + 8 * rootNameOffset);
            var rootStringOffset = _reader.ReadLong();
            var rootName = _reader.ReadString(rootStringOffset);

            switch (rootName)
            {
                case "Character":
                {
                    for (var j = rootFirstChildId; j < rootFirstChildId + rootChildCount; j++)
                    {
                        var offset = _reader.NodeBlockOffset + 20 * j;
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
                                for (var k = firstChildId; k < firstChildId + childCount; k++)
                                {
                                    var childOffset = _reader.NodeBlockOffset + 20 * j;
                                    _reader.Seek(childOffset);
                                    var childNameOffset = _reader.ReadInt();
                                    _reader.Seek(_reader.StringBlockOffset + 8 * childNameOffset);
                                    var childStringOffset = _reader.ReadLong();
                                    var childName = _reader.ReadString(childStringOffset);
                                    
                                    if (!name.Contains(".img")) continue;
                                    _stringPool.Add($"{rootName}/{name}/{childName}", childOffset);
                                }
                            }
                                break;
                        }
                    }
                    break;
                }
                case "Map":
                {
                    for (var j = rootFirstChildId; j < rootFirstChildId + rootChildCount; j++)
                    {
                        var offset = _reader.NodeBlockOffset + 20 * j;
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
                                for (var k = firstChildId; k < firstChildId + childCount; k++)
                                {
                                    var mapOffset = _reader.NodeBlockOffset + 20 * k;
                                    _reader.Seek(mapOffset);
                                    var mapNameOffset = _reader.ReadInt();
                                    var mapFirstChildId = _reader.ReadInt();
                                    var mapChildCount = _reader.ReadShort();
                                    _reader.Seek(_reader.StringBlockOffset + 8 * mapNameOffset);
                                    var mapStringOffset = _reader.ReadLong();
                                    var mapName = _reader.ReadString(mapStringOffset);
                                    for (var l = mapFirstChildId; l < mapFirstChildId + mapChildCount; l++)
                                    {
                                        var childOffset = _reader.NodeBlockOffset + 20 * l;
                                        _reader.Seek(childOffset);
                                        var childNameOffset = _reader.ReadInt();
                                        _reader.Seek(_reader.StringBlockOffset + 8 * childNameOffset);
                                        var childStringOffset = _reader.ReadLong();
                                        var childName = _reader.ReadString(childStringOffset);
                                        
                                        if (!childName.Contains(".img")) continue;
                                        _stringPool.Add($"{rootName}/{mapName}/{childName}", childOffset);
                                    }
                                }
                            }
                                break;
                            case "Back":
                            case "Tile":
                            case "Obj":
                            case "WorldMap":
                            {
                                for (var k = firstChildId; k < firstChildId + childCount; k++)
                                {
                                    var childOffset = _reader.NodeBlockOffset + 20 * k;
                                    _reader.Seek(childOffset);
                                    var childNameOffset = _reader.ReadInt();
                                    _reader.Seek(_reader.StringBlockOffset + 8 * childNameOffset);
                                    var childStringOffset = _reader.ReadLong();
                                    var childName = _reader.ReadString(childStringOffset);
                                    if (!childName.Contains(".img")) continue;
                                    _stringPool.Add($"{rootName}/{name}/{childName}", childOffset);
                                }
                            }
                                break;
                            case "Effect.img":
                            case "MapHelper.img":
                            case "Physics.img":
                                _stringPool.Add($"{rootName}/{name}", offset);
                                break;
                        }
                    }
                }
                    break;
                default:
                    for (var j = rootFirstChildId; j < rootFirstChildId + rootChildCount; j++)
                    {
                        var childOffset = _reader.NodeBlockOffset + 20 * j;
                        _reader.Seek(childOffset);
                        
                        var childNameOffset = _reader.ReadInt();
                        _reader.Seek(_reader.StringBlockOffset + 8 * childNameOffset);
                        var childStringOffset = _reader.ReadLong();
                        var childName = _reader.ReadString(childStringOffset);
                                    
                        if (!childName.Contains(".img")) continue;
                        _stringPool.Add($"{rootName}/{childName}", childOffset);
                    }
                    break;
            }
        }
    }

    private void ReadNodeData()
    {
        var rootNodeOffset = _reader.NodeBlockOffset;
        _reader.Seek(rootNodeOffset);
        _reader.Skip(4);
        var rootFirstChildId = _reader.ReadInt();
        var rootChildCount = _reader.ReadShort();

        switch (_parentFile)
        {
            case "Map":
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

                break;
            }
            case "Character":
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

                break;
            }
            default:
            {
                for (var i = rootFirstChildId; i < rootFirstChildId + rootChildCount; i++)
                {
                    var offset = _reader.NodeBlockOffset + 20 * i;
                    _reader.Seek(offset);

                    var nameOffset = _reader.ReadInt();
                    _reader.Seek(_reader.StringBlockOffset + 8 * nameOffset);
                    var stringOffset = _reader.ReadLong();
                    var name = _reader.ReadString(stringOffset);
                    if (!name.Contains(".img")) continue;
                    _stringPool.Add(name, offset);
                }

                break;
            }
        }
    }

    public NxNode GetNode(string nodeName)
    {
        if (!_stringPool.TryGetValue(nodeName, out var offset))
            throw new NullReferenceException($"[NX] The node ({nodeName}) was not found within the string pool.");
        return new NxNode(_reader, offset, nodeName, _saveMode) { FullPath = nodeName };
    }

    public void Dispose()
    {
        _reader.Dispose();
        _stringPool.Clear();
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