using System.Numerics;
using Client.NX;
using Raylib_CsLo;

namespace Client.Avatar;

public struct BodyPart
{
    private readonly NxNode _node;
    private readonly PartType _part;

    private readonly Dictionary<string, List<Texture>> _textures;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="node">The node that contains the data we're looking, NOT THE .IMG NODE!</param>
    /// <param name="partType"></param>
    public BodyPart(NxNode node, PartType partType)
    {
        _node = node;
        _part = partType;
        _textures = new();
        Init();
    }

    private void Init()
    {
        for (var i = 0; i < _node.ChildCount; i++)
        {
            switch (_part)
            {
                case PartType.Body:
                    _textures.TryAdd("body", new ());
                    _textures["body"].Add(_node[$"{i}"].GetTexture("body"));
                    break;
                case PartType.Arm:
                    _textures.TryAdd("arm", new ());
                    _textures["arm"].Add(_node[$"{i}"].GetTexture("arm"));
                    break;
                case PartType.Head:
                    _textures.TryAdd("head", new ());
                    _textures["head"].Add(_node[$"{i}"].GetTexture("head"));
                    break;
                case PartType.Face:
                    _textures.TryAdd("face", new());
                    _textures["face"].Add(_node[$"{i}"].GetTexture("face"));
                    break;
                case PartType.Hair:
                    var hairNode = _node[$"{i}"];
                    if (hairNode?.Has("hairOverHead") ?? false)
                    {
                        _textures.TryAdd("hairOverHead", new());
                        _textures["hairOverHead"].Add(hairNode?.GetTexture("hairOverHead") ?? throw new Exception("[Hair] Something happened here."));
                    }
                    if (hairNode?.Has("hair") ?? false)
                    {
                        _textures.TryAdd("hair", new());
                        _textures["hair"].Add(hairNode?.GetTexture("hair") ?? throw new Exception("[Hair] Something happened here."));
                    }
                    if (hairNode?.Has("hairShade") ?? false)
                    {
                        //_hairs[i][HairType.HairOverHead] = hairNode?.GetTexture("hairShade") ?? throw new Exception("[Hair] Something happened here.");
                    }
                    if (hairNode?.Has("backHair") ?? false)
                    {
                        _textures.TryAdd("backHair", new());
                        _textures["backHair"].Add(hairNode?.GetTexture("backHair") ?? throw new Exception("[Hair] Something happened here."));
                    }
                    if (hairNode?.Has("backHairBelowCap") ?? false)
                    {
                        _textures.TryAdd("backHairBelowCap", new());
                        _textures["backHairBelowCap"].Add(hairNode?.GetTexture("backHairBelowCap") ?? throw new Exception("[Hair] Something happened here."));
                    }
                    break;
                case PartType.Afterimage:
                    break;
                case PartType.Accessory1:
                case PartType.Accessory2:
                case PartType.Accessory3:
                case PartType.Accessory4:
                case PartType.Accessory5:
                case PartType.Accessory6:
                    break;
                case PartType.Cap:
                    _textures.TryAdd("cap", new());
                    _textures["cap"].Add(_node[$"{i}"].GetTexture("cap"));
                    break;
                case PartType.Cape:
                    _textures.TryAdd("cape", new());
                    _textures["cape"].Add(_node[$"{i}"].GetTexture("cape"));
                    break;
                case PartType.Longcoat:
                case PartType.Coat:
                    _textures.TryAdd("mail", new());
                    _textures.TryAdd("mailArm", new());
                    _textures["mail"].Add(_node[$"{i}"].GetTexture("mail"));
                    _textures["mailArm"].Add(_node[$"{i}"].GetTexture("mailArm"));
                    break;
                case PartType.Glove:
                    _textures.TryAdd("glove", new());
                    _textures["glove"].Add(_node[$"{i}"].GetTexture("glove"));
                    break;
                case PartType.Pants:
                    _textures.TryAdd("pants", new());
                    _textures["pants"].Add(_node[$"{i}"].GetTexture("pants"));
                    break;
                case PartType.Shield:
                    _textures.TryAdd("shield", new());
                    _textures["shield"].Add(_node[$"{i}"].GetTexture("shield"));
                    break;
                case PartType.Weapon:
                    _textures.TryAdd("weapon", new());
                    _textures["weapon"].Add(_node[$"{i}"].GetTexture("weapon"));
                    break;
                case PartType.FWeapon:
                    _textures.TryAdd("weaponOverBody", new());
                    _textures.TryAdd("weaponArmOverHair", new());
                    _textures["weaponOverBody"].Add(_node[$"{i}"].GetTexture("weaponOverBody"));
                    _textures["weaponArmOverHair"].Add(_node[$"{i}"].GetTexture("weaponArmOverHair"));
                    break;
            }

        }
    }
    
    public void Clear()
    {
        foreach (var (_,state) in _textures)
            foreach (var tex in state)
            Raylib.UnloadTexture(tex);
        _textures.Clear();
    }

    public Vector2 GetOrigin(int index, string part)
    {
        return _part switch
        {
            PartType.Body => _node[$"{index}"]["body"].GetVector("origin"),
            PartType.Arm => _node[$"{index}"]["arm"].GetVector("origin"),
            PartType.Head => _node[$"{index}"]["head"].GetVector("origin"),
            PartType.Face => _node[$"{index}"]["face"].GetVector("origin"),
            _ => _node[$"{index}"][part].GetVector("origin")
        };
    }

    public Vector2 GetMap(int index, string part, string map)
    {
        return _part switch
        {
            PartType.Body => _node[$"{index}"]["body"]["map"].GetVector(map),
            PartType.Arm => _node[$"{index}"]["arm"]["map"].GetVector(map),
            PartType.Head => _node[$"{index}"]["head"]["map"].GetVector(map),
            PartType.Face => _node[$"{index}"]["face"]["map"].GetVector(map),
            _ => _node[$"{index}"][part]["map"].GetVector(map)
        };
    }

    public Texture GetTexture(int index, string part)
    {
        return _part switch
        {
            PartType.Body => _textures["body"][index],
            PartType.Arm => _textures["arm"][index],
            PartType.Head => _textures["head"][index],
            PartType.Face => _textures["face"][index],
            _ => _textures[part][index]
        };
    }
}