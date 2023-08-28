namespace MapleSharp.Core.Event;

public class EventSubscriber
{
    public string EventName { get; }
    public object Sender { get; }
    public object Receiver { get; }
    public Action<EventData> Callback { get; }
    
    public EventSubscriber(string eventName, object sender, object receiver, Action<EventData> callback)
    {
        EventName = eventName;
        Sender = sender;
        Receiver = receiver;
        Callback += callback;
    }
}