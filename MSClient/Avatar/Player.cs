using System.Numerics;
using MSClient.Actors;
using MSClient.Managers;
using MSClient.Net;
using MSClient.NX;
using Raylib_CsLo;

namespace MSClient.Avatar;

public class Player : Actor
{
    private string _bodyState = "walk1";
    private string _faceState = "blink";
    private NxNode? _skinNode, _headNode, _faceNode;
    private Dictionary<string, BodyPart> _body;
    private Dictionary<string, BodyPart> _head;
    private Dictionary<string, BodyPart> _arm;
    private Dictionary<string, BodyPart> _face;
    private int _bodyFrame, _faceFrame, _faceFrameCount, _bodyFrameCount, _bodyDelay, _faceDelay, _blinkDelay;
    private bool _isLeft = true;
    
    public Player()
        : base(null, ActorType.Player)
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

        _bodyFrameCount = _skinNode[_bodyState].ChildCount;
        _bodyFrame = 0;
        _faceFrameCount = _faceNode[_faceState].ChildCount;
        _faceFrame = 0;
        _body[_bodyState] = new (_skinNode[_bodyState], BodyType.Body);
        _head[_bodyState] = new (_headNode[_bodyState], BodyType.Head);
        _face[_faceState] = new(_faceNode[_faceState], BodyType.Face);
        
        if (_skinNode[_bodyState]["0"].Has("arm"))
            _arm[_bodyState] = new (_skinNode[_bodyState], BodyType.Arm);
    }

    public void ChangeState(string stateName)
    {
        if (_bodyState == stateName)
            return;
        _ = _body.TryAdd(stateName, new (_skinNode[stateName], BodyType.Body));
        _ = _head.TryAdd(stateName, new (_headNode[stateName], BodyType.Head));
        _ = _arm.TryAdd(stateName, new (_skinNode[stateName], BodyType.Arm));
        _bodyFrameCount = _skinNode[stateName].ChildCount;
        _bodyFrame = 0;
        _bodyState = stateName;
    }

    public override void Clear()
    {
        foreach (var (_, head) in _head)
            head.Clear();
        _head.Clear();
        _arm.Clear();
        _body.Clear();
    }

    public override void Draw(float frameTime)
    {
        var bodyOrigin = _body[_bodyState].GetOrigin(_bodyFrame);
        Raylib.DrawTextureEx(_body[_bodyState].Frames[_bodyFrame], Vector2.One - bodyOrigin, 0f, 1.0f, Raylib.WHITE);
        
        var headOrigin = _head[_bodyState].GetOrigin(_bodyFrame) + _head[_bodyState].GetMap(_bodyFrame, "neck") - _body[_bodyState].GetMap(_bodyFrame, "neck");
        Raylib.DrawTextureEx(_head[_bodyState].Frames[_bodyFrame], Vector2.One - headOrigin, 0f, 1.0f, Raylib.WHITE);
        
        if (_skinNode?[_bodyState][$"{_bodyFrame}"].GetInt("face") == 1)
        {
            var faceOrigin = _face[_faceState].GetOrigin(_faceFrame) - _body[_bodyState].GetMap(_bodyFrame, "neck") + 
                             Vector2.Abs(_head[_bodyState].GetMap(_bodyFrame, "brow") - _face[_faceState].GetMap(_faceFrame, "brow"));
            Raylib.DrawTextureEx(_face[_faceState].Frames[_faceFrame], Vector2.One - (faceOrigin), 0f, 1.0f, Raylib.WHITE);
        }

        if (_arm.ContainsKey(_bodyState))
        {
            var armOrigin = _arm[_bodyState].GetOrigin(_bodyFrame) + _arm[_bodyState].GetMap(_bodyFrame, "navel") - _body[_bodyState].GetMap(_bodyFrame, "navel");
            Raylib.DrawTextureEx(_arm[_bodyState].Frames[_bodyFrame], Vector2.One - armOrigin, 0f, 1.0f, Raylib.WHITE);
        }
    }

    public override void Update(float frameTime)
    {
        UpdateBody(frameTime);
        UpdateFace(frameTime);
    }

    private void UpdateBody(float frameTime)
    {
        if (_bodyFrameCount <= 1) return;

        if (_bodyDelay <= 0) {
            _bodyFrame++;
            if (_bodyFrame >= _bodyFrameCount)
                _bodyFrame = 0;
            _bodyDelay = _skinNode[_bodyState][$"{_bodyFrame}"].GetInt("delay");
        } else {
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
                _blinkDelay = Random.Shared.Next(3000) * _faceDelay % 3000;
            }
        }

        _blinkDelay -= (int)frameTime;
    }

    public override void ProcessPacket(PacketResponse response)
    {
    }
}