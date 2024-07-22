using System.Numerics;
using Client.NX;
using Raylib_CsLo;

namespace Client.Avatar;

public struct BodyPart
{
    private NxNode _node;
    private Dictionary<string, Vector2> _mappings;
    private List<Texture> _frames;
    private BodyType _bodyType;
    
    public List<Texture> Frames => _frames;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="node">The node that contains the data we're looking, NOT THE .IMG NODE!</param>
    /// <param name="bodyType"></param>
    public BodyPart(NxNode node, BodyType bodyType)
    {
        _node = node;
        _bodyType = bodyType;
        _mappings = new();
        _frames = new();
        
        Init();
    }

    private void Init()
    {
        for (var i = 0; i < _node.ChildCount; i++)
        {
            switch (_bodyType)
            {
                case BodyType.Body:
                    _frames.Add(_node[$"{i}"].GetTexture("body"));
                    break;
                case BodyType.Arm:
                    _frames.Add(_node[$"{i}"].GetTexture("arm"));
                    break;
                case BodyType.Head:
                    _frames.Add(_node[$"{i}"].GetTexture("head"));
                    break;
                case BodyType.Face:
                    _frames.Add(_node[$"{i}"].GetTexture("face"));
                    break;
            }
            
        }
    }
    
    public void Clear()
    {
        foreach (var tex in _frames)
            Raylib.UnloadTexture(tex);
        _frames.Clear();
        _mappings.Clear();
    }

    public Vector2 GetOrigin(int frame)
    {
        return _bodyType switch
        {
            BodyType.Body => _node[$"{frame}"]["body"].GetVector("origin"),
            BodyType.Arm => _node[$"{frame}"]["arm"].GetVector("origin"),
            BodyType.Head => _node[$"{frame}"]["head"].GetVector("origin"),
            BodyType.Face => _node[$"{frame}"]["face"].GetVector("origin"),
            _ => Vector2.Zero
        };
    }

    public Vector2 GetMap(int frame, string map)
    {
        return _bodyType switch
        {
            BodyType.Body => _node[$"{frame}"]["body"]["map"].GetVector(map),
            BodyType.Arm => _node[$"{frame}"]["arm"]["map"].GetVector(map),
            BodyType.Head => _node[$"{frame}"]["head"]["map"].GetVector(map),
            BodyType.Face => _node[$"{frame}"]["face"]["map"].GetVector(map),
            _ => Vector2.Zero
        };
    }
}