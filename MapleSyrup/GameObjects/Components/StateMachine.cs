using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.GameObjects.Components;

public class StateMachine
{
    private readonly Actor _parent;
    private string _currentState;
    private List<string> _possibleStates;
    private Dictionary<string, State> _states;

    public State State => _states[_currentState];
    public string Name => _currentState;

    public StateMachine(Actor actor)
    {
        _parent = actor;
        _possibleStates = new ();
        _states = new();
        _currentState = "stand";
    }

    public void Default()
    {
        _currentState = "stand";
    }

    public bool AddState(string stateName, State state)
    {
        if (!_possibleStates.Contains(stateName))
            _possibleStates.Add(stateName);
        return _states.TryAdd(stateName, state);
    }

    public bool ChangeState(string stateName)
    {
        if (!_possibleStates.Contains(stateName))
            return false; // TODO: Change this to default state.
        _currentState = _possibleStates.Find(state => state == stateName);
        return true;
    }

    public void Clear()
    {
        foreach (var (_, state) in _states)
            state.Clear();
        _possibleStates.Clear();
    }
}