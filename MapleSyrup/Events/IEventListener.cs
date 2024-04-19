namespace MapleSyrup.Events;

public interface IEventListener
{
    void ProcessEvent(EventType eventType, Variant eventData);
}