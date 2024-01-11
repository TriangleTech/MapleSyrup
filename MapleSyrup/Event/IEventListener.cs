namespace MapleSyrup.Event;

public interface IEventListener
{
    EventFlag Flags { get; }
    
    void ProcessEvent(EventFlag flag);

    public static bool operator &(IEventListener listener, EventFlag flag)
    {
        return (listener.Flags & flag) != 0;
    }
}