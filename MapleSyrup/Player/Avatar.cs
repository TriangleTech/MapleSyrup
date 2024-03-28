using System.Runtime.CompilerServices;
using MapleSyrup.EC;
using MapleSyrup.EC.Components;
using MapleSyrup.Managers;
using MapleSyrup.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MapleSyrup.Player;

public class Avatar : IEntity
{
    public string Name { get; set; }
    public EntityFlag Flags { get; set; }
    public ComponentFlag CFlags { get; set; }
    public TransformComponent Transform { get; }
    public RenderLayer Layer { get; set; }
    public Texture2D Texture { get; set; }
    public CameraComponent Camera { get; set; }
    public string SkinId { get; set; }

    private readonly Dictionary<AvatarState, BodyPart> _headStates;
    private readonly Dictionary<AvatarState, BodyPart> _bodyStates;
    private readonly Dictionary<AvatarState, BodyPart> _armStates;
    private readonly ManagerLocator _locator;
    private AvatarState _currentState, _previousState;
    private SpriteEffects flipState = SpriteEffects.None;
    
    public Avatar(ManagerLocator locator)
    {
        _locator = locator;
        _currentState = AvatarState.stand1;
        _headStates = new();
        _bodyStates = new();
        _armStates = new();
        SkinId = "000";
        
        Transform = new TransformComponent(this);
        Layer = RenderLayer.TileObj3;
        Flags = EntityFlag.Active | EntityFlag.PlayerControlled;
        CFlags = ComponentFlag.Transform | ComponentFlag.Equipment;
        ChangeState(AvatarState.stand1);
    }

    public void DrawPlayer(SpriteBatch spriteBatch)
    {
        if (!_bodyStates.ContainsKey(_currentState))
            ChangeState(_currentState);
        
        spriteBatch.Draw(_bodyStates[_currentState].Texture, _bodyStates[_currentState].GetTransform(), null, Color.White, 0f,
            _bodyStates[_currentState].GetOrigin(), 1f, flipState, 0f);
        spriteBatch.Draw(_armStates[_currentState].Texture, _armStates[_currentState].GetTransform(), null, Color.White, 0f,
            _armStates[_currentState].GetOrigin(), 1f, flipState, 0f);
        spriteBatch.Draw(_headStates[_currentState].Texture, _headStates[_currentState].GetTransform(), null, Color.White, 0f,
            _headStates[_currentState].GetOrigin(), 1f, flipState, 0f);
    }

    public void UpdatePlayer(GameTime gameTime)
    {
        if (_bodyStates[_currentState].Advance(gameTime.ElapsedGameTime.Milliseconds))
            _bodyStates[_currentState].NextFrame();
        if (_armStates[_currentState].Advance(gameTime.ElapsedGameTime.Milliseconds))
            _armStates[_currentState].NextFrame();
        if (_headStates[_currentState].Advance(gameTime.ElapsedGameTime.Milliseconds))
            _headStates[_currentState].NextFrame();
        
        _bodyStates[_currentState].UpdateMatrix(_bodyStates[_currentState]);
        _armStates[_currentState].UpdateMatrix(_bodyStates[_currentState]);
        _headStates[_currentState].UpdateMatrix(_bodyStates[_currentState]);
    }

    public void TestInput()
    {
        var keyboard = Keyboard.GetState();
        if (keyboard.IsKeyDown(Keys.Left))
        {
            _previousState = _currentState;
            _currentState = AvatarState.walk1;
        }
        else if (keyboard.IsKeyDown(Keys.Right))
        {
            _previousState = _currentState;
            _currentState = AvatarState.walk1;
        }
        else if (keyboard.IsKeyDown(Keys.Down))
        {
            _previousState = _currentState;
            _currentState = AvatarState.prone;
        }
        else
        {
            _previousState = _currentState;
            _currentState = AvatarState.stand1;
        }
        
        ChangeState(_currentState);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ChangeState(AvatarState state)
    {
        if (_currentState == _previousState)
            return;

        var has = _bodyStates.ContainsKey(state);
        if (!has)
        {
            var resource = _locator.GetManager<ResourceManager>();
           // _bodyStates[state] = resource.LoadBodyPart(this, state, BodyType.Body);
           // _armStates[state] = resource.LoadBodyPart(this, state, BodyType.Arm);
           // _headStates[state] = resource.LoadBodyPart(this, state, BodyType.Head);
        }

        _currentState = state;
    }

    public void CleanUp()
    {
        foreach (var (_, part) in _bodyStates)
        {
            part.Clear();
        }
        foreach (var (_, part) in _armStates)
        {
            part.Clear();
        }
        foreach (var (_, part) in _headStates)
        {
            part.Clear();
        }
    }
}