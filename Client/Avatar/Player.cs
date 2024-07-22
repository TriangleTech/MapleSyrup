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
    private NxNode? _skinNode, _headNode, _faceNode;
    private Dictionary<AvatarState, BodyPart> _body;
    private Dictionary<AvatarState, BodyPart> _head;
    private Dictionary<AvatarState, BodyPart> _arm;
    private Dictionary<string, BodyPart> _face;
    private int _bodyFrame, _faceFrame, _faceFrameCount, _bodyFrameCount, _bodyDelay, _faceDelay, _blinkDelay;
    private bool _isLeft = false, _isMoving = false;
    private float _scale = 1.0f;
    
    public uint ID { get; set; }
    public string Name { get; set; }
    public int Z { get; set; }
    public bool Visible { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Origin { get; set; }
    public ActorLayer Layer { get; set; }
    public ActorType ActorType { get; set; } = ActorType.Player;
    public Rectangle Bounds { get; set; }
    public NxNode Node { get; set; }

    private readonly List<string> _possibleAvatarStates =
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

    private readonly List<int> _avatarStateFrameCount =
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

    public Player()
    {
        _body = new();
        _head = new();
        _arm = new();
        _face = new();
    }

    public void Init()
    {
        var character = ServiceLocator.Get<NxManager>().Get(MapleFiles.Character);
        _skinNode = character.GetNode("00002000.img");
        _headNode = character.GetNode("00012000.img");
        _faceNode = character.GetNode("Face/00020000.img");

        for (var i = 0; i < _possibleAvatarStates.Count; i++)
        {
            var state = _possibleAvatarStates[i];
            _bodyFrameCount = _skinNode[state].ChildCount;
            _bodyFrame = 0;
            _body[(AvatarState)i] = new(_skinNode[state], BodyType.Body);

            if (state == "dead") _head[(AvatarState)i] = new(_headNode["stand1"], BodyType.Head);
            else _head[(AvatarState)i] = new(_headNode[state], BodyType.Head);

            if (_skinNode[state]["0"].Has("arm"))
                _arm[(AvatarState)i] = new(_skinNode[state], BodyType.Arm);
        }

        _bodyFrameCount = _avatarStateFrameCount[(int)_bodyState];
        _bodyFrame = 0;
        _face[_faceState] = new(_faceNode[_faceState], BodyType.Face);
        _faceFrameCount = _faceNode[_faceState].ChildCount;
        _faceFrame = 0;
    }

    public void ChangeState(AvatarState state)
    {
        if (state == _bodyState) return;
        _bodyState = state;
        _bodyFrameCount = _avatarStateFrameCount[(int)_bodyState];
        _bodyFrame = 0;
    }

    public void Clear()
    {
        foreach (var (_, head) in _head)
            head.Clear();
        foreach (var (_, arm) in _arm)
            arm.Clear();
        foreach (var (_, body) in _body)
            body.Clear();
        _head.Clear();
        _arm.Clear();
        _body.Clear();
        _avatarStateFrameCount.Clear();
        _possibleAvatarStates.Clear();
    }

    #region Player Draw Functions

    public void Draw(float frameTime)
    {
        if (_isLeft)
        {
            DrawOriginal(frameTime);
        }
        else
        {
            DrawFlipped(frameTime);
        }
    }

    private void DrawOriginal(float frameTime)
    {
        var state = _possibleAvatarStates[(int)_bodyState];
        var bodyOrigin = _body[_bodyState].GetOrigin(_bodyFrame);
        var bodyTexture = _body[_bodyState].Frames[_bodyFrame];
        var bodySrcRec = new Rectangle(0, 0, bodyTexture.width, bodyTexture.height);
        var bodyDstRec = new Rectangle(Position.X - bodyOrigin.X, Position.Y - bodyOrigin.Y, bodyTexture.width,
            bodyTexture.height);

        Raylib.DrawTexturePro(bodyTexture, bodySrcRec, bodyDstRec, Vector2.Zero, 0f, Raylib.WHITE);

        var headOrigin = _head[_bodyState].GetOrigin(_bodyFrame) + _head[_bodyState].GetMap(_bodyFrame, "neck") -
                         _body[_bodyState].GetMap(_bodyFrame, "neck");
        var headTexture = _head[_bodyState].Frames[_bodyFrame];
        var headSrcRec = new Rectangle(0, 0, headTexture.width, headTexture.height);
        var headDstRec = new Rectangle(Position.X - headOrigin.X, Position.Y - headOrigin.Y, headTexture.width,
            headTexture.height);

        Raylib.DrawTexturePro(headTexture, headSrcRec, headDstRec, Vector2.Zero, 0f, Raylib.WHITE);

        var showFace = _skinNode?[state][$"{_bodyFrame}"].GetInt("face");
        if (showFace == 1)
        {
            var faceOrigin = _face[_faceState].GetOrigin(_faceFrame) -
                             _body[_bodyState].GetMap(_bodyFrame, "neck") +
                             Vector2.Abs(_head[_bodyState].GetMap(_bodyFrame, "brow") -
                                         _face[_faceState].GetMap(_faceFrame, "brow"));
            var faceTexture = _face[_faceState].Frames[_faceFrame];
            var faceSrcRec = new Rectangle(0, 0, faceTexture.width, faceTexture.height);
            var faceDstRec = new Rectangle(Position.X - faceOrigin.X, Position.Y - faceOrigin.Y,
                faceTexture.width,
                faceTexture.height);
            Raylib.DrawTexturePro(faceTexture, faceSrcRec, faceDstRec, Vector2.Zero, 0f, Raylib.WHITE);
        }

        if (_arm.ContainsKey(_bodyState))
        {
            var armOrigin = _arm[_bodyState].GetOrigin(_bodyFrame) + _arm[_bodyState].GetMap(_bodyFrame, "navel") -
                            _body[_bodyState].GetMap(_bodyFrame, "navel");
            var armTexture = _arm[_bodyState].Frames[_bodyFrame];
            var armSrcRect = new Rectangle(0, 0, armTexture.width, armTexture.height);
            var armDstRec = new Rectangle(Position.X - armOrigin.X, Position.Y - armOrigin.Y, armTexture.width,
                armTexture.height);

            Raylib.DrawTexturePro(armTexture, armSrcRect, armDstRec, Vector2.Zero, 0f, Raylib.WHITE);
        }
    }

    private void DrawFlipped(float frameTime)
    {
        var state = _possibleAvatarStates[(int)_bodyState];
        var bodyOrigin = _body[_bodyState].GetOrigin(_bodyFrame);
        var bodyTexture = _body[_bodyState].Frames[_bodyFrame];
        var bodySrcRec = new Rectangle(0, 0, -bodyTexture.width, bodyTexture.height);
        var bodyDstRec = new Rectangle(Position.X + bodyOrigin.X - (bodyTexture.width),
            Position.Y - bodyOrigin.Y, bodyTexture.width,
            bodyTexture.height);

        Raylib.DrawTexturePro(bodyTexture, bodySrcRec, bodyDstRec, Vector2.Zero, 0f, Raylib.WHITE);

        var headOrigin = _head[_bodyState].GetOrigin(_bodyFrame) + _head[_bodyState].GetMap(_bodyFrame, "neck") -
                         _body[_bodyState].GetMap(_bodyFrame, "neck");
        var headTexture = _head[_bodyState].Frames[_bodyFrame];
        var headSrcRec = new Rectangle(0, 0, -headTexture.width, headTexture.height);
        var headDstRec = new Rectangle(Position.X + headOrigin.X - headTexture.width, Position.Y - headOrigin.Y,
            headTexture.width,
            headTexture.height);

        Raylib.DrawTexturePro(headTexture, headSrcRec, headDstRec, Vector2.Zero, 0f, Raylib.WHITE);

        var showFace = _skinNode?[state][$"{_bodyFrame}"].GetInt("face");
        if (showFace == 1)
        {
            var faceOrigin = _face[_faceState].GetOrigin(_faceFrame) -
                             _body[_bodyState].GetMap(_bodyFrame, "neck") +
                             Vector2.Abs(_head[_bodyState].GetMap(_bodyFrame, "brow") -
                                         _face[_faceState].GetMap(_faceFrame, "brow"));
            var faceTexture = _face[_faceState].Frames[_faceFrame];
            var faceSrcRec = new Rectangle(0, 0, -faceTexture.width, faceTexture.height);
            var faceDstRec = new Rectangle(Position.X + faceOrigin.X - faceTexture.width,
                Position.Y - faceOrigin.Y,
                faceTexture.width,
                faceTexture.height);
            Raylib.DrawTexturePro(faceTexture, faceSrcRec, faceDstRec, Vector2.Zero, 0f, Raylib.WHITE);
        }

        if (_arm.ContainsKey(_bodyState))
        {
            var armOrigin = _arm[_bodyState].GetOrigin(_bodyFrame) + _arm[_bodyState].GetMap(_bodyFrame, "navel") -
                            _body[_bodyState].GetMap(_bodyFrame, "navel");
            var armTexture = _arm[_bodyState].Frames[_bodyFrame];
            var armSrcRect = new Rectangle(0, 0, -armTexture.width, armTexture.height);
            var armDstRec = new Rectangle(Position.X + armOrigin.X - armTexture.width, Position.Y - armOrigin.Y,
                armTexture.width,
                armTexture.height);

            Raylib.DrawTexturePro(armTexture, armSrcRect, armDstRec, Vector2.Zero, 0f, Raylib.WHITE);
        }
    }

    #endregion

    #region Player Update Functions
    public void Update(float frameTime)
    {
        var world = ServiceLocator.Get<WorldManager>();
        var _position = Vector2.Zero;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT) && _bodyState != AvatarState.Ladder)
        {
            _position.X = Position.X - 10.0f;
            _isLeft = true;
            ChangeState(AvatarState.Walk1);
        }
        else if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT) && _bodyState != AvatarState.Ladder)
        {
            _position.X = Position.X + 10.0f;
            _isLeft = false;
            ChangeState(AvatarState.Walk1);
        }
        else if (Raylib.IsKeyDown(KeyboardKey.KEY_UP))
        {
            _position.Y = Position.Y - 10.0f;
            //_position.Y -= _bodyState != AvatarState.Ladder ? 0f : 10.0f;
            _isMoving = true;
        }
        else if (Raylib.IsKeyDown(KeyboardKey.KEY_DOWN))
        {
            _position.Y = Position.Y + 10.0f;
            //_position.Y += _bodyState != AvatarState.Ladder ? 0f : 10.0f;
            _isMoving = true;
            ChangeState(_bodyState != AvatarState.Ladder ? AvatarState.Prone : AvatarState.Ladder);
        }
        else
        {
            ChangeState(_bodyState != AvatarState.Ladder ? AvatarState.Stand1 : AvatarState.Ladder);
            _isMoving = false;
        }

        world.UpdateCamera(Position);
        if (_bodyState != AvatarState.Ladder)
        {
            UpdateBody(frameTime);
        }
        else
        {
            if (_isMoving)
                UpdateBody(frameTime);
        }
        UpdateFace(frameTime);
        Position = _position;
    }

    private void UpdateBody(float frameTime)
    {
        var state = _possibleAvatarStates[(int)_bodyState];
        if (_bodyFrameCount <= 1)
            return;

        if (_bodyDelay <= 0)
        {
            _bodyFrame++;
            if (_bodyFrame >= _bodyFrameCount)
                _bodyFrame = 0;
            _bodyDelay = _skinNode[state][$"{_bodyFrame}"].GetInt("delay");
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
                _faceDelay = _faceNode[_faceState]["0"].GetInt("delay");
                _blinkDelay = Random.Shared.Next(3000) * _faceDelay % 30000;
            }
        }

        _blinkDelay -= (int)frameTime;
    }

    void UpdateZMapping()
    {

    }
    
    #endregion

    public void ProcessPacket(PacketResponse response)
    {
    }
}