using MapleSyrup.Events.Enums;

namespace MapleSyrup.Events;

public record struct EventMessage
{
    public int SenderId { get; init; }
    public int ReceiverId { get; init; }
    public EventType EventType { get; init; }
}