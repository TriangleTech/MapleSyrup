using System.Numerics;
using Client.Actors;
using Client.Managers;
using Client.Net;
using Client.NX;
using Raylib_CsLo;

namespace Client.Avatar;

public class Player : IActor
{
    #region Variables

    private AvatarState _bodyState = AvatarState.Stand1;
    private string _faceState = "blink";
    private readonly Dictionary<PartType, NxNode?> _partNodes;

    private readonly Dictionary<PartType, Dictionary<string, BodyPart>>
        _parts; // TODO: We can change this to an int since the states are hardcoded.

    private int _bodyFrame, _faceFrame, _faceFrameCount, _bodyFrameCount, _bodyDelay, _faceDelay, _blinkDelay;

    public int ID { get; init; }
    public string Name { get; set; }
    public int Z { get; set; }
    public bool Visible { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Origin { get; set; }
    public Vector2 ScreenOffset { get; set; }
    public ActorLayer Layer { get; set; }
    public ActorType ActorType { get; init; } = ActorType.Player;
    public Rectangle Bounds { get; set; }
    public Rectangle DstRectangle { get; set; }
    public NxNode? Node { get; set; }
    public AvatarLook Look { get; init; }
    public AvatarStats Stats { get; init; }
    public bool ControlsLocked { get; set; }
    public bool IsLeft { get; set; }
    public bool Moving { get; set; }
    public bool OnGround { get; set; }
    public bool IsFemale { get; }
    private Vector2 _position;
    private readonly int _defaultShirt, _defaultPants;

    private readonly List<string> _moveStates =
    [
        "stand1",
        "stand2",
        "alert",
        "walk1",
        "walk2",
        "jump",
        "shoot1",
        "shoot2",
        "shootF",
        "sit",
        "stabO1",
        "stabO2",
        "stabOF",
        "stabT1",
        "stabT2",
        "swingO1",
        "swingO2",
        "swingO3",
        "swingP1",
        "swingP2",
        "swingT1",
        "swingT2",
        "swingT3",
        "dead",
        "fly",
        "ladder",
        "rope",
        "proneStab",
        "swingOF",
        "swingPF",
        "swingTF",
        "stabTF",
        "prone",
        "heal"
    ];

    private readonly List<int> _moveFrameCount =
    [
        3,
        3,
        3,
        4,
        4,
        1,
        3,
        5,
        2,
        1,
        2,
        2,
        3,
        3,
        3,
        3,
        3,
        3,
        3,
        3,
        3,
        3,
        3,
        1,
        2,
        2,
        2,
        2,
        4,
        4,
        4,
        4,
        1,
        3
    ];

    #endregion

    public Player(AvatarStats stats, AvatarLook look)
    {
        Stats = stats;
        Look = look;
        _parts = new();
        _partNodes = new();
        IsFemale = Look.Gender == 1;
        _defaultShirt = IsFemale ? 0 : 1040036;
        _defaultPants = IsFemale ? 0 : 1060026;
        InitializeBody();
        InitializeEquips();
    }

    private void InitializeBody()
    {
        var character = ServiceLocator.Get<NxManager>();
        _position = Position;
        _partNodes[PartType.Body] = character.GetNode($"00002{GetSkinId(Look.SkinId)}.img");
        _partNodes[PartType.Head] = character.GetNode($"00012{GetSkinId(Look.SkinId)}.img");
        _partNodes[PartType.Face] = character.GetNode($"Face/0002000{Look.Face}.img");
        _partNodes[PartType.Hair] =
            character.GetNode($"Hair/000{Look.Hair}.img"); // TODO: Does the hair id get sent as a whole?

        var body = _parts[PartType.Body] = new();
        var head = _parts[PartType.Head] = new();
        var arm = _parts[PartType.Arm] = new();
        var hair = _parts[PartType.Hair] = new();
        var face = _parts[PartType.Face] = new();

        foreach (var state in _moveStates)
        {
            _bodyFrameCount = _partNodes[PartType.Body]?[state]?.ChildCount ?? 0;
            _bodyFrame = 0;
            body[state] = new(_partNodes[PartType.Body]?[state]!, PartType.Body);

            if (state == "dead")
            {
                head[state] = new(_partNodes[PartType.Head]?["stand1"]!, PartType.Head);
                hair[state] = new(_partNodes[PartType.Hair]?["stand1"]!, PartType.Hair);
            }
            else
            {
                head[state] = new(_partNodes[PartType.Head]?[state]!, PartType.Head);
                hair[state] = new(_partNodes[PartType.Hair]?[state]!, PartType.Hair);
            }

            if (_partNodes[PartType.Body]?[state]?["0"]!.Has("arm") ?? false)
                arm[state] = new(_partNodes[PartType.Body][state], PartType.Arm);
        }

        _bodyFrameCount = _moveFrameCount[(int)_bodyState];
        _bodyFrame = 0;
        face[_faceState] = new(_partNodes[PartType.Face]?[_faceState]!, PartType.Face);
        _faceFrameCount = _partNodes[PartType.Face]?[_faceState]?.ChildCount ?? 0;
        _faceFrame = 0;
    }

    private void InitializeEquips()
    {
        if (!IsFemale)
        {
            foreach (var state in _moveStates)
            {
                foreach (var (part, item) in Look.Equipment)
                {
                    switch (part)
                    {
                        case PartType.Accessory1:
                        case PartType.Accessory2:
                        case PartType.Accessory3:
                        case PartType.Accessory4:
                        case PartType.Accessory5:
                        case PartType.Accessory6:
                            break;
                        case PartType.Cap:
                            _partNodes.TryAdd(part,
                                ServiceLocator.Get<NxManager>().GetNode($"Cap/{GetItemId(item)}.img"));
                            break;
                        case PartType.Cape:
                            _partNodes.TryAdd(part,
                                ServiceLocator.Get<NxManager>().GetNode($"Cape/{GetItemId(item)}.img"));
                            break;
                        case PartType.Longcoat:
                            _partNodes.TryAdd(part,
                                ServiceLocator.Get<NxManager>().GetNode($"Longcoat/{GetItemId(item)}.img"));
                            break;
                        case PartType.Coat:
                            _partNodes.TryAdd(part,
                                ServiceLocator.Get<NxManager>().GetNode($"Coat/{GetItemId(item)}.img"));
                            break;
                        case PartType.Glove:
                            _partNodes.TryAdd(part,
                                ServiceLocator.Get<NxManager>().GetNode($"Glove/{GetItemId(item)}.img"));
                            break;
                        case PartType.Pants:
                            _partNodes.TryAdd(part,
                                ServiceLocator.Get<NxManager>().GetNode($"Pants/{GetItemId(item)}.img"));
                            break;
                        case PartType.Shield:
                            _partNodes.TryAdd(part,
                                ServiceLocator.Get<NxManager>().GetNode($"Shield/{GetItemId(item)}.img"));
                            break;
                        case PartType.Weapon:
                            _partNodes.TryAdd(part,
                                ServiceLocator.Get<NxManager>().GetNode($"Weapon/{GetItemId(item)}.img"));
                            break;
                        case PartType.FWeapon:
                            break;
                    }

                    _parts.TryAdd(part, new());
                    if (!_partNodes[part]?.Has(state) ?? false) continue;
                    var equip = _parts[part];
                    equip[state] = new(_partNodes[part]![state]!, part);
                }
            }
        }
    }

    public void ChangeState(AvatarState state)
    {
        if (state == _bodyState) return;
        _bodyState = state;
        _bodyFrameCount = _moveFrameCount[(int)_bodyState];
        _bodyFrame = 0;
    }

    public void Clear()
    {
        foreach (var part in _parts)
        {
            foreach (var (_, item) in part.Value)
            {
                item.Clear();
            }

            part.Value.Clear();
        }

        foreach (var (_, node) in _partNodes)
            node?.Dispose();

        _moveFrameCount.Clear();
        _moveStates.Clear();
    }

    #region Player Draw Functions

    public void Draw(float frameTime)
    {
        if (_bodyState is not (AvatarState.Ladder or AvatarState.Rope))
        {

            DrawCape();
            DrawBody();
            DrawPants();
            DrawCoat();
            DrawHead();
            DrawFace();
            DrawHair(_partNodes.ContainsKey(PartType.Cap) ? "hair" : "hairOverHead");

            if (Look.Equipment.TryGetValue(PartType.Weapon, out var id))
            {
                switch (GetWeaponCategory(id))
                {
                    case 30:
                    case 31:
                    case 32:
                    case 33:
                    case 37:
                    case 38:
                        DrawWeapon();
                        break;
                }
            }

            DrawArm();
        }
        else
        {

        }
    }

    #region Draw Body/Head/Arm/Face/Hair

    private void DrawHead()
    {
        var state = _moveStates[(int)_bodyState];
        var body = _parts[PartType.Body];
        var head = _parts[PartType.Head];
        if (IsLeft)
        {
            var headOrigin = head[state].GetOrigin(_bodyFrame, "") +
                             head[state].GetMap(_bodyFrame, "", "neck") -
                             body[state].GetMap(_bodyFrame, "", "neck");
            var headTexture = head[state].GetTexture(_bodyFrame, "");
            var headSrcRec = new Rectangle(0, 0, headTexture.width, headTexture.height);
            var headDstRec = new Rectangle(
                (Position.X - headOrigin.X) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetX,
                (Position.Y - headOrigin.Y) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetY,
                headTexture.width * AppConfig.ScaleFactor,
                headTexture.height * AppConfig.ScaleFactor);
            Raylib.DrawTexturePro(headTexture, headSrcRec, headDstRec, Vector2.Zero, 0f, Raylib.WHITE);
        }
        else
        {
            var headOrigin = head[state].GetOrigin(_bodyFrame, "") +
                             head[state].GetMap(_bodyFrame, "", "neck") -
                             body[state].GetMap(_bodyFrame, "", "neck");
            var headTexture = head[state].GetTexture(_bodyFrame, "");
            var headSrcRec = new Rectangle(0, 0, -headTexture.width, headTexture.height);
            var headDstRec = new Rectangle(
                (Position.X + headOrigin.X - headTexture.width) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetX,
                (Position.Y - headOrigin.Y) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetY,
                headTexture.width * AppConfig.ScaleFactor,
                headTexture.height * AppConfig.ScaleFactor);

            Raylib.DrawTexturePro(headTexture, headSrcRec, headDstRec, Vector2.Zero, 0f, Raylib.WHITE);

        }
    }

    private void DrawBody()
    {
        var state = _moveStates[(int)_bodyState];
        var body = _parts[PartType.Body];
        if (IsLeft)
        {
            var bodyOrigin = body[state].GetOrigin(_bodyFrame, "");
            var bodyTexture = body[state].GetTexture(_bodyFrame, "");
            var bodySrcRec = new Rectangle(0, 0, bodyTexture.width, bodyTexture.height);
            var bodyDstRec = new Rectangle(
                (Position.X - bodyOrigin.X) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetX,
                (Position.Y - bodyOrigin.Y) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetY,
                bodyTexture.width * AppConfig.ScaleFactor,
                bodyTexture.height * AppConfig.ScaleFactor);

            Raylib.DrawTexturePro(bodyTexture, bodySrcRec, bodyDstRec, Vector2.Zero, 0f, Raylib.WHITE);
        }
        else
        {
            var bodyOrigin = body[state].GetOrigin(_bodyFrame, "");
            var bodyTexture = body[state].GetTexture(_bodyFrame, "");
            var bodySrcRec = new Rectangle(0, 0, -bodyTexture.width, bodyTexture.height);
            var bodyDstRec = new Rectangle(
                (Position.X + bodyOrigin.X - bodyTexture.width) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetX,
                (Position.Y - bodyOrigin.Y) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetY,
                bodyTexture.width * AppConfig.ScaleFactor,
                bodyTexture.height * AppConfig.ScaleFactor);

            Raylib.DrawTexturePro(bodyTexture, bodySrcRec, bodyDstRec, Vector2.Zero, 0f, Raylib.WHITE);
        }

    }

    private void DrawArm()
    {
        var state = _moveStates[(int)_bodyState];
        var body = _parts[PartType.Body];
        var arm = _parts[PartType.Arm];
        if (!arm.TryGetValue(state, out BodyPart value)) return;

        if (IsLeft)
        {
            var armOrigin = value.GetOrigin(_bodyFrame, "") +
                            value.GetMap(_bodyFrame, "", "navel") -
                            body[state].GetMap(_bodyFrame, "", "navel");
            var armTexture = value.GetTexture(_bodyFrame, "");
            var armSrcRect = new Rectangle(0, 0, armTexture.width, armTexture.height);
            var armDstRec = new Rectangle(
                (Position.X - armOrigin.X) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetX,
                (Position.Y - armOrigin.Y) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetY,
                armTexture.width * AppConfig.ScaleFactor,
                armTexture.height * AppConfig.ScaleFactor);

            Raylib.DrawTexturePro(armTexture, armSrcRect, armDstRec, Vector2.Zero, 0f, Raylib.WHITE);
        }
        else
        {
            var armOrigin = value.GetOrigin(_bodyFrame, "") +
                            value.GetMap(_bodyFrame, "", "navel") -
                            body[state].GetMap(_bodyFrame, "", "navel");
            var armTexture = value.GetTexture(_bodyFrame, "");
            var armSrcRect = new Rectangle(0, 0, -armTexture.width, armTexture.height);
            var armDstRec = new Rectangle(
                (Position.X + armOrigin.X - armTexture.width) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetX,
                (Position.Y - armOrigin.Y) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetY,
                armTexture.width * AppConfig.ScaleFactor,
                armTexture.height * AppConfig.ScaleFactor);
            Raylib.DrawTexturePro(armTexture, armSrcRect, armDstRec, Vector2.Zero, 0f, Raylib.WHITE);
        }
    }

    private void DrawFace()
    {
        var state = _moveStates[(int)_bodyState];
        var body = _parts[PartType.Body];
        var head = _parts[PartType.Head];
        var face = _parts[PartType.Face];
        var showFace = _partNodes[PartType.Body]?[state][$"{_bodyFrame}"].GetInt("face");
        if (showFace != 1) return;

        if (IsLeft)
        {
            var faceOrigin = face[_faceState].GetOrigin(_faceFrame, "") -
                             head[state].GetMap(_bodyFrame, "", "brow") +
                             face[_faceState].GetMap(_faceFrame, "", "brow") +
                             head[state].GetMap(_bodyFrame, "", "neck") -
                             body[state].GetMap(_bodyFrame, "", "neck");
            var faceTexture = face[_faceState].GetTexture(_faceFrame, "face");
            var faceSrcRec = new Rectangle(0, 0, faceTexture.width, faceTexture.height);
            var faceDstRec = new Rectangle(
                (Position.X - faceOrigin.X) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetX,
                (Position.Y - faceOrigin.Y) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetY,
                faceTexture.width * AppConfig.ScaleFactor,
                faceTexture.height * AppConfig.ScaleFactor);
            Raylib.DrawTexturePro(faceTexture, faceSrcRec, faceDstRec, Vector2.Zero, 0f, Raylib.WHITE);
        }
        else
        {
            var faceOrigin = face[_faceState].GetOrigin(_faceFrame, "") -
                             head[state].GetMap(_bodyFrame, "", "brow") +
                             face[_faceState].GetMap(_faceFrame, "", "brow") +
                             head[state].GetMap(_bodyFrame, "", "neck") -
                             body[state].GetMap(_bodyFrame, "", "neck");
            var faceTexture = face[_faceState].GetTexture(_faceFrame, "face");
            var faceSrcRec = new Rectangle(0, 0, -faceTexture.width, faceTexture.height);
            var faceDstRec = new Rectangle(
                (Position.X + faceOrigin.X - faceTexture.width) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetX,
                (Position.Y - faceOrigin.Y) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetY,
                faceTexture.width * AppConfig.ScaleFactor,
                faceTexture.height * AppConfig.ScaleFactor);
            Raylib.DrawTexturePro(faceTexture, faceSrcRec, faceDstRec, Vector2.Zero, 0f, Raylib.WHITE);
        }
    }

    private void DrawHair(string hairType)
    {
        var state = _moveStates[(int)_bodyState];
        var body = _parts[PartType.Body];
        var head = _parts[PartType.Head];
        var hair = _parts[PartType.Hair];
        if (IsLeft)
        {
            // TODO: Handle hairShade, hairBelowCap, and Hair.
            var hairOrigin = hair[state].GetOrigin(_bodyFrame, hairType) -
                             head[state].GetMap(_bodyFrame, "", "brow") +
                             hair[state].GetMap(_bodyFrame, hairType, "brow") +
                             head[state].GetMap(_bodyFrame, "", "neck") -
                             body[state].GetMap(_bodyFrame, "", "neck");
            var hairTexture = hair[state].GetTexture(_bodyFrame, hairType);
            var hairSrcRec = new Rectangle(0, 0, hairTexture.width, hairTexture.height);
            var hairDstRec = new Rectangle(
                (Position.X - hairOrigin.X) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetX,
                (Position.Y - hairOrigin.Y) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetY,
                hairTexture.width * AppConfig.ScaleFactor,
                hairTexture.height * AppConfig.ScaleFactor);
            Raylib.DrawTexturePro(hairTexture, hairSrcRec, hairDstRec, Vector2.Zero, 0f, Raylib.WHITE);
        }
        else
        {
            var hairOrigin = hair[state].GetOrigin(_bodyFrame, hairType) -
                             head[state].GetMap(_bodyFrame, "", "brow") +
                             hair[state].GetMap(_bodyFrame, hairType, "brow") +
                             head[state].GetMap(_bodyFrame, "", "neck") -
                             body[state].GetMap(_bodyFrame, "", "neck");
            var hairTexture = hair[state].GetTexture(_bodyFrame, hairType);
            var hairSrcRec = new Rectangle(0, 0, -hairTexture.width, hairTexture.height);
            var hairDstRec = new Rectangle(
                (Position.X + hairOrigin.X - hairTexture.width) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetX,
                (Position.Y - hairOrigin.Y) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetY,
                hairTexture.width * AppConfig.ScaleFactor,
                hairTexture.height * AppConfig.ScaleFactor);
            Raylib.DrawTexturePro(hairTexture, hairSrcRec, hairDstRec, Vector2.Zero, 0f, Raylib.WHITE);

        }
    }

    #endregion

    #region Equipment Draw Functions

    private void DrawWeapon()
    {
        if (!_partNodes.ContainsKey(PartType.Weapon)) return;
        var state = _moveStates[(int)_bodyState];
        var body = _parts[PartType.Body];
        var arm = _parts[PartType.Arm];
        var head = _parts[PartType.Head];
        var weapon = _parts[PartType.Weapon];
        if (IsLeft)
        {
            var weaponOrigin = weapon[state].GetOrigin(_bodyFrame, "weapon") -
                               arm[state].GetMap(_bodyFrame, "", "hand") +
                               weapon[state].GetMap(_bodyFrame, "weapon", "hand") -
                               body[state].GetMap(_bodyFrame, "", "navel") +
                               arm[state].GetMap(_bodyFrame, "", "navel");
            var weaponTexture = weapon[state].GetTexture(_bodyFrame, "weapon");
            var weaponSrcRec = new Rectangle(0, 0, weaponTexture.width, weaponTexture.height);
            var weaponDstRec = new Rectangle(
                (Position.X - weaponOrigin.X) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetX,
                (Position.Y - weaponOrigin.Y) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetY,
                weaponTexture.width * AppConfig.ScaleFactor,
                weaponTexture.height * AppConfig.ScaleFactor);

            Raylib.DrawTexturePro(weaponTexture, weaponSrcRec, weaponDstRec, Vector2.Zero, 0f, Raylib.WHITE);
        }
        else
        {
            var weaponOrigin = weapon[state].GetOrigin(_bodyFrame, "weapon") -
                               arm[state].GetMap(_bodyFrame, "", "hand") +
                               weapon[state].GetMap(_bodyFrame, "weapon", "hand") -
                               body[state].GetMap(_bodyFrame, "", "navel") +
                               arm[state].GetMap(_bodyFrame, "", "navel");
            var weaponTexture = weapon[state].GetTexture(_bodyFrame, "weapon");
            var weaponSrcRec = new Rectangle(0, 0, -weaponTexture.width, weaponTexture.height);
            var weaponDstRec = new Rectangle(
                (Position.X + weaponOrigin.X - weaponTexture.width) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetX,
                (Position.Y - weaponOrigin.Y) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetY,
                weaponTexture.width * AppConfig.ScaleFactor,
                weaponTexture.height * AppConfig.ScaleFactor);

            Raylib.DrawTexturePro(weaponTexture, weaponSrcRec, weaponDstRec, Vector2.Zero, 0f, Raylib.WHITE);
        }
    }

    private void DrawCape()
    {
        if (!_partNodes.ContainsKey(PartType.Cape)) return;

        var state = _moveStates[(int)_bodyState];
        var body = _parts[PartType.Body];
        var head = _parts[PartType.Head];
        var cape = _parts[PartType.Cape];
        if (IsLeft)
        {
            var capeOrigin = cape[state].GetOrigin(_bodyFrame, "cape") -
                             body[state].GetMap(_bodyFrame, "", "navel") +
                             cape[state].GetMap(_bodyFrame, "cape", "navel");
            var capeTexture = cape[state].GetTexture(_bodyFrame, "cape");
            var capeSrcRect = new Rectangle(0, 0, capeTexture.width, capeTexture.height);
            var capeDstRec = new Rectangle(
                (Position.X - capeOrigin.X) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetX,
                (Position.Y - capeOrigin.Y) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetY,
                capeTexture.width * AppConfig.ScaleFactor,
                capeTexture.height * AppConfig.ScaleFactor);

            Raylib.DrawTexturePro(capeTexture, capeSrcRect, capeDstRec, Vector2.Zero, 0f, Raylib.WHITE);
        }
        else
        {
            var capeOrigin = cape[state].GetOrigin(_bodyFrame, "cape") -
                             body[state].GetMap(_bodyFrame, "", "navel") +
                             cape[state].GetMap(_bodyFrame, "cape", "navel");
            var capeTexture = cape[state].GetTexture(_bodyFrame, "cape");
            var capeSrcRect = new Rectangle(0, 0, -capeTexture.width, capeTexture.height);
            var capeDstRec = new Rectangle(
                (Position.X + capeOrigin.X - capeTexture.width) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetX,
                (Position.Y - capeOrigin.Y) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetY,
                capeTexture.width * AppConfig.ScaleFactor,
                capeTexture.height * AppConfig.ScaleFactor);

            Raylib.DrawTexturePro(capeTexture, capeSrcRect, capeDstRec, Vector2.Zero, 0f, Raylib.WHITE);
        }
    }

    private void DrawCap()
    {
        if (!_partNodes.ContainsKey(PartType.Cap)) return;
        var state = _moveStates[(int)_bodyState];
        var body = _parts[PartType.Body];
        var arm = _parts[PartType.Arm];
        var head = _parts[PartType.Head];
        var cap = _parts[PartType.Cap];
        if (IsLeft)
        {
            var capOrigin = cap[state].GetOrigin(_bodyFrame, "cap") -
                            body[state].GetMap(_bodyFrame, "", "navel") +
                            cap[state].GetMap(_bodyFrame, "cap", "navel");
            var capTexture = cap[state].GetTexture(_bodyFrame, "cap");
            var capSrcRect = new Rectangle(0, 0, capTexture.width, capTexture.height);
            var capDstRec = new Rectangle(
                (Position.X - capOrigin.X) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetX,
                (Position.Y - capOrigin.Y) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetY,
                capTexture.width * AppConfig.ScaleFactor,
                capTexture.height * AppConfig.ScaleFactor);

            Raylib.DrawTexturePro(capTexture, capSrcRect, capDstRec, Vector2.Zero, 0f, Raylib.WHITE);
        }
        else
        {
            var capOrigin = cap[state].GetOrigin(_bodyFrame, "cap") -
                            body[state].GetMap(_bodyFrame, "", "navel") +
                            cap[state].GetMap(_bodyFrame, "cap", "navel");
            var capTexture = cap[state].GetTexture(_bodyFrame, "cap");
            var capSrcRect = new Rectangle(0, 0, -capTexture.width, capTexture.height);
            var capDstRec = new Rectangle(
                (Position.X + capOrigin.X - capTexture.width) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetX,
                (Position.Y - capOrigin.Y) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetY,
                capTexture.width * AppConfig.ScaleFactor,
                capTexture.height * AppConfig.ScaleFactor);

            Raylib.DrawTexturePro(capTexture, capSrcRect, capDstRec, Vector2.Zero, 0f, Raylib.WHITE);
        }
    }

    private void DrawCoat()
    {
        if (!_partNodes.ContainsKey(PartType.Coat)) return;
        var state = _moveStates[(int)_bodyState];
        var body = _parts[PartType.Body];
        var arm = _parts[PartType.Arm];
        var head = _parts[PartType.Head];
        var coat = _parts[PartType.Coat];
        if (IsLeft)
        {
            var coatOrigin = coat[state].GetOrigin(_bodyFrame, "mail") -
                             body[state].GetMap(_bodyFrame, "", "navel") +
                             coat[state].GetMap(_bodyFrame, "mail", "navel");
            var coatTexture = coat[state].GetTexture(_bodyFrame, "mail");
            var coatSrcRect = new Rectangle(0, 0, coatTexture.width, coatTexture.height);
            var coatDstRec = new Rectangle(
                (Position.X - coatOrigin.X) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetX,
                (Position.Y - coatOrigin.Y) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetY,
                coatTexture.width * AppConfig.ScaleFactor,
                coatTexture.height * AppConfig.ScaleFactor);

            Raylib.DrawTexturePro(coatTexture, coatSrcRect, coatDstRec, Vector2.Zero, 0f, Raylib.WHITE);
        }
        else
        {
            var coatOrigin = coat[state].GetOrigin(_bodyFrame, "mail") -
                             body[state].GetMap(_bodyFrame, "", "navel") +
                             coat[state].GetMap(_bodyFrame, "mail", "navel");
            var coatTexture = coat[state].GetTexture(_bodyFrame, "mail");
            var coatSrcRect = new Rectangle(0, 0, -coatTexture.width, coatTexture.height);
            var coatDstRec = new Rectangle(
                (Position.X + coatOrigin.X - coatTexture.width) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetX,
                (Position.Y - coatOrigin.Y) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetY,
                coatTexture.width * AppConfig.ScaleFactor,
                coatTexture.height * AppConfig.ScaleFactor);

            Raylib.DrawTexturePro(coatTexture, coatSrcRect, coatDstRec, Vector2.Zero, 0f, Raylib.WHITE);
        }
    }

    private void DrawPants()
    {
        if (!_partNodes.ContainsKey(PartType.Pants)) return;
        var state = _moveStates[(int)_bodyState];
        var body = _parts[PartType.Body];
        var pants = _parts[PartType.Pants];
        if (IsLeft)
        {
            var pantsOrigin = pants[state].GetOrigin(_bodyFrame, "pants") -
                              body[state].GetMap(_bodyFrame, "", "navel") +
                              pants[state].GetMap(_bodyFrame, "pants", "navel");
            var pantsTexture = pants[state].GetTexture(_bodyFrame, "pants");
            var pantsSrcRec = new Rectangle(0, 0, pantsTexture.width, pantsTexture.height);
            var pantsDstRec = new Rectangle(
                (Position.X - pantsOrigin.X) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetX,
                (Position.Y - pantsOrigin.Y) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetY,
                pantsTexture.width * AppConfig.ScaleFactor,
                pantsTexture.height * AppConfig.ScaleFactor);

            Raylib.DrawTexturePro(pantsTexture, pantsSrcRec, pantsDstRec, Vector2.Zero, 0f, Raylib.WHITE);
        }
        else
        {
            var pantsOrigin = pants[state].GetOrigin(_bodyFrame, "pants") -
                              body[state].GetMap(_bodyFrame, "", "navel") +
                              pants[state].GetMap(_bodyFrame, "pants", "navel");
            var pantsTexture = pants[state].GetTexture(_bodyFrame, "pants");
            var pantsSrcRec = new Rectangle(0, 0, -pantsTexture.width, pantsTexture.height);
            var pantsDstRec = new Rectangle(
                (Position.X + pantsOrigin.X - pantsTexture.width) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetX,
                (Position.Y - pantsOrigin.Y) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetY,
                pantsTexture.width * AppConfig.ScaleFactor,
                pantsTexture.height * AppConfig.ScaleFactor);

            Raylib.DrawTexturePro(pantsTexture, pantsSrcRec, pantsDstRec, Vector2.Zero, 0f, Raylib.WHITE);
        }
    }

    private void DrawShoes()
    {
        if (!_partNodes.ContainsKey(PartType.Shoes)) return;
        var state = _moveStates[(int)_bodyState];
        var body = _parts[PartType.Body];
        var shoes = _parts[PartType.Pants];
        if (IsLeft)
        {
            var shoeOrigin = shoes[state].GetOrigin(_bodyFrame, "shoe") -
                             body[state].GetMap(_bodyFrame, "", "navel") +
                             shoes[state].GetMap(_bodyFrame, "shoe", "navel");
            var shoeTexture = shoes[state].GetTexture(_bodyFrame, "shoe");
            var shoeSrcRec = new Rectangle(0, 0, shoeTexture.width, shoeTexture.height);
            var shoeDstRec = new Rectangle(
                (Position.X - shoeOrigin.X) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetX,
                (Position.Y - shoeOrigin.Y) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetY,
                shoeTexture.width * AppConfig.ScaleFactor,
                shoeTexture.height * AppConfig.ScaleFactor);

            Raylib.DrawTexturePro(shoeTexture, shoeSrcRec, shoeDstRec, Vector2.Zero, 0f, Raylib.WHITE);
        }
        else
        {
            var shoeOrigin = shoes[state].GetOrigin(_bodyFrame, "shoe") -
                             body[state].GetMap(_bodyFrame, "", "navel") +
                             shoes[state].GetMap(_bodyFrame, "shoe", "navel");
            var shoeTexture = shoes[state].GetTexture(_bodyFrame, "shoe");
            var shoeSrcRec = new Rectangle(0, 0, -shoeTexture.width, shoeTexture.height);
            var shoeDstRec = new Rectangle(
                (Position.X + shoeOrigin.X - shoeTexture.width) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetX,
                (Position.Y - shoeOrigin.Y) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetY,
                shoeTexture.width * AppConfig.ScaleFactor,
                shoeTexture.height * AppConfig.ScaleFactor);

            Raylib.DrawTexturePro(shoeTexture, shoeSrcRec, shoeDstRec, Vector2.Zero, 0f, Raylib.WHITE);
        }
    }

    #endregion

    #endregion


    #region Player Update Functions

    public void Update(float frameTime)
    {
        UpdateBody(frameTime);
        UpdateFace(frameTime);

        if (ControlsLocked) return;
        var input = ServiceLocator.Get<InputManager>();

        if (input.IsKeyDown(KeyboardKey.KEY_LEFT))
        {
            _position.X += 7f;
        }
    }

    private void UpdateBody(float frameTime)
    {
        var state = _moveStates[(int)_bodyState];
        if (_bodyFrameCount <= 1)
            return;

        if (_bodyDelay <= 0)
        {
            _bodyFrame++;
            if (_bodyFrame >= _bodyFrameCount)
                _bodyFrame = 0;
            _bodyDelay = _partNodes[PartType.Body][state][$"{_bodyFrame}"].GetInt("delay");
        }
        else
        {
            _bodyDelay -= (int)frameTime;
        }
    }

    private void UpdateFace(float frameTime)
    {
        if (_faceFrameCount <= 1) return;

        if (_blinkDelay <= 0)
        {
            _faceFrame++;
            if (_faceFrame >= _faceFrameCount)
            {
                _faceFrame = 0;
                _faceDelay = _partNodes[PartType.Face][_faceState]["0"].GetInt("delay");
                _blinkDelay = Random.Shared.Next(3000) * _faceDelay % 30000;
            }
        }

        _blinkDelay -= (int)frameTime;
    }

    #endregion

    #region ID Functions

    private string GetSkinId(int id)
    {
        return id.ToString().PadLeft(3, '0');
    }

    private string GetFaceId(int id)
    {
        return id.ToString().PadLeft(3, '0');
    }

    private string GetItemId(int id)
    {
        return id.ToString().PadLeft(8, '0');
    }

    private int GenderFromId(int id)
    {
        if ((id / 1000000 != 1 && id / 10000 != 254) || id / 10000 == 119 || id / 10000 == 168) return 2;
        return (id / 1000 % 10) switch
        {
            0 => 0, // Male
            1 => 1, // Female
            _ => 2 // Both
        };
    }

    private int GetWeaponCategory(int id)
    {
        return id / 10000 % 100;
    }

    #endregion
}