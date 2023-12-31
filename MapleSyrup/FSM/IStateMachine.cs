namespace MapleSyrup.FSM;

public interface IStateMachine
{
    public string GetState();
    public void NextState();
}