namespace MapleSyrup.Core.Event;

public class Subscriber
{
    public EventType EventType { get; init; }
    public Action<EventData> Event { get; init; }
    public object Sender { get; init; }
}